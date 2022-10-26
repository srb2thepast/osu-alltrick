using System;
using System.IO;
using osu.Framework;
using osu.Framework.Platform;

namespace SkillAnalyzer
{
    public static class Program
    {
        public static void Main()
        {
            
            using (GameHost host = Host.GetSuitableDesktopHost("SkillAnalyzer"))
            using (var game = new SkillAnalyzerTestBrowser())
                host.Run(game);
        }
    }
}
