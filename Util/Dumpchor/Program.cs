// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Nabaztag.Net.Models;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Threading;

namespace Dumpchor
{
    class Program
    {
        // This is a copy from the Choreography file
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

        static void Main(string[] args)
        {
            Console.WriteLine("Hello dump Choreography!");

            foreach (var file in Directory.GetFiles(".", "*.chor"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"File: {file}");
                Console.ResetColor();
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
                    throw new Exception("Not a chor file");
                byte[] header = new byte[4];
                fs.Read(header, 0, header.Length);
                if (!((header[0] == 1) && (header[1] == 1) && (header[2] == 1) && (header[3] == 1)))
                    throw new Exception("Not a chor file, header not 1 1 1 1");

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
                        case OpCode.SetLedOff:
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
                            Console.WriteLine($"{nameof(Ear)}: {earAvance}, {nameof(delta)}: {delta}");
                            break;
                        case OpCode.Ifne:
                            int ifneNumber = fs.ReadByte();
                            int sizeHigh = fs.ReadByte();
                            int sizeLow = fs.ReadByte();
                            int size = (sizeHigh << 8) + sizeLow;                            
                            Console.WriteLine($"{nameof(ifneNumber)}: {ifneNumber}, {nameof(sizeHigh)}: {sizeHigh}, {nameof(sizeLow)}: {sizeLow}, {nameof(size)}: {size}");
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
                            Console.WriteLine();
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Wrong or non existing code");
                            Console.ResetColor();
                            break;
                    }
                }
            }

        }

    }
}
