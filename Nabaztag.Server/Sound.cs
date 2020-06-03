using Iot.Device.Media;
using MP3Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nabaztag.Server
{
    /// <summary>
    /// Class to handle recording and playing sounds
    /// </summary>
    public class Sound
    {
        /// <summary>
        /// When recording, using always the same file name
        /// </summary>
        public const string FileName = "record.wav";

        private bool _recording = false;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Sound()
        {
            IsRecording = false;
            IsPlaying = false;
        }

        /// <summary>
        /// Start recording, default recording chunck is 1 second
        /// </summary>
        public void StartRecording()
        {
            SoundConnectionSettings settings = new SoundConnectionSettings();
            try
            {
                using (SoundDevice device = SoundDevice.Create(settings))
                {
                    string path = Directory.GetCurrentDirectory();

                    device.StartRecording($"{path}/{FileName}");
                    IsRecording = true;
                    _recording = true;
                    new Thread(() =>
                    {
                        try
                        {
                            while (_recording)
                            {
                                Thread.Sleep(1);
                            }

                            device.StopRecording();
                            IsRecording = false;

                        }
                        catch
                        {
                            _recording = false;
                            IsRecording = false;
                        }
                    }).Start();
                }
            }
            catch
            {
                IsRecording = false;
            }

        }

        /// <summary>
        /// Stop recording
        /// </summary>
        public void StopRecording()
        {
            _recording = false;
        }

        /// <summary>
        /// Are we already recording? Need to avoid recording 2 times at the same time
        /// </summary>
        public bool IsRecording { get; internal set; }

        /// <summary>
        /// Are we playing something?
        /// </summary>
        public bool IsPlaying { get; internal set; }

        /// <summary>
        /// Play a sound, supported only WAV and MP3
        /// </summary>
        /// <param name="fileName"></param>
        public void Play(string fileName)
        {
            IsPlaying = true;
            new Thread(() =>
            {
                SoundConnectionSettings settings = new SoundConnectionSettings();
                try
                {
                    using (SoundDevice device = SoundDevice.Create(settings))
                    {
                        // Check the file type
                        if (fileName.Substring(fileName.Length - 3).ToLower() == "mp3")
                        {
                            // open the mp3 file.
                            MP3Stream stream = new MP3Stream(fileName);

                            // Create the buffer.
                            byte[] buffer = new byte[4096];
                            // read the entire mp3 file.
                            int bytesReturned = 1;
                            int totalBytesRead = 0;
                            Stream fsWav = new MemoryStream();

                            while (bytesReturned > 0)
                            {
                                bytesReturned = stream.Read(buffer, 0, buffer.Length);
                                fsWav.Write(buffer, 0, bytesReturned);
                                totalBytesRead += bytesReturned;
                            }
                            // close the stream after we're done with it.
                            stream.Close();
                            stream.Dispose();
                            fsWav.Position = 0;

                            WavHeaderChunk chunk = new WavHeaderChunk
                            {
                                ChunkId = new[] { 'R', 'I', 'F', 'F' },
                                ChunkSize = (uint)totalBytesRead + 36
                            };
                            WavHeaderChunk subChunk1 = new WavHeaderChunk
                            {
                                ChunkId = new[] { 'f', 'm', 't', ' ' },
                                ChunkSize = 16
                            };
                            WavHeaderChunk subChunk2 = new WavHeaderChunk
                            {
                                ChunkId = new[] { 'd', 'a', 't', 'a' },
                                ChunkSize = (uint)totalBytesRead
                            };

                            WavHeader header = new WavHeader
                            {
                                Chunk = chunk,
                                Format = new[] { 'W', 'A', 'V', 'E' },
                                SubChunk1 = subChunk1,
                                AudioFormat = 1,
                                NumChannels = (ushort)(stream.Format == SoundFormat.Pcm16BitMono ? 1 : 2),
                                SampleRate = (uint)stream.Frequency,
                                ByteRate = (uint)(16 * stream.Frequency * (stream.Format == SoundFormat.Pcm16BitMono ? 1 : 2) / 8),
                                BlockAlign = (ushort)((stream.Format == SoundFormat.Pcm16BitMono ? 1 : 2) * 2),
                                BitsPerSample = 16,
                                SubChunk2 = subChunk2
                            };

                            device.WriteWavHeader(fsWav, header);
                            fsWav.Position = 0;
                            device.Play(fsWav);
                            fsWav.Close();
                            fsWav.Dispose();
                        }
                        else
                        {
                            device.Play(fileName);
                        }
                        IsPlaying = false;
                    }
                }
                catch
                {
                    IsPlaying = false;
                }

            }).Start();
        }
    }
}
