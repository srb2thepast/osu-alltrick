// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Linq;
using System.Net;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Audio;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Skinning;
using osuAT.Game.Types;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.Mods;
using osu.Framework.Allocation;
using osu.Game.Storyboards;
using Beatmap = osu.Game.Beatmaps.Beatmap;
using FileWebRequest = osu.Framework.IO.Network.FileWebRequest;
using RulesetStore = osuAT.Game.Types.RulesetStore;
using FileInfo = System.IO.FileInfo;
using osu.Game.Resources.Localisation.Web;
using osu.Game.Database;

namespace osuAT.Game
{
    /// <summary>
    /// A <see cref="WorkingBeatmap"/> which reads from a .osu file.
    /// </summary>
    public class ProcessorWorkingBeatmap : WorkingBeatmap
    {
        private readonly Beatmap beatmap;
        [Resolved]
        private AudioManager audioManager { get; set; }

        [Resolved]
        private static AudioManager audio { get; set; }

        /// <summary>
        /// Constructs a new <see cref="ProcessorWorkingBeatmap"/> from a .osu file.
        /// </summary>
        /// <param name="file">The .osu file.</param>
        /// <param name="beatmapId">An optional beatmap ID (for cases where .osu file doesn't have one).</param>
        /// <param name="audioManager"></param>
        public ProcessorWorkingBeatmap(string file, int? beatmapId = null, AudioManager audioManager = null)
            : this(readFromFile(file), beatmapId, audioManager)
        {
            this.audioManager = audioManager;
        }

        private ProcessorWorkingBeatmap(Beatmap beatmap, int? beatmapId = null, AudioManager audioManager = null)
            : base(beatmap.BeatmapInfo, audioManager)
        {
            this.beatmap = beatmap;
            this.audioManager = audioManager;

            beatmap.BeatmapInfo.Ruleset = RulesetStore.ConvertToOsuRuleset(RulesetStore.GetByNum(beatmap.BeatmapInfo.Ruleset.OnlineID)).RulesetInfo;

            if (beatmapId.HasValue)
                beatmap.BeatmapInfo.OnlineID = beatmapId.Value;

            BeatmapSetInfo = beatmap.BeatmapInfo.BeatmapSet ?? new BeatmapSetInfo();
        }


        private static Beatmap readFromFile(string filename)
        {
            using (var stream = File.OpenRead(filename))
            using (var reader = new LineBufferedReader(stream))
                return Decoder.GetDecoder<Beatmap>(reader).Decode(reader);
        }


        public static ProcessorWorkingBeatmap FromFileOrId(string  fileOrId, AudioManager audioManager = null, string cachePath = "cache")
        {
            if (fileOrId.EndsWith(".osu"))
            {
                if (!File.Exists(fileOrId))
                    throw new ArgumentException($"Beatmap file {fileOrId} does not exist.");

                return new ProcessorWorkingBeatmap(fileOrId, null, audioManager?? audio);
            }
            throw new NotImplementedException("osu!apiv2 is not currently supported. Getting by ID requires osu!apiv2.");
            /*
            if (!int.TryParse(fileOrId, out var beatmapId))
                throw new ArgumentException("Could not parse provided beatmap ID.");

            cachePath = Path.Combine(cachePath, $"{beatmapId}.osu");

            if (!File.Exists(cachePath))
            {
                Console.WriteLine($"Downloading {beatmapId}.osu...");

                try
                {
                    new FileWebRequest(cachePath, $"{APIManager.ENDPOINT_CONFIGURATION.WebsiteRootUrl}/osu/{beatmapId}").Perform();
                }
                catch (WebException)
                {
                    // FileWebRequest will sometimes create a file regardless of status
                    if (File.Exists(cachePath))
                        File.Delete(cachePath);

                    throw;
                }

                // FileWebRequest will always create an empty file if the beatmap doesn't exist, clean it up
                if (new System.IO.FileInfo(cachePath).Length == 0)
                    File.Delete(cachePath);

                if (!File.Exists(cachePath))
                    throw new ArgumentException($"Beatmap {beatmapId} does not exist.");
            }

            try
            {
                return new ProcessorWorkingBeatmap(readFromFile(cachePath), beatmapId, audioManager);
            }
            catch (Exception)
            {
                // remove maps that failed to import - its safer to try redownloading it later than keeping a broken map
                File.Delete(cachePath);
                throw;
            }
            */
        }

