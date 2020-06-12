using Nabaztag.Net.Models;
using Nabaztag.Server.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nabaztag.Server
{
    public class Choreography
    {
        private const int FrameDurationToMiliseconds = 10;

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
            Attend = 19,
            SetMotorDirection = 20,
        }

        static private List<Color[]> _palettesColors = new List<Color[]>()
        {
            new Color[8] {
                Color.FromArgb(255, 12, 0),
                Color.FromArgb(0, 255, 31),
                Color.FromArgb(255, 242, 0),
                Color.FromArgb(0, 3, 255),
                Color.FromArgb(255, 242, 0),
                Color.FromArgb(0, 255, 31),
                Color.FromArgb(255, 12, 0),
                Color.FromArgb(0, 0, 0)
            },
            new Color[8] {
                Color.FromArgb(95, 0, 255),
                Color.FromArgb(127, 0, 255),
                Color.FromArgb(146, 0, 255),
                Color.FromArgb(191, 0, 255),
                Color.FromArgb(223, 0, 255),
                Color.FromArgb(255, 0, 223),
                Color.FromArgb(255, 0, 146),
                Color.FromArgb(0, 0, 0)
            },
            new Color[8] {
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(0, 0, 0)
            },
            new Color[8] {
                Color.FromArgb(254, 128, 2),
                Color.FromArgb(243, 68, 2),
                Color.FromArgb(216, 6, 7),
                Color.FromArgb(200, 4, 13),
                Color.FromArgb(170, 0, 24),
                Color.FromArgb(218, 5, 96),
                Color.FromArgb(207, 6, 138),
                Color.FromArgb(0, 0, 0)
            },
            new Color[8] {
                Color.FromArgb(20, 155, 18),
                Color.FromArgb(255, 0, 0),
                Color.FromArgb(252, 243, 5),
                Color.FromArgb(20, 155, 18),
                Color.FromArgb(252, 243, 5),
                Color.FromArgb(255, 0, 0),
                Color.FromArgb(20, 155, 18),
                Color.FromArgb(0, 0, 0)
            },
            new Color[8] {
                Color.FromArgb(252, 238, 71),
                Color.FromArgb(206, 59, 69),
                Color.FromArgb(85, 68, 212),
                Color.FromArgb(78, 167, 82),
                Color.FromArgb(243, 75, 153),
                Color.FromArgb(151, 71, 196),
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(0, 0, 0)
            },
            new Color[8] {
                Color.FromArgb(204, 255, 102),
                Color.FromArgb(204, 255, 0),
                Color.FromArgb(153, 255, 0),
                Color.FromArgb(51, 204, 0),
                Color.FromArgb(0, 153, 51),
                Color.FromArgb(0, 136, 0),
                Color.FromArgb(0, 102, 51),
                Color.FromArgb(0, 0, 0)
            },
        };


        static private List<string> _listMidi = new List<string>() {
            "choreographies/1noteA4.mp3",
            "choreographies/1noteB5.mp3",
            "choreographies/1noteBb4.mp3",
            "choreographies/1noteC5.mp3",
            "choreographies/1noteE4.mp3",
            "choreographies/1noteF4.mp3",
            "choreographies/1noteF5.mp3",
            "choreographies/1noteG5.mp3",
            "choreographies/2notesC6C4.mp3",
            "choreographies/2notesC6F5.mp3",
            "choreographies/2notesD4A5.mp3",
            "choreographies/2notesD4G4.mp3",
            "choreographies/2notesD5G4.mp3",
            "choreographies/2notesE5A5.mp3",
            "choreographies/2notesE5C6.mp3",
            "choreographies/2notesE5E4.mp3",
            "choreographies/3notesA4G5G5.mp3",
            "choreographies/3notesB5A5F5.mp3",
            "choreographies/3notesB5D5C6.mp3",
            "choreographies/3notesD4E4G4.mp3",
            "choreographies/3notesE5A5C6.mp3",
            "choreographies/3notesE5C6D5.mp3",
            "choreographies/3notesE5D5A5.mp3",
            "choreographies/3notesF5C6G5.mp3",
        };

        static private int[] _palette = new int[3]
        {
            new Random().Next(0,7),
            new Random().Next(0,7),
            new Random().Next(0,7)
        };

        static private Color[] _currentPalette = new Color[8]
        {
            Color.Black,
            Color.Black,
            Color.Black,
            Color.Black,
            Color.Black,
            Color.Black,
            Color.Black,
            Color.Black,
        };

        private static int _taichiNumber = (new Random().Next(0, 255) * 30) >> 8;
        private static EarDirection[] _taichiEarDirection = new EarDirection[2];

        public static bool IsChoreographyPlaying = false;

        public static void ReadChoreography(Stream fs)
        {
            IsChoreographyPlaying = true;
            int frameDuration = 0;
            _currentPalette = _palettesColors[new Random().Next(0, 7)];

            DateTime dtFrame;
            OpCode opCode;
            int delay;

            while (fs.Position < fs.Length)
            {
                delay = fs.ReadByte();
                dtFrame = DateTime.Now.AddMilliseconds(frameDuration * delay * FrameDurationToMiliseconds);
                while (dtFrame > DateTime.Now)
                {
                    Thread.Sleep(1);
                }

                opCode = (OpCode)fs.ReadByte();

                switch (opCode)
                {
                    case OpCode.FrameDuration:
                        frameDuration = fs.ReadByte();
                        break;
                    case OpCode.SetLedColor:
                        Led led = (Led)fs.ReadByte();
                        int red = fs.ReadByte();
                        int green = fs.ReadByte();
                        int blue = fs.ReadByte();
                        // Read 2 more bytes for nothing
                        fs.ReadByte();
                        fs.ReadByte();
                        NabServer.Leds.SetLedAndDisplay(led, Color.FromArgb(red, green, blue));
                        break;
                    case OpCode.SetMotor:
                        Ear ear = (Ear)fs.ReadByte();
                        int position = fs.ReadByte();
                        var direction = (EarDirection)fs.ReadByte();
                        NabServer.Ears.MoveAbsolute(ear, direction, (byte)position);
                        break;
                    case OpCode.SetLedsColor:
                        int reds = fs.ReadByte();
                        int greens = fs.ReadByte();
                        int blues = fs.ReadByte();
                        NabServer.Leds.SetAllLeds(Color.FromArgb(reds, greens, blues));
                        break;
                    case OpCode.SetLedOff:
                        Led ledOff = (Led)fs.ReadByte();
                        NabServer.Leds.SetAllLeds(Color.Black);
                        break;
                    case OpCode.SetLedPalette:
                        Led ledPalette = (Led)fs.ReadByte();
                        int palette = fs.ReadByte() & 0x03;
                        NabServer.Leds.SetLedAndDisplay(ledPalette, _currentPalette[_palette[palette]]);
                        break;
                    case OpCode.Avance:
                        Ear earAvance = (Ear)fs.ReadByte();
                        int delta = fs.ReadByte();
                        NabServer.Ears.MoveRelative(earAvance, _taichiEarDirection[(int)earAvance], (byte)delta);
                        break;
                    case OpCode.Ifne:
                        int ifneNumber = fs.ReadByte();
                        int sizeHigh = fs.ReadByte();
                        int sizeLow = fs.ReadByte();
                        int size = (sizeHigh << 8) + sizeLow;
                        if (_taichiNumber != ifneNumber)
                        {
                            fs.Position += size;
                        }
                        break;
                    case OpCode.Attend:
                        // Console.WriteLine($"waiting for all movements");
                        while (NabServer.Ears.IsMoving(Ear.Left) || NabServer.Ears.IsMoving(Ear.Right))
                        {
                            Thread.Sleep(1);
                        }

                        break;
                    case OpCode.SetMotorDirection:
                        var motor = fs.ReadByte();
                        var directionMotor = (EarDirection)fs.ReadByte();
                        _taichiEarDirection[motor] = directionMotor;
                        break;
                    case OpCode.RandomMidi:
                        // Console.WriteLine($"Playing a random midi");
                        var fileName = _listMidi[new Random().Next(_listMidi.Count - 1)];
                        string path = Directory.GetCurrentDirectory();
                        while (NabServer.Sound.IsPlaying)
                        {
                            Thread.Sleep(1);
                        }
                        NabServer.Sound.Play($"{path}/sounds/{fileName}");
                        break;
                    case OpCode.Nop:
                        break;
                    default:
                        break;
                }
            }

            IsChoreographyPlaying = false;
        }

        public static void ReadChoreography(string filePath)
        {

            IsChoreographyPlaying = true;

            using (FileStream fs = File.OpenRead(filePath))
            {
                if (fs.Length < 4)
                    throw new Exception("Not a chor file");
                byte[] header = new byte[4];
                fs.Read(header, 0, header.Length);
                if (!((header[0] == 1) && (header[1] == 1) && (header[2] == 1) && (header[3] == 1)))
                    throw new Exception("Not a chor file, header not 1 1 1 1");

                ReadChoreography(fs);
                IsChoreographyPlaying = false;
            }
        }
    }
}
