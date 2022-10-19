using System;
using System.IO;
using System.Linq;
using System.Collections.Specialized;
using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Framework.Caching;
using osu.Framework.Extensions.EnumExtensions;
using osu.Framework.Utils;
using osuAT.Game.Types;
using osuTK;

// reminder to organize this and clean up ALL of it
namespace osuAT.Game.Types.BeatmapParsers
{
    public class OsuParser: IParser
    {
        protected static readonly int FormatVersion;
        #region Hit Objects

        public class OsuHitObject: HitObject {
            /// <summary>
            /// The radius of hit objects (ie. the radius of a <see cref="HitCircle"/>).
            /// </summary>
            public const float OBJECT_RADIUS = 64;
            public double Radius => OBJECT_RADIUS * Scale;

            private Bindable<float> scale = new Bindable<float>(1);
            public float Scale
            {
                get => scale.Value;
                set => scale.Value = value;
            }

            private Bindable<int> stackHeight;
            public int StackHeight
            {
                get => stackHeight.Value;
                set => stackHeight.Value = value;
            }

            public virtual Vector2 StackOffset => new Vector2(StackHeight * Scale * -6.4f);
            public Vector2 StackedPosition => Position + StackOffset;

            public virtual Vector2 EndPosition => Position;
            public Vector2 StackedEndPosition => EndPosition + StackOffset;
        }

        /// <summary>
        /// The normal osu! HitCircle.
        /// </summary>
        public class HitCircle : OsuHitObject
        {
        }

        /// <summary>
        /// The osu! Slider.
        /// </summary>
        public class Slider : OsuHitObject
        {
            public int RepeatCount { get; set; }
            public int SpanCount => RepeatCount + 1;
            public double EndTime => StartTime + (SpanCount) * Path.Distance / Velocity;
            public double Duration
            {
                get => EndTime - StartTime;
                set => throw new System.NotSupportedException($"Adjust via {nameof(RepeatCount)} instead"); // can be implemented if/when needed.
            }

            #region IHasPath + WithRepeatsExtensions (osu.Game.Rulesets.Objects.Types.IHasPathWithRepeats.cs)

            private readonly SliderPath path = new SliderPath();
            public SliderPath Path
            {
                get => path;
                set
                {
                    path.ControlPoints.Clear();
                    path.ExpectedDistance.Value = null;

                    if (value != null)
                    {
                        path.ControlPoints.AddRange(value.ControlPoints.Select(c => new PathControlPoint(c.Position, c.Type)));
                        path.ExpectedDistance.Value = value.ExpectedDistance.Value;
                    }
                }
            }

            /// <summary>
            /// Computes the position on the curve relative to how much of the <see cref="HitObject"/> has been completed.
            /// </summary>
            /// <param name="obj">The curve.</param>
            /// <param name="progress">[0, 1] where 0 is the start time of the <see cref="HitObject"/> and 1 is the end time of the <see cref="HitObject"/>.</param>
            /// <returns>The position on the curve.</returns>
            public Vector2 CurvePositionAt(double progress) => Path.PositionAt(ProgressAt(progress));

            /// <summary>
            /// Computes the progress along the curve relative to how much of the <see cref="HitObject"/> has been completed.
            /// </summary>
            /// <param name="obj">The curve.</param>
            /// <param name="progress">[0, 1] where 0 is the start time of the <see cref="HitObject"/> and 1 is the end time of the <see cref="HitObject"/>.</param>
            /// <returns>[0, 1] where 0 is the beginning of the curve and 1 is the end of the curve.</returns>
            public double ProgressAt(double progress)
            {
                double p = progress * SpanCount % 1;
                if (SpanAt(progress) % 2 == 1)
                    p = 1 - p;
                return p;
            }

            /// <summary>
            /// Determines which span of the curve the progress point is on.
            /// </summary>
            /// <param name="obj">The curve.</param>
            /// <param name="progress">[0, 1] where 0 is the beginning of the curve and 1 is the end of the curve.</param>
            /// <returns>[0, SpanCount) where 0 is the first run.</returns>
            public int SpanAt(double progress)
                => (int)(progress * SpanCount);
            #endregion

