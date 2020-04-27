// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// Class to create dynamically a Choreography and serialize
    /// </summary>
    public class Choreography
    {
        /// <summary>
        /// Used for the serialization of choreography files.
        /// </summary>
        private enum OpCode
        {
            Nop = 0,
            FrameDuration = 1,
            SetLedColor = 7,
            SetMotor = 8,
            SetLedsColor = 9,
            SetLedOff = 10,
            SetLedPalette = 14,
            RandomMidi = 16,
            Avance = 17,
            Ifne = 18,
            Attentte = 19,
            SetMotorDirection = 20,
        }

        private List<byte> _choreography = new List<byte>();

        /// <summary>
        /// Get the Frame Duration in milliseconds
        /// </summary>
        public int FrameDurationMilliseconds { internal set; get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Choreography()
        {

        }

        /// <summary>
        /// Constructor loading from a file
        /// </summary>
        /// <param name="filePath">file path</param>
        public Choreography(string filePath)
        {
            LoadFromFile(filePath);
        }

        /// <summary>
        /// Load a Choreography from a file
        /// </summary>
        /// <param name="filePath">file path</param>
        public void LoadFromFile(string filePath)
        {
            using (FileStream fs = File.OpenRead(filePath))
            {
                if (fs.Length < 4)
                    throw new Exception("Not a valid chor file, length less than 4");

                byte[] header = new byte[4];
                fs.Read(header, 0, header.Length);
                if (!((header[0] == 1) && (header[1] == 1) && (header[2] == 1) && (header[3] == 1)))
                    throw new Exception("Not a valid chor file, header not 1 1 1 1");

                byte[] toRead = new byte[fs.Length - header.Length];
                fs.Read(toRead, 0, toRead.Length);
                _choreography.AddRange(toRead);
            }
        }

        /// <summary>
        /// Set a wait for choreography to finishes, so basically that ears stop moving and music stop playing
        /// </summary>
        /// <param name="durationMilliseconds">The duration in milliseconds</param>
        /// <param name="waitDurationTimeframeMultiple">duration to wait before executing the command</param>
        public void SetAttente(byte waitDurationTimeframeMultiple = 0)
        {
            _choreography.AddRange(new byte[] {
                waitDurationTimeframeMultiple,  (byte)OpCode.Attentte,
            });
        }

        /// <summary>
        /// Set the frame duration in milliseconds
        /// </summary>
        /// <param name="durationMilliseconds">The duration in milliseconds</param>
        /// <param name="waitDurationTimeframeMultiple">duration to wait before executing the command</param>
        public void SetFrameDuration(byte durationMilliseconds, byte waitDurationTimeframeMultiple = 0)
        {
            _choreography.AddRange(new byte[] {
                waitDurationTimeframeMultiple,  (byte)OpCode.FrameDuration, durationMilliseconds,
            });
            FrameDurationMilliseconds = durationMilliseconds;
        }

        /// <summary>
        /// Set a palette on a led
        /// </summary>
        /// <param name="led">The led to set the palette</param>
        /// <param name="palette">The palette number from 0 to 7</param>
        /// <param name="waitDurationTimeframeMultiple">duration to wait before executing the command</param>
        public void SetLedPalette(Led led, byte palette, byte waitDurationTimeframeMultiple = 0)
        {
            if (palette > 7)
                throw new ArgumentException($"{palette} has to be between 0 and 7");

            _choreography.AddRange(new byte[] {
                waitDurationTimeframeMultiple,  (byte)OpCode.SetLedPalette, (byte)led, palette,
            });
        }

        /// <summary>
        /// Set a random midi play
        /// </summary>
        /// <param name="waitDurationTimeframeMultiple">duration to wait before executing the command</param>
        public void SetRanDomMidi(byte waitDurationTimeframeMultiple = 0)
        {
            _choreography.AddRange(new byte[] {
                waitDurationTimeframeMultiple,  (byte)OpCode.RandomMidi,
            });
        }

        /// <summary>
        /// Add a ear movement
        /// </summary>
        /// <param name="ear">The ear to move</param>
        /// <param name="steps">the steps from -17 to +17</param>
        /// <param name="waitDurationTimeframeMultiple">duration to wait before executing the command</param>
        public void MoveEarAbsolute(Ear ear, int steps, byte waitDurationTimeframeMultiple = 0)
        {
            if ((steps < -17) || (steps > 17))
                throw new ArgumentException($"Ears can only be moved from -17 to +17");

            byte[] chrono = new byte[] {
                waitDurationTimeframeMultiple, (byte)OpCode.SetMotor, (byte)ear, (byte)(steps > 0 ? 0 : 1), (byte)(steps > 0 ? steps : -steps),
            };

            _choreography.AddRange(chrono);
        }

        /// <summary>
        /// Add a ear movement
        /// </summary>
        /// <param name="ear">The ear to move</param>
        /// <param name="steps">the steps from -17 to +17</param>
        /// <param name="waitDurationTimeframeMultiple">duration to wait before executing the command</param>
        public void MoveEarRelative(Ear ear, byte steps, byte waitDurationTimeframeMultiple = 0)
        {
            byte[] chrono = new byte[] {
                waitDurationTimeframeMultiple, (byte)OpCode.Avance, (byte)ear, steps,
            };

            _choreography.AddRange(chrono);
        }

        /// <summary>
        /// Set ear direction for taichi
        /// </summary>
        /// <param name="ear">The ear to move</param>
        /// <param name="forward">true if forward</param>
        /// <param name="waitDurationTimeframeMultiple">duration to wait before executing the command</param>
        public void SetEarDirection(Ear ear, bool forward, byte waitDurationTimeframeMultiple = 0)
        {

            byte[] chrono = new byte[] {
                waitDurationTimeframeMultiple, (byte)OpCode.SetMotorDirection, (byte)(forward ? 0 : 1),
            };

            _choreography.AddRange(chrono);
        }

        /// <summary>
        /// Blink a led
        /// </summary>
        /// <param name="led">The led to blink</param>
        /// <param name="color">The color of the led</param>
        /// <param name="dureationMiliseconds">the duration of blinking</param>
        /// <param name="repeat">Number of time to blink</param>
        /// <param name="waitDurationTimeframeMultiple">duration to wait before executing the command</param>
        public void BlinkLed(Led led, Color color, byte dureationMiliseconds = 100, int repeat = 2, byte waitDurationTimeframeMultiple = 0)
        {
            if (repeat <= 0)
                throw new ArgumentException($"{repeat} has to be positive");

            _choreography.AddRange(new byte[] {
                    waitDurationTimeframeMultiple, (byte)OpCode.SetLedColor, (byte)led, color.R, color.G, color.B, 0, 0,
                    dureationMiliseconds, (byte)OpCode.SetLedColor, (byte)led, 0, 0, 0, 0, 0,
                });

            for (int i = 1; i < repeat; i++)
            {
                _choreography.AddRange(new byte[] {
                    dureationMiliseconds, (byte)OpCode.SetLedColor, (byte)led, color.R, color.G, color.B, 0, 0,
                    dureationMiliseconds, (byte)OpCode.SetLedColor, (byte)led, 0, 0, 0, 0, 0,
                });
            }
        }

        /// <summary>
        /// Blink all leds at the same time with the same color
        /// </summary>
        /// <param name="color">The color of the leds</param>
        /// <param name="dureationMiliseconds">the duration in milliseconds</param>
        /// <param name="repeat">Number of time to blink</param>
        /// <param name="waitDurationTimeframeMultiple">duration to wait before executing the command</param>
        public void BlinkAllLeds(Color color, byte dureationMiliseconds = 100, int repeat = 2, byte waitDurationTimeframeMultiple = 0)
        {
            if (repeat <= 0)
                throw new ArgumentException($"{repeat} has to be positive");

            _choreography.AddRange(new byte[] {
                waitDurationTimeframeMultiple,  (byte)OpCode.SetLedsColor, color.R, color.G, color.B, 0, 0, 0,
                dureationMiliseconds, (byte)OpCode.SetLedsColor, 0, 0, 0, 0, 0, 0,
            });

            for (int i = 1; i < repeat; i++)
            {
                _choreography.AddRange(new byte[] {
                    dureationMiliseconds, (byte)OpCode.SetLedsColor, color.R, color.G, color.B, 0, 0, 0,
                    dureationMiliseconds, (byte)OpCode.SetLedsColor, 0, 0, 0, 0, 0, 0,
                });
            }
        }

        /// <summary>
        /// Set one led to a specific color
        /// </summary>
        /// <param name="led"></param>
        /// <param name="color"></param>
        /// <param name="waitDurationTimeframeMultiple">duration to wait before executing the command</param>
        public void SetLed(Led led, Color color, byte waitDurationTimeframeMultiple = 0)
        {
            _choreography.AddRange(new byte[] {
                waitDurationTimeframeMultiple,  (byte)OpCode.SetLedColor, (byte)led, color.R, color.G, color.B, 0, 0,
            });
        }

        /// <summary>
        /// Set one led to a specific color
        /// </summary>
        /// <param name="led"></param>
        /// <param name="color"></param>
        /// <param name="waitDurationTimeframeMultiple">duration to wait before executing the command</param>
        public void SetLedOff(Led led, byte waitDurationTimeframeMultiple = 0)
        {
            _choreography.AddRange(new byte[] {
                waitDurationTimeframeMultiple,  (byte)OpCode.SetLedOff, (byte)led,
            });
        }

        /// <summary>
        /// Set all leds off
        /// </summary>
        /// <param name="waitDurationTimeframeMultiple">duration to wait before executing the command</param>
        public void SetAllLedsOff(byte waitDurationTimeframeMultiple = 0)
        {
            SetAllLeds(Color.Black, waitDurationTimeframeMultiple);
        }

        /// <summary>
        /// Set all lets to a specific color
        /// </summary>
        /// <param name="color"></param>
        /// <param name="waitDurationTimeframeMultiple">duration to wait before executing the command</param>
        public void SetAllLeds(Color color, byte waitDurationTimeframeMultiple = 0)
        {
            _choreography.AddRange(new byte[] {
                waitDurationTimeframeMultiple,  (byte)OpCode.SetLedsColor, color.R, color.G, color.B,
            });
        }

        /// <summary>
        /// Set Ifne, This is to allow multiple choreography in the same file
        /// It's used byt the Tai Chi
        /// </summary>
        /// <param name="ifneNumber">The number of Ifne</param>
        /// <param name="sizeOfBlock">The size of the Ifne block</param>
        /// <param name="waitDurationTimeframeMultiple">duration to wait before executing the command</param>
        public void SetIfne(byte ifneNumber, int sizeOfBlock, byte waitDurationTimeframeMultiple = 0)
        {
            // Should use for the conversion BinaryPrimitives but coding offline            
            _choreography.AddRange(new byte[] {
                waitDurationTimeframeMultiple,  (byte)OpCode.Ifne, ifneNumber, (byte)(sizeOfBlock >> 8), (byte)(sizeOfBlock & 0xFF),
            });
        }

        /// <summary>
        /// Serialize a binary choreography built before
        /// </summary>
        /// <returns>The string of the encoded choreography</returns>
        public string SerializeChoreography()
        {
            return $"data:application/x-nabaztag-mtl-choreography;base64,{Convert.ToBase64String(_choreography.ToArray())}";
        }

        /// <summary>
        /// Serialize a random streamining choreography
        /// </summary>
        /// <returns>The string of the encoded choreography</returns>
        public string SerializeRandomStreamingChoreography()
        {
            return "urn:x-chor:streaming";
        }

        /// <summary>
        /// Serialize a specific palette choreography
        /// </summary>
        /// <param name="palette">The palette number</param>
        /// <returns>The string of the encoded choreography</returns>
        public string SerializeStreamingChoreography(byte palette)
        {
            return $"urn:x-chor:streaming:{palette}";
        }

        /// <summary>
        /// Save the Choreography to file
        /// </summary>
        /// <param name="filePath">the file path</param>
        public void SaveToFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            using (FileStream fs = File.Create(filePath))
            {
                byte[] header = new byte[] { 1, 1, 1, 1 };
                fs.Write(header, 0, header.Length);
                fs.Write(_choreography.ToArray(), 0, _choreography.Count);
            }
        }

        /// <summary>
        /// Get a byte array representing the Choreography
        /// </summary>
        /// <returns>Byte array</returns>
        public byte[] ToArray() => _choreography.ToArray();
    }
}
