using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// Class to create dynaically a Choreography and serialize
    /// </summary>
    public class Choreography
    {
        /// <summary>
        /// Used for the serialization into base 64.
        /// From the python source code
        /// </summary>
        private enum OpcodeHandler
        {
            Nop = 0,
            FrameDuration = 1,
            // set_color = 6,  # 'set_color', but commented
            SetLedColor = 7,
            SetMotor = 8,
            SetLedsColor = 9, // # v16
            SetLedoff = 10, // # v17
            SetLedPalette = 14,
            //set_palette = 15,  # 'set_palette', but commented
            RandomMidi = 16,
            Avance = 17,
            Ifne = 18,  //# only used for taichi
            Attend = 19,
            SetMotorDirection = 20,  //# v16
        }

        private List<byte> _choreography = new List<byte>();

        /// <summary>
        /// Set a palette on a led
        /// </summary>
        /// <param name="led">The led to set the palette</param>
        /// <param name="palette">The palette number from 0 to 7</param>
        public void SetLedPalette(Led led, byte palette)
        {
            if (palette > 7)
                throw new ArgumentException($"{palette} has to be between 0 and 7");

            _choreography.Concat(new byte[] {
                0,  (byte)OpcodeHandler.SetLedColor, (byte)led, palette, 0, 0, 0, 0,
            });
        }

        /// <summary>
        /// Add a ear movement
        /// </summary>
        /// <param name="ear">The ear to move</param>
        /// <param name="steps">the steps from -17 to +17</param>
        public void MoveEar(Ear ear, int steps)
        {
            if ((steps < -17) || (steps > 17))
                throw new ArgumentException($"Ears can only be moved from -17 to +17");

            byte[] chrono = new byte[] {
                0, (byte)OpcodeHandler.SetMotorDirection, (byte)ear, (byte)(steps > 0 ? 0 : 1),
                0, (byte)OpcodeHandler.Avance, (byte)ear, (byte)(steps > 0 ? steps : -steps)
            };

            _choreography.Concat(chrono);
        }

        /// <summary>
        /// Blink a led
        /// </summary>
        /// <param name="led">The led to blink</param>
        /// <param name="color">The color of the led</param>
        /// <param name="dureationMiliseconds">the duration of blinking</param>
        /// <param name="repeat">Number of time to blink</param>
        public void BlinkLed(Led led, Color color, byte dureationMiliseconds = 100, int repeat = 2)
        {
            if (repeat <= 0)
                throw new ArgumentException($"{repeat} has to be positive");

            _choreography.Concat(new byte[] {
                0,  (byte)OpcodeHandler.FrameDuration, dureationMiliseconds
            });

            _choreography.Concat(new byte[] {
                0,  (byte)OpcodeHandler.SetLedColor, (byte)led, color.R, color.G, color.B, 0, 0,
                15, (byte)OpcodeHandler.SetLedColor, (byte)led, 0, 0, 0, 0, 0,
            });

            for (int i = 1; i < repeat; i++)
            {
                _choreography.Concat(new byte[] {
                    15, (byte)OpcodeHandler.SetLedColor, (byte)led, color.R, color.G, color.B, 0, 0,
                    15, (byte)OpcodeHandler.SetLedColor, (byte)led, 0, 0, 0, 0, 0,
                });
            }
        }

        /// <summary>
        /// Blink all leds at the same time with the same color
        /// </summary>
        /// <param name="color">The color of the leds</param>
        /// <param name="dureationMiliseconds">the duration in milliseconds</param>
        /// <param name="repeat">Number of time to blink</param>
        public void BlinkAllLeds(Color color, byte dureationMiliseconds = 100, int repeat = 2)
        {
            if (repeat <= 0)
                throw new ArgumentException($"{repeat} has to be positive");

            _choreography.Concat(new byte[] {
                0,  (byte)OpcodeHandler.FrameDuration, dureationMiliseconds
            });

            _choreography.Concat(new byte[] {
                0,  (byte)OpcodeHandler.SetLedsColor, color.R, color.G, color.B, 0, 0, 0,
                15, (byte)OpcodeHandler.SetLedsColor, 0, 0, 0, 0, 0, 0,
            });

            for (int i = 1; i < repeat; i++)
            {
                _choreography.Concat(new byte[] {
                    15, (byte)OpcodeHandler.SetLedsColor, color.R, color.G, color.B, 0, 0, 0,
                    15, (byte)OpcodeHandler.SetLedsColor, 0, 0, 0, 0, 0, 0,
                });
            }
        }

        /// <summary>
        /// Set one led to a specific color
        /// </summary>
        /// <param name="led"></param>
        /// <param name="color"></param>
        public void SetLed(Led led, Color color)
        {
            _choreography.Concat(new byte[] {
                0,  (byte)OpcodeHandler.SetLedColor, (byte)led, color.R, color.G, color.B, 0, 0,
            });
        }

        /// <summary>
        /// Set one led to a specific color
        /// </summary>
        /// <param name="led"></param>
        /// <param name="color"></param>
        public void SetLedOff(Led led)
        {
            SetLed(led, Color.Black);
        }

        /// <summary>
        /// Set all leds off
        /// </summary>
        public void SetAllLedsOff()
        {
            SetAllLeds(Color.Black);
        }

        /// <summary>
        /// Set all lets to a specific color
        /// </summary>
        /// <param name="color"></param>
        public void SetAllLeds(Color color)
        {
            _choreography.Concat(new byte[] {
                0,  (byte)OpcodeHandler.SetLedsColor, color.R, color.G, color.B, 0, 0, 0,
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

    }
}