            public override Vector2 EndPosition => endPositionCache.IsValid ? endPositionCache.Value : endPositionCache.Value = Position + CurvePositionAt(1);
            private readonly Cached<Vector2> endPositionCache = new Cached<Vector2>();
            public Vector2 StackedPositionAt(double t) => StackedPosition + CurvePositionAt(t);

            /// <summary>
            /// The length of one span of this <see cref="Slider"/>.
            /// </summary>
            public double SpanDuration => Duration / SpanCount;

            /// <summary>
            /// Velocity of this <see cref="Slider"/>.
            /// </summary>
            public double Velocity { get; private set; }

            /// <summary>
            /// Spacing between <see cref="SliderTick"/>s of this <see cref="Slider"/>.
            /// </summary>
            public double TickDistance { get; private set; }

            public SliderHeadCircle HeadCircle { get; protected set; }

            public SliderTailCircle TailCircle { get; protected set; }

            /// <summary>
            /// An extra multiplier that affects the number of <see cref="SliderTick"/>s generated by this <see cref="Slider"/>.
            /// An increase in this value increases <see cref="TickDistance"/>, which reduces the number of ticks generated.
            /// </summary>
            public double TickDistanceMultiplier = 1;

            public Slider()
            {
                Path.Version.ValueChanged += _ => updateNestedPositions();
            }

            private void updateNestedPositions()
            {
                endPositionCache.Invalidate();

                if (HeadCircle != null)
                    HeadCircle.Position = Position;

                if (TailCircle != null)
                    TailCircle.Position = EndPosition;
            }
        }

        /// <summary>
        /// The Slider's Head.
        /// </summary>
        public class SliderHeadCircle : HitCircle
        {

        }
        /// <summary>
        /// The Slider's Tail.
        /// </summary>
        public class SliderTailCircle : SliderEndCircle
        {
            public SliderTailCircle(Slider slider)
                : base(slider)
            {
            }
        }
        public abstract class SliderEndCircle : HitCircle
        {
            private readonly Slider slider;

            protected SliderEndCircle(Slider slider)
            {
                this.slider = slider;
            }

            public int RepeatIndex { get; set; }

            public double SpanDuration => slider.SpanDuration;


        }

        /// <summary>
        /// A component of the Slider relating to it's path.
        /// </summary>
        public class SliderPath
        {
            /// <summary>
            /// The current version of this <see cref="SliderPath"/>. Updated when any change to the path occurs.
            /// </summary>
            public IBindable<int> Version => version;

            private readonly Bindable<int> version = new Bindable<int>();

            /// <summary>
            /// The user-set distance of the path. If non-null, <see cref="Distance"/> will match this value,
            /// and the path will be shortened/lengthened to match this length.
            /// </summary>
            public readonly Bindable<double?> ExpectedDistance = new Bindable<double?>();

            public bool HasValidLength => Distance > 0;

            /// <summary>
            /// The control points of the path.
            /// </summary>
            public readonly BindableList<PathControlPoint> ControlPoints = new BindableList<PathControlPoint>();

            private readonly List<Vector2> calculatedPath = new List<Vector2>();
            private readonly List<double> cumulativeLength = new List<double>();
            private readonly Cached pathCache = new Cached();

            private double calculatedLength;

            /// <summary>
            /// Creates a new <see cref="SliderPath"/>.
            /// </summary>
            public SliderPath()
            {
                ExpectedDistance.ValueChanged += _ => invalidate();

                ControlPoints.CollectionChanged += (_, args) =>
                {
                    switch (args.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (var c in args.NewItems.Cast<PathControlPoint>())
                                c.Changed += invalidate;
                            break;

                        case NotifyCollectionChangedAction.Reset:
                        case NotifyCollectionChangedAction.Remove:
                            foreach (var c in args.OldItems.Cast<PathControlPoint>())
                                c.Changed -= invalidate;
                            break;
                    }

                    invalidate();
                };
            }

