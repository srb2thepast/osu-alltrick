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
        [Test]
        public void TestGetContents()
        {
            Beatmap map = new Beatmap() {
                FolderLocation = "Hi"
            }
            Assert.Throws<NotSupportedException>("Throws from immediate access",() => {} ) 
        }
    }
}
