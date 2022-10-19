namespace osuAT.Game.Types.BeatmapParsers
{
    public interface IParser
    {
        public HitObject ParseHitObject(string line) { return new HitObject(); }
    }
}