            /// <summary>
            /// Creates a new <see cref="SliderPath"/> initialised with a list of control points.
            /// </summary>
            /// <param name="controlPoints">An optional set of <see cref="PathControlPoint"/>s to initialise the path with.</param>
            /// <param name="expectedDistance">A user-set distance of the path that may be shorter or longer than the true distance between all control points.
            /// The path will be shortened/lengthened to match this length. If null, the path will use the true distance between all control points.</param>
            public SliderPath(PathControlPoint[] controlPoints, double? expectedDistance = null)
                : this()
            {
                ControlPoints.AddRange(controlPoints);
                ExpectedDistance.Value = expectedDistance;
            }

            /* weird CS 9 upgrade error occuring here
            public SliderPath(PathType type, Vector2[] controlPoints, double? expectedDistance = null)
                : this(controlPoints.Select((c, i) => new PathControlPoint(c, type = i == 0 ? type : null)).ToArray(), expectedDistance)
            {
            }*/

            /// <summary>
            /// The distance of the path after lengthening/shortening to account for <see cref="ExpectedDistance"/>.
            /// </summary>
            public double Distance
            {
                get
                {
                    ensureValid();
                    return cumulativeLength.Count == 0 ? 0 : cumulativeLength[^1];
                }
            }

            /// <summary>
            /// The distance of the path prior to lengthening/shortening to account for <see cref="ExpectedDistance"/>.
            /// </summary>
            public double CalculatedDistance
            {
                get
                {
                    ensureValid();
                    return calculatedLength;
                }
            }

            /// <summary>
            /// Computes the slider path until a given progress that ranges from 0 (beginning of the slider)
            /// to 1 (end of the slider) and stores the generated path in the given list.
            /// </summary>
            /// <param name="path">The list to be filled with the computed path.</param>
            /// <param name="p0">Start progress. Ranges from 0 (beginning of the slider) to 1 (end of the slider).</param>
            /// <param name="p1">End progress. Ranges from 0 (beginning of the slider) to 1 (end of the slider).</param>
            public void GetPathToProgress(List<Vector2> path, double p0, double p1)
            {
                ensureValid();

                double d0 = progressToDistance(p0);
                double d1 = progressToDistance(p1);

                path.Clear();

                int i = 0;

                for (; i < calculatedPath.Count && cumulativeLength[i] < d0; ++i)
                {
                }

                path.Add(interpolateVertices(i, d0));

                for (; i < calculatedPath.Count && cumulativeLength[i] <= d1; ++i)
                    path.Add(calculatedPath[i]);

                path.Add(interpolateVertices(i, d1));
            }

            /// <summary>
            /// Computes the position on the slider at a given progress that ranges from 0 (beginning of the path)
            /// to 1 (end of the path).
            /// </summary>
            /// <param name="progress">Ranges from 0 (beginning of the path) to 1 (end of the path).</param>
            public Vector2 PositionAt(double progress)
            {
                ensureValid();

                double d = progressToDistance(progress);
                return interpolateVertices(indexOfDistance(d), d);
            }

            /// <summary>
            /// Returns the control points belonging to the same segment as the one given.
            /// The first point has a PathType which all other points inherit.
            /// </summary>
            /// <param name="controlPoint">One of the control points in the segment.</param>
            public List<PathControlPoint> PointsInSegment(PathControlPoint controlPoint)
            {
                bool found = false;
                List<PathControlPoint> pointsInCurrentSegment = new List<PathControlPoint>();

                foreach (PathControlPoint point in ControlPoints)
                {
                    if (point.Type != null)
                    {
                        if (!found)
                            pointsInCurrentSegment.Clear();
                        else
                        {
                            pointsInCurrentSegment.Add(point);
                            break;
                        }
                    }

                    pointsInCurrentSegment.Add(point);

                    if (point == controlPoint)
                        found = true;
                }

                return pointsInCurrentSegment;
            }

            private void invalidate()
            {
                pathCache.Invalidate();
                version.Value++;
            }

            private void ensureValid()
            {
                if (pathCache.IsValid)
                    return;

                calculatePath();
                calculateLength();

                pathCache.Validate();
            }