        // [!] i need a tutorial on how to use trackstore!!!! i hope o!f gets big enough for tutorials one day
        // for now im stealing stuff from osu's editor.cs lul
        protected override Track GetBeatmapTrack() => new TrackVirtual(beatmap.HitObjects.LastOrDefault().StartTime);

        public override IBeatmap GetPlayableBeatmap(IRulesetInfo ruleset, IReadOnlyList<Mod> mods, CancellationToken token)
        {
            Ruleset ruleset2 = ruleset.CreateInstance();
            if (ruleset2 == null)
            {
                throw new RulesetLoadException("Creating ruleset instance failed when attempting to create playable beatmap.");
            }

            IBeatmapConverter beatmapConverter = CreateBeatmapConverter(Beatmap, ruleset2);
            if (Beatmap.HitObjects.Count > 0 && !beatmapConverter.CanConvert())
            {
                throw new BeatmapInvalidForRulesetException(string.Format("{0} can not be converted for the ruleset (ruleset: {1}, converter: {2}).", "Beatmap", ruleset.InstantiationInfo, beatmapConverter));
            }

            foreach (IApplicableToBeatmapConverter item in mods.OfType<IApplicableToBeatmapConverter>())
            {
                token.ThrowIfCancellationRequested();
                item.ApplyToBeatmapConverter(beatmapConverter);
            }

            IBeatmap beatmap = beatmapConverter.Convert(token);
            foreach (IApplicableAfterBeatmapConversion item2 in mods.OfType<IApplicableAfterBeatmapConversion>())
            {
                token.ThrowIfCancellationRequested();
                item2.ApplyToBeatmap(beatmap);
            }

            if (mods.Any((Mod m) => m is IApplicableToDifficulty))
            {
                foreach (IApplicableToDifficulty item3 in mods.OfType<IApplicableToDifficulty>())
                {
                    token.ThrowIfCancellationRequested();
                    item3.ApplyToDifficulty(beatmap.Difficulty);
                }
            }

            IBeatmapProcessor beatmapProcessor = ruleset2.CreateBeatmapProcessor(beatmap);
            if (beatmapProcessor != null)
            {
                foreach (IApplicableToBeatmapProcessor item4 in mods.OfType<IApplicableToBeatmapProcessor>())
                {
                    item4.ApplyToBeatmapProcessor(beatmapProcessor);
                }

                beatmapProcessor.PreProcess();
            }

            foreach (HitObject hitObject in beatmap.HitObjects)
            {
                token.ThrowIfCancellationRequested();
                hitObject.ApplyDefaults(beatmap.ControlPointInfo, beatmap.Difficulty, token);
            }

            foreach (IApplicableToHitObject item5 in mods.OfType<IApplicableToHitObject>())
            {
                foreach (HitObject hitObject2 in beatmap.HitObjects)
                {
                    token.ThrowIfCancellationRequested();
                    item5.ApplyToHitObject(hitObject2);
                }
            }

            foreach (IApplicableToRate item6 in mods.OfType<IApplicableToRate>())
            {
                foreach (HitObject hitObject3 in beatmap.HitObjects)
                {
                    token.ThrowIfCancellationRequested();
                    hitObject3.StartTime = beatmap.HitObjects.FirstOrDefault().StartTime + (hitObject3.StartTime - beatmap.HitObjects.FirstOrDefault().StartTime) * 1/item6.ApplyToRate(hitObject3.StartTime);
                }
            }

            beatmapProcessor?.PostProcess();
            foreach (IApplicableToBeatmap item6 in mods.OfType<IApplicableToBeatmap>())
            {
                token.ThrowIfCancellationRequested();
                item6.ApplyToBeatmap(beatmap);
            }

            return beatmap;
        }


        public new readonly BeatmapSetInfo BeatmapSetInfo;
        protected override IBeatmap GetBeatmap() => beatmap;
        protected override Texture GetBackground() => null;
        protected override ISkin GetSkin() => null;
        public override Stream GetStream(string storagePath) => null;
    }
}
