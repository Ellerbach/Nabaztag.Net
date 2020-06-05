using Nabaztag.Net.Models;
using Nabaztag.Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nabaztag.Server
{
    public class Ears : IDisposable
    {
        #region libc imports

        [DllImport("libc", SetLastError = true)]
        internal static extern int read(int fd, IntPtr buf, int count);

        [DllImport("libc", SetLastError = true)]
        internal static extern int write(int fd, IntPtr buf, int count);

        [DllImport("libc")]
        internal static extern int close(int fd);

        [DllImport("libc", SetLastError = true)]
        internal static extern int open([MarshalAs(UnmanagedType.LPStr)] string pathname, FileOpenFlags flags);

        internal enum FileOpenFlags
        {
            O_RDONLY = 0x00,
            O_RDWR = 0x02,
            O_NONBLOCK = 0x800,
            O_SYNC = 0x101000
        }

        #endregion

        private const string EarDriver = "/dev/ear";
        private const int Left = 0;
        private const int Right = 1;
        private int[] _earPointer = new int[2];
        private bool[] _isEarMoving = new bool[2];

        public delegate void EarEventHandler(EarsEventArguments earsEventArguments);
        public event EarEventHandler EarEvent;

        public Ears()
        {
            _earPointer[Left] = open($"{EarDriver}{Left}", FileOpenFlags.O_RDWR);
            _earPointer[Right] = open($"{EarDriver}{Right}", FileOpenFlags.O_RDWR);
            _isEarMoving[Left] = false;
            _isEarMoving[Right] = false;
        }

        public void MoveAbsolute(Ear ear, EarDirection direction, byte position, bool blocking = false)
        {
            string earCommand = $"{(direction == EarDirection.Forward ? '>' : '<')}{(char)position}{(blocking ? "" : ".")}";
            byte[] command = Encoding.ASCII.GetBytes(earCommand);
            var earFile = ear == Ear.Left ? Left : Right;
            if (blocking)
            {
                Write(earFile, command);
            }
            else
            {
                new Thread(() =>
                {
                    Write(earFile, command);
                }).Start();
            }

        }

        private unsafe void Write(int earFile, byte[] command)
        {
            while (_isEarMoving[earFile] == true)
            {
                Thread.Sleep(0);
            }

            _isEarMoving[earFile] = true;
            fixed (byte* p = command)
            {
                IntPtr ptr = (IntPtr)p;
                write(_earPointer[earFile], ptr, command.Length);
            }

            _isEarMoving[earFile] = false;
        }

        public void MoveRelative(Ear ear, EarDirection direction, byte steps, bool blocking = false)
        {
            string earCommand = $"{(direction == EarDirection.Forward ? '+' : '-')}{(char)steps}{(blocking ? "" : ".")}";
            byte[] command = Encoding.ASCII.GetBytes(earCommand);
            var earFile = ear == Ear.Left ? Left : Right;
            if (blocking)
            {
                Write(earFile, command);
            }
            else
            {
                new Thread(() =>
                {
                    Write(earFile, command);
                }).Start();
            }
        }

        public unsafe int GetEarPosition(Ear ear)
        {
            string earCommand = "!";
            byte[] command = Encoding.ASCII.GetBytes(earCommand);
            var earFile = ear == Ear.Left ? Left : Right;
            return Position(earFile, command);
        }

        private unsafe int Position(int earFile, byte[] command)
        {
            while (_isEarMoving[earFile] == true)
            {
                Thread.Sleep(0);
            }

            fixed (byte* p = command)
            {
                IntPtr ptr = (IntPtr)p;
                write(_earPointer[earFile], ptr, command.Length);
                var ret = read(_earPointer[earFile], ptr, command.Length);
                if (ret <= 0)
                {
                    return -1;
                }

                return command[0];
            }
        }

        public void ProcessEvents(EarsEventArguments earEventHandler)
        {
            EarEvent?.Invoke(earEventHandler);
        }

        public int DetectEarPosition(Ear ear)
        {
            string earCommand = "?";
            byte[] command = Encoding.ASCII.GetBytes(earCommand);
            var earFile = ear == Ear.Left ? Left : Right;
            return Position(earFile, command);
        }

        public bool IsMoving(Ear ear) => _isEarMoving[ear == Ear.Left ? Left : Right];

        public bool IsAnyMoving => IsMoving(Ear.Left) || IsMoving(Ear.Right);
        public void Dispose()
        {
            if (_earPointer[Left] != -1)
            {
                close(_earPointer[Left]);
            }
            if (_earPointer[Right] != -1)
            {
                close(_earPointer[Right]);
            }
        }
    }
}