            private void calculatePath()
            {
                calculatedPath.Clear();

                if (ControlPoints.Count == 0)
                    return;

                Vector2[] vertices = new Vector2[ControlPoints.Count];
                for (int i = 0; i < ControlPoints.Count; i++)
                    vertices[i] = ControlPoints[i].Position;

                int start = 0;

                for (int i = 0; i < ControlPoints.Count; i++)
                {
                    if (ControlPoints[i].Type == null && i < ControlPoints.Count - 1)
                        continue;

                    // The current vertex ends the segment
                    var segmentVertices = vertices.AsSpan().Slice(start, i - start + 1);
                    var segmentType = ControlPoints[start].Type ?? PathType.Linear;

                    foreach (Vector2 t in calculateSubPath(segmentVertices, segmentType))
                    {
                        if (calculatedPath.Count == 0 || calculatedPath.Last() != t)
                            calculatedPath.Add(t);
                    }

                    // Start the new segment at the current vertex
                    start = i;
                }
            }

            private List<Vector2> calculateSubPath(ReadOnlySpan<Vector2> subControlPoints, PathType type)
            {
                switch (type)
                {
                    case PathType.Linear:
                        return PathApproximator.ApproximateLinear(subControlPoints);

                    case PathType.PerfectCurve:
                        if (subControlPoints.Length != 3)
                            break;

                        List<Vector2> subPath = PathApproximator.ApproximateCircularArc(subControlPoints);

                        // If for some reason a circular arc could not be fit to the 3 given points, fall back to a numerically stable bezier approximation.
                        if (subPath.Count == 0)
                            break;

                        return subPath;

                    case PathType.Catmull:
                        return PathApproximator.ApproximateCatmull(subControlPoints);
                }

                return PathApproximator.ApproximateBezier(subControlPoints);
            }

            private void calculateLength()
            {
                calculatedLength = 0;
                cumulativeLength.Clear();
                cumulativeLength.Add(0);

                for (int i = 0; i < calculatedPath.Count - 1; i++)
                {
                    Vector2 diff = calculatedPath[i + 1] - calculatedPath[i];
                    calculatedLength += diff.Length;
                    cumulativeLength.Add(calculatedLength);
                }

                if (ExpectedDistance.Value is double expectedDistance && calculatedLength != expectedDistance)
                {
                    // In osu-stable, if the last two control points of a slider are equal, extension is not performed.
                    if (ControlPoints.Count >= 2 && ControlPoints[^1].Position == ControlPoints[^2].Position && expectedDistance > calculatedLength)
                    {
                        cumulativeLength.Add(calculatedLength);
                        return;
                    }

                    // The last length is always incorrect
                    cumulativeLength.RemoveAt(cumulativeLength.Count - 1);

                    int pathEndIndex = calculatedPath.Count - 1;

                    if (calculatedLength > expectedDistance)
                    {
                        // The path will be shortened further, in which case we should trim any more unnecessary lengths and their associated path segments
                        while (cumulativeLength.Count > 0 && cumulativeLength[^1] >= expectedDistance)
                        {
                            cumulativeLength.RemoveAt(cumulativeLength.Count - 1);
                            calculatedPath.RemoveAt(pathEndIndex--);
                        }
                    }

                    if (pathEndIndex <= 0)
                    {
                        // The expected distance is negative or zero
                        // TODO: Perhaps negative path lengths should be disallowed altogether
                        cumulativeLength.Add(0);
                        return;
                    }

                    // The direction of the segment to shorten or lengthen
                    Vector2 dir = (calculatedPath[pathEndIndex] - calculatedPath[pathEndIndex - 1]).Normalized();

                    calculatedPath[pathEndIndex] = calculatedPath[pathEndIndex - 1] + dir * (float)(expectedDistance - cumulativeLength[^1]);
                    cumulativeLength.Add(expectedDistance);
                }
            }

            private int indexOfDistance(double d)
            {
                int i = cumulativeLength.BinarySearch(d);
                if (i < 0) i = ~i;

                return i;
            }

