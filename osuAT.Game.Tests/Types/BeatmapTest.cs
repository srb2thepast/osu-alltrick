// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using NUnit.Framework;
using osuAT.Game.Types;

namespace osuAT.Game.Tests.Types
{
    [TestFixture]
    public class BeatmapTest
    {
        private Beatmap map;
        [Test]
        public void TestGetContents()
        {
            AddStep("Create new beatmap",createMap);
            Assert.Throws<NotSupportedException>("Throws from immediate access",() => {System.Console.WriteLine(Beatmap.Content)});
            AddStep("Create new beatmap with invalid FolderLocation",() => 
                {createMap(); map.FolderLocation = ""; }
            );
            Assert.Throws<NotSupportedException>("Throws from Loading with invalid Location",() => {map.LoadMapContents()});
            AddStep("Create new beatmap",createMap);
            AddStep("Load Content",map.LoadMapContents());
            AddStep("Access Content", Console.WriteLine(map.Content));
        }

        private void createMap() {
            map = new Beatmap() {
                FolderLocation = "Hi"
            };
            
        }
    }
}
