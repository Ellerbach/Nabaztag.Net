// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.DependencyModel;
using Nabaztag.Net.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Nabaztag.Net.Test
{
    public class ChoreographyTests
    {
        [Fact]
        public void LoadFromFile()
        {
            string OriginFile = Path.Combine(".", "1.chor");
            string ToCheckFile = Path.Combine(".", "tocheck.chor");

            // Load the 1.chor file and compare once saved
            Choreography choreography = new Choreography(OriginFile);
            choreography.SaveToFile(ToCheckFile);
            // Load for file and compare
            using (FileStream fsOrigine = File.OpenRead(OriginFile))
            {
                using (FileStream fsToCheck = File.OpenRead(ToCheckFile))
                {
                    Assert.Equal(fsOrigine.Length, fsToCheck.Length);
                    for (int i = 0; i < fsOrigine.Length; i++)
                        Assert.Equal(fsOrigine.ReadByte(), fsToCheck.ReadByte());
                }
            }
        }

        [Fact]
        public void CreateFrameRatePaletteLedsCheckWithFile()
        {
            string OriginFile = Path.Combine(".", "1.chor");

            Choreography choreography = new Choreography();
            // delay: 0, OpCode: FrameDuration frameDuration: 16
            choreography.SetFrameDuration(16);
            // delay: 0, OpCode: SetLedPalette Led: Bottom, palette: 0
            choreography.SetLedPalette(Led.Bottom, 0);
            // delay: 0, OpCode: SetLedPalette Led: Left, palette: 1
            choreography.SetLedPalette(Led.Left, 1);
            // delay: 0, OpCode: SetLedPalette Led: Right, palette: 1
            choreography.SetLedPalette(Led.Right, 1);
            // delay: 0, OpCode: SetLedPalette Led: Nose, palette: 2
            choreography.SetLedPalette(Led.Nose, 2);
            // delay: 1, OpCode: SetLedoff Led: Left
            choreography.SetLedOff(Led.Left, 1);
            // delay: 0, OpCode: SetLedoff Led: Right
            choreography.SetLedOff(Led.Right);
            // delay: 0, OpCode: SetLedPalette Led: Center, palette: 1
            choreography.SetLedPalette(Led.Center, 1);
            // delay: 1, OpCode: SetLedoff Led: Center
            choreography.SetLedOff(Led.Center, 1);
            // delay: 1, OpCode: SetLedoff Led: Nose
            choreography.SetLedOff(Led.Nose, 1);
            // delay: 1, OpCode: SetLedPalette Led: Left, palette: 1
            choreography.SetLedPalette(Led.Left, 1, 1);
            // delay: 1, OpCode: SetLedoff Led: Bottom
            choreography.SetLedOff(Led.Bottom, 1);
            // delay: 0, OpCode: SetLedoff Led: Left
            choreography.SetLedOff(Led.Left);
            // delay: 0, OpCode: SetLedPalette Led: Center, palette: 1
            choreography.SetLedPalette(Led.Center, 1);
            // delay: 0, OpCode: SetLedPalette Led: Nose, palette: 2
            choreography.SetLedPalette(Led.Nose, 2);
            // delay: 1, OpCode: SetLedoff Led: Center
            choreography.SetLedOff(Led.Center, 1);
            // delay: 0, OpCode: SetLedPalette Led: Right, palette: 1
            choreography.SetLedPalette(Led.Right, 1);
            // delay: 1, OpCode: SetLedoff Led: Right
            choreography.SetLedOff(Led.Right, 1);
            // delay: 0, OpCode: SetLedPalette Led: Bottom, palette: 0
            choreography.SetLedPalette(Led.Bottom, 0);
            // delay: 1, OpCode: SetLedoff Led: Nose
            choreography.SetLedOff(Led.Nose, 1);

            var chorToCheck = choreography.ToArray();

            using (FileStream fs = File.OpenRead(OriginFile))
            {
                // Move position past the header
                fs.Position += 4;
                for (int i = 0; i < chorToCheck.Length; i++)
                {
                    Assert.Equal(fs.ReadByte(), chorToCheck[i]);
                }
            }
        }
    }
}