            private double progressToDistance(double progress)
            {
                return Math.Clamp(progress, 0, 1) * Distance;
            }

            private Vector2 interpolateVertices(int i, double d)
            {
                if (calculatedPath.Count == 0)
                    return Vector2.Zero;

                if (i <= 0)
                    return calculatedPath.First();
                if (i >= calculatedPath.Count)
                    return calculatedPath.Last();

                Vector2 p0 = calculatedPath[i - 1];
                Vector2 p1 = calculatedPath[i];

                double d0 = cumulativeLength[i - 1];
                double d1 = cumulativeLength[i];

                // Avoid division by and almost-zero number in case two points are extremely close to each other.
                if (Precision.AlmostEquals(d0, d1))
                    return p0;

                double w = (d - d0) / (d1 - d0);
                return p0 + (p1 - p0) * (float)w;
            }
        }


        public enum PathType
        {
            Catmull,
            Bezier,
            Linear,
            PerfectCurve
        }

        private static PathType convertPathType(string input)
        {
            switch (input[0])
            {
                default:
                case 'C':
                    return PathType.Catmull;

                case 'B':
                    return PathType.Bezier;

                case 'L':
                    return PathType.Linear;

                case 'P':
                    return PathType.PerfectCurve;
            }
        }

        /// <summary>
        /// A component of the Slider's SliderPath.
        /// </summary>
        public class PathControlPoint : IEquatable<PathControlPoint>
        {
            private Vector2 position;

            /// <summary>
            /// The position of this <see cref="PathControlPoint"/>.
            /// </summary>
            public Vector2 Position
            {
                get => position;
                set
                {
                    if (value == position)
                        return;

                    position = value;
                    Changed?.Invoke();
                }
            }

            private PathType? type;

            /// <summary>
            /// The type of path segment starting at this <see cref="PathControlPoint"/>.
            /// If null, this <see cref="PathControlPoint"/> will be a part of the previous path segment.
            /// </summary>
            public PathType? Type
            {
                get => type;
                set
                {
                    if (value == type)
                        return;

                    type = value;
                    Changed?.Invoke();
                }
            }

            /// <summary>
            /// Invoked when any property of this <see cref="PathControlPoint"/> is changed.
            /// </summary>
            public event Action Changed;

            /// <summary>
            /// Creates a new <see cref="PathControlPoint"/>.
            /// </summary>
            public PathControlPoint()
            {
            }

            /// <summary>
            /// Creates a new <see cref="PathControlPoint"/> with a provided position and type.
            /// </summary>
            /// <param name="position">The initial position.</param>
            /// <param name="type">The initial type.</param>
            public PathControlPoint(Vector2 position, PathType? type = null)
                : this()
            {
                Position = position;
                Type = type;
            }

            public bool Equals(PathControlPoint other) => Position == other?.Position && Type == other.Type;
        }

        private static PathControlPoint[] convertPathString(string pointString, Vector2 offset)
        {
            // This code takes on the responsibility of handling explicit segments of the path ("X" & "Y" from above). Implicit segments are handled by calls to convertPoints().
            string[] pointSplit = pointString.Split('|');

            var controlPoints = new List<Memory<PathControlPoint>>();
            int startIndex = 0;
            int endIndex = 0;
            bool first = true;

            while (++endIndex < pointSplit.Length)
            {
                // Keep incrementing endIndex while it's not the start of a new segment (indicated by having a type descriptor of length 1).
                if (pointSplit[endIndex].Length > 1)
                    continue;

                // Multi-segmented sliders DON'T contain the end point as part of the current segment as it's assumed to be the start of the next segment.
                // The start of the next segment is the index after the type descriptor.
                string endPoint = endIndex < pointSplit.Length - 1 ? pointSplit[endIndex + 1] : null;

                controlPoints.AddRange(convertPoints(pointSplit.AsMemory().Slice(startIndex, endIndex - startIndex), endPoint, first, offset));
                startIndex = endIndex;
                first = false;
            }

            if (endIndex > startIndex)
                controlPoints.AddRange(convertPoints(pointSplit.AsMemory().Slice(startIndex, endIndex - startIndex), null, first, offset));

            return mergePointsLists(controlPoints);
        }

