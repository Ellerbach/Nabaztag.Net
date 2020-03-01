using System;
using System.Buffers.Binary;
using System.IO;
using System.Threading;

namespace Dumpchor
{

    /// <summary>
    /// The Ear enum
    /// </summary>
    public enum Ear
    {
        /// <summary>
        /// Right
        /// </summary>
        Right,
        /// <summary>
        /// Left
        /// </summary>
        Left
    }

    /// <summary>
    /// Enum for the Led on the rabbit
    /// </summary>
    public enum Led
    {
        /// <summary>
        /// Nose
        /// </summary>
        Nose,
        /// <summary>
        /// Right
        /// </summary>
        Right,
        /// <summary>
        /// Center
        /// </summary>
        Center,
        /// <summary>
        /// Left
        /// </summary>
        Left,
        /// <summary>
        /// Bottom
        /// </summary>
        Bottom
    }

    class Program
    {

        private enum OpCode
        {
            Nop = 0,
            FrameDuration = 1,
            SetLedColor = 7,
            SetMotor = 8,
            SetLedsColor = 9,
            SetLedoff = 10,
            SetLedPalette = 14,
            RandomMidi = 16,
            Avance = 17,
            Ifne = 18,
            Attend = 19,
            SetMotorDirection = 20,
            End = 255,
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello dumpchor!");

            foreach (var file in Directory.GetFiles(".", "*.chor"))
            {
                Console.WriteLine($"File: {file}");
                ReadChoreography(file);
                Console.WriteLine();
            }

        }

        static void ReadChoreography(string filePath)
        {
            int frameDuration = 0;

            using (FileStream fs = File.OpenRead(filePath))
            {
                if (fs.Length < 4)
                    throw new Exception("Can't be a chor file");
                byte[] header = new byte[4];
                fs.Read(header, 0, header.Length);
                if (!((header[0] == 1) && (header[1] == 1) && (header[2] == 1) && (header[3] == 1)))
                    throw new Exception("not a chor file, header not 1 1 1 1");

                while (fs.Position < fs.Length)
                {
                    int delay = fs.ReadByte();
                    OpCode opCode = (OpCode)fs.ReadByte();
                    Console.Write($"{nameof(delay)}: {delay}, {nameof(OpCode)}: {opCode} ");

                    switch (opCode)
                    {
                        case OpCode.FrameDuration:
                            frameDuration = fs.ReadByte();
                            Console.WriteLine($"{nameof(frameDuration)}: {frameDuration}");
                            break;
                        case OpCode.SetLedColor:
                            Led led = (Led)fs.ReadByte();
                            int red = fs.ReadByte();
                            int green = fs.ReadByte();
                            int blue = fs.ReadByte();
                            // Read 2 more bytes for nothing
                            fs.ReadByte();
                            fs.ReadByte();
                            Console.WriteLine($"{nameof(Led)}: {led}, {nameof(red)}: {red}, {nameof(green)}: {green}, {nameof(blue)}: {blue}");
                            break;
                        case OpCode.SetMotor:
                            Ear ear = (Ear)fs.ReadByte();
                            int position = fs.ReadByte();
                            int direction = fs.ReadByte();
                            Console.WriteLine($"{nameof(ear)}: {ear}, {nameof(position)}: {position}, {nameof(direction)}: {direction}");
                            break;
                        case OpCode.SetLedsColor:
                            int reds = fs.ReadByte();
                            int greens = fs.ReadByte();
                            int blues = fs.ReadByte();
                            Console.WriteLine($"{nameof(reds)}: {reds}, {nameof(greens)}: {greens}, {nameof(blues)}: {blues}");
                            break;
                        case OpCode.SetLedoff:
                            Led ledOff = (Led)fs.ReadByte();
                            Console.WriteLine($"{nameof(Led)}: {ledOff}");
                            break;
                        case OpCode.SetLedPalette:
                            Led ledPalette = (Led)fs.ReadByte();
                            int palette = fs.ReadByte();
                            Console.WriteLine($"{nameof(Led)}: {ledPalette}, {nameof(palette)}: {palette}");
                            break;
                        case OpCode.Avance:
                            Ear earAvance = (Ear)fs.ReadByte();
                            int delta = fs.ReadByte();
                            int directionAvance = fs.ReadByte();
                            Console.WriteLine($"{nameof(Ear)}: {earAvance}, {nameof(delta)}: {delta}, {nameof(directionAvance)}: {directionAvance}");
                            break;
                        case OpCode.Ifne:
                            // Not sure, should move by 3 in some conditions, otherwise move by the next numbers
                            int value1 = fs.ReadByte();
                            int value2 = fs.ReadByte();
                            int value3 = fs.ReadByte();
                            Console.WriteLine($"{nameof(value1)}: {value1}, {nameof(value2)}: {value2}, {nameof(value3)}: {value3}");                            
                            break;
                        case OpCode.Attend:
                            Console.WriteLine($"waiting for all movements");
                            break;
                        case OpCode.SetMotorDirection:
                            Ear motor = (Ear)fs.ReadByte();
                            int directionMotor = fs.ReadByte();
                            Console.WriteLine($"{nameof(Ear)}: {motor}, {nameof(directionMotor)}: {directionMotor}");
                            break;
                        case OpCode.RandomMidi:
                        case OpCode.Nop:
                        case OpCode.End:
                            Console.WriteLine();
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Wrong or inexisting code");
                            Console.ResetColor();
                            break;
                    }
                }
            }

        }

    }
}
