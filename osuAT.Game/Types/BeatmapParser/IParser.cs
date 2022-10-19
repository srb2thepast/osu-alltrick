namespace osuAT.Game.Types.BeatmapParsers
{
    /// <summary>
    /// An interface for classes that may parse a line from the [HitObjects] section of a 
    /// .osu file into a HitObject for placement in a <see cref="Beatmap">'s HitObjects list.
    /// Converted maps (ex. from osu! to mania) are currently not supported.
    /// </summary>
    public interface IParser
    {
        public HitObject ParseHitObject(string line) { return new HitObject(); }
    }
}