        private static PathControlPoint[] mergePointsLists(List<Memory<PathControlPoint>> controlPointList)
        {
            int totalCount = 0;

            foreach (var arr in controlPointList)
                totalCount += arr.Length;

            var mergedArray = new PathControlPoint[totalCount];
            var mergedArrayMemory = mergedArray.AsMemory();
            int copyIndex = 0;

            foreach (var arr in controlPointList)
            {
                arr.CopyTo(mergedArrayMemory.Slice(copyIndex));
                copyIndex += arr.Length;
            }

            return mergedArray;
        }

        /// <summary>
        /// Converts a given point list into a set of path segments.
        /// </summary>
        /// <param name="points">The point list.</param>
        /// <param name="endPoint">Any extra endpoint to consider as part of the points. This will NOT be returned.</param>
        /// <param name="first">Whether this is the first segment in the set. If <c>true</c> the first of the returned segments will contain a zero point.</param>
        /// <param name="offset">The positional offset to apply to the control points.</param>
        /// <returns>The set of points contained by <paramref name="points"/> as one or more segments of the path, prepended by an extra zero point if <paramref name="first"/> is <c>true</c>.</returns>
        private static IEnumerable<Memory<PathControlPoint>> convertPoints(ReadOnlyMemory<string> points, string endPoint, bool first, Vector2 offset)
        {
            PathType type = convertPathType(points.Span[0]);

            int readOffset = first ? 1 : 0; // First control point is zero for the first segment.
            int readablePoints = points.Length - 1; // Total points readable from the base point span.
            int endPointLength = endPoint != null ? 1 : 0; // Extra length if an endpoint is given that lies outside the base point span.

            var vertices = new PathControlPoint[readOffset + readablePoints + endPointLength];

            // Fill any non-read points.
            for (int i = 0; i < readOffset; i++)
                vertices[i] = new PathControlPoint();

            // Parse into control points.
            for (int i = 1; i < points.Length; i++)
                readPoint(points.Span[i], offset, out vertices[readOffset + i - 1]);

            // If an endpoint is given, add it to the end.
            if (endPoint != null)
                readPoint(endPoint, offset, out vertices[^1]);

            // Edge-case rules (to match stable).
            if (type == PathType.PerfectCurve)
            {
                if (vertices.Length != 3)
                    type = PathType.Bezier;
                else if (isLinear(vertices))
                {
                    // osu-stable special-cased colinear perfect curves to a linear path
                    type = PathType.Linear;
                }
            }

            // The first control point must have a definite type.
            vertices[0].Type = type;

            // A path can have multiple implicit segments of the same type if there are two sequential control points with the same position.
            // To handle such cases, this code may return multiple path segments with the final control point in each segment having a non-null type.
            // For the point string X|1:1|2:2|2:2|3:3, this code returns the segments:
            // X: { (1,1), (2, 2) }
            // X: { (3, 3) }
            // Note: (2, 2) is not returned in the second segments, as it is implicit in the path.
            int startIndex = 0;
            int endIndex = 0;

            while (++endIndex < vertices.Length - endPointLength)
            {
                // Keep incrementing while an implicit segment doesn't need to be started.
                if (vertices[endIndex].Position != vertices[endIndex - 1].Position)
                    continue;

                // Legacy Catmull sliders don't support multiple segments, so adjacent Catmull segments should be treated as a single one.
                // Importantly, this is not applied to the first control point, which may duplicate the slider path's position
                // resulting in a duplicate (0,0) control point in the resultant list.
                if (type == PathType.Catmull && endIndex > 1 && FormatVersion < 128)
                    continue;

                // The last control point of each segment is not allowed to start a new implicit segment.
                if (endIndex == vertices.Length - endPointLength - 1)
                    continue;

                // Force a type on the last point, and return the current control point set as a segment.
                vertices[endIndex - 1].Type = type;
                yield return vertices.AsMemory().Slice(startIndex, endIndex - startIndex);

                // Skip the current control point - as it's the same as the one that's just been returned.
                startIndex = endIndex + 1;
            }

            if (endIndex > startIndex)
                yield return vertices.AsMemory().Slice(startIndex, endIndex - startIndex);

            static void readPoint(string value, Vector2 startPos, out PathControlPoint point)
            {
                string[] vertexSplit = value.Split(':');

                Vector2 pos = new Vector2((int)Parsing.ParseDouble(vertexSplit[0], Parsing.MAX_COORDINATE_VALUE), (int)Parsing.ParseDouble(vertexSplit[1], Parsing.MAX_COORDINATE_VALUE)) - startPos;
                point = new PathControlPoint { Position = pos };
            }

            static bool isLinear(PathControlPoint[] p) => Precision.AlmostEquals(0, (p[1].Position.Y - p[0].Position.Y) * (p[2].Position.X - p[0].Position.X)
                                                                                    - (p[1].Position.X - p[0].Position.X) * (p[2].Position.Y - p[0].Position.Y));
        }


        public class Spinner : OsuHitObject {
            public double EndTime
            {
                get => StartTime + Duration;
                set => Duration = value - StartTime;
            }

            public double Duration { get; set; }
        }

            

        internal enum LegacyHitObjectType
        {
            Circle = 1,
            Slider = 1 << 1,
            NewCombo = 1 << 2,
            Spinner = 1 << 3,
            ComboOffset = (1 << 4) | (1 << 5) | (1 << 6),
            Hold = 1 << 7
        }
        #endregion

        public HitObject ParseHitObject(string text)
        {
            string[] split = text.Split(',');

            Vector2 pos = new Vector2((int)Parsing.ParseFloat(split[0], Parsing.MAX_COORDINATE_VALUE), (int)Parsing.ParseFloat(split[1], Parsing.MAX_COORDINATE_VALUE));

            double startTime = Parsing.ParseDouble(split[2]) + 0;

            LegacyHitObjectType type = (LegacyHitObjectType)Parsing.ParseInt(split[3]);

            HitObject result;
            if (type.HasFlagFast(LegacyHitObjectType.Circle))
            {
                // osu.Game.Rulesets.Objects.Legacy.Osu.ConvertHitObjectParser.CreateHit
                result = new HitCircle
                {
                    Position = pos,
                };

            }
            else if (type.HasFlagFast(LegacyHitObjectType.Slider))
            {
                double? length = null;

                int repeatCount = Parsing.ParseInt(split[6]);

                if (repeatCount > 9000)
                    throw new FormatException(@"Repeat count is way too high");

                // osu-stable treated the first span of the slider as a repeat, but no repeats are happening
                repeatCount = Math.Max(0, repeatCount - 1);

                if (split.Length > 7)
                {
                    length = Math.Max(0, Parsing.ParseDouble(split[7], Parsing.MAX_COORDINATE_VALUE));
                    if (length == 0)
                        length = null;
                }

                // One node for each repeat + the start and end nodes
                int nodes = repeatCount + 2;

                // Read any per-node sound types
                
                result = new Slider
                {
                    Position = pos,
                    Path = new SliderPath(convertPathString(split[5], pos), length),
                    RepeatCount = repeatCount,
                };

            }
            else if (type.HasFlagFast(LegacyHitObjectType.Spinner))
            {
                double duration = Math.Max(0, Parsing.ParseDouble(split[5]) + 0 - startTime);

                result = new Spinner
                {
                    Position = pos,
                    Duration = duration
                };
            }
            else if (type.HasFlagFast(LegacyHitObjectType.Hold))
            {
                throw new InvalidDataException($"INVALID OSZ RULESET, HOLD SHOULD NOT APPEAR IN OSU! RULESET. Line: {text}");
            } else
            {
                throw new InvalidDataException($"This shouldn't happen... Line: {text}");
            }

            if (result == null)
                throw new InvalidDataException($"Unknown hit object type: {split[3]}");

            result.StartTime = startTime;

            return result;
        }



    }
}
