// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Nabaztag.Net.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Nabaztag.Net.Sample
{
    class Program
    {
        static Nabaztag _nabaztag;
        const int TimeToWaitBetweenOperationsMilliseconds = 10_000;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello Nabaztag!");

            if (args.Length > 0)
            {
                _nabaztag = new Nabaztag(args[0], 10543);
            }
            else
            {
                _nabaztag = new Nabaztag("192.168.1.125", 10543);
            }

            _nabaztag.StateEvent += Nabaztag_StateEvent;
            _nabaztag.ButtonEvent += Nabaztag_ButtonEvent;
            _nabaztag.EarsEvent += Nabaztag_EarsEvent;
            _nabaztag.AsrEvent += Nabaztag_AsrEvent;

            //GetStatistics();
            //Thread.Sleep(500);
            //SleepAwake();
            //Tests();
            //PlayMyOwnFiles();
            //SetInteractive();
            //ResetAllEvents();
            SubscribeToEvents();
            //SendInfo();
            //PlayChoreographyCommand();
            //PlayAudioChoreoMessage();
            ResetAllEvents();
        }

        private static void GetStatistics()
        {
            var res = _nabaztag.GetStatistics();
            Console.WriteLine($"Connections: {res.Connections}");
            Console.WriteLine($"Hardware: ");
            Console.WriteLine($"   Model: {res.Hardware?.Model}");
            Console.WriteLine($"   Sound Card: {res.Hardware?.SoundCard}");
            Console.WriteLine($"   Sound Input: {res.Hardware?.SoundInput}");
            Console.WriteLine($"   Is RFID: {res.Hardware?.IsRfid}");
            Console.WriteLine($"   Left ear: {res.Hardware?.LeftEarStatus}");
            Console.WriteLine($"   Right ear: {res.Hardware?.RrightEarStatus}");
            Console.WriteLine($"State: {res.State}");
            Console.WriteLine($"Uptime: {res.Uptime}");
        }

        private static void PlayMyOwnFiles()
        {
            Console.WriteLine($"This will play a sploc and then a nice test voice. Warning: this is only working on a Raspberry Pi only");
            var signature = new Sequence() { AudioList = new string[] { "../../../../nabaztag/sploc.mp3" } };
            var body = new Sequence[] { new Sequence() { AudioList = new string[] { "../../../../nabaztag/test.mp3" } } };
            var resp = _nabaztag.Message(signature, body, DateTime.MinValue);
            if (resp.Status == Status.Ok)
                Console.WriteLine("List played properly");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");
        }

        private static void SleepAwake()
        {
            // Asking Nabaztag to sleep
            Console.WriteLine("Nabaztag, please sleep :-)");
            var resp = _nabaztag.Sleep();
            if (resp.Status == Status.Ok)
                Console.WriteLine("Your Nabaztag is sleeping");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");

            Console.WriteLine($"Waiting {TimeToWaitBetweenOperationsMilliseconds } milliseconds");
            Thread.Sleep(TimeToWaitBetweenOperationsMilliseconds);

            Console.WriteLine("Nabaztag, please wake up!");
            resp = _nabaztag.Wakeup();
            if (resp.Status == Status.Ok)
                Console.WriteLine("Your Nabaztag is awake");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");
        }

        private static void Tests()
        {
            // Test ears and leds
            Console.WriteLine("Test ears");
            _nabaztag.Test(TestType.Ears);
            Console.WriteLine("Test leds");
            _nabaztag.Test(TestType.Leds);
        }

        private static void SetInteractive()
        {
            // Set interactive mode and all the events
            Console.WriteLine("Setting interactive mode and all events, press a key to change mode");
            var resp = _nabaztag.EventMode(ModeType.Interactive, new EventType[] { EventType.Button, EventType.Ears });
            if (resp.Status == Status.Ok)
                Console.WriteLine("Your Nabaztag is in interactive mode and will receive all events");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");

            Console.ReadKey();
            //ResetAllEvents();
        }

        private static void ResetAllEvents()
        {
            //Reset all events
            Console.WriteLine("Reset all events");
            var resp = _nabaztag.EventMode(ModeType.Idle, new EventType[] { });
            if (resp.Status == Status.Ok)
                Console.WriteLine("Your Nabaztag is in idle mode");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");
        }

        private static void SubscribeToEvents()
        {
            //Console.WriteLine("Setting up Idle mode and all events, press a key to change mode");
            var resp = _nabaztag.EventMode(ModeType.Idle, new EventType[] { EventType.Button, EventType.Ears, EventType.Asr }, false, 30);
            if (resp.Status == Status.Ok)
                Console.WriteLine("Your Nabaztag is in Idle mode and will receive all events");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");

            Console.ReadKey();
            //ResetAllEvents();
        }

        private static void PlayChoreographyCommand()
        {
            // Playing a Choreography and streaming music
            Sequence[] seq = new Sequence[] { new Sequence(), new Sequence() };
            //seq.ChoreographyList = new string[] { CreateChoreography().SerializeChoreography() };
            seq[0].ChoreographyList = CreateChoreography().SerializeChoreography();
            seq[1].AudioList = new string[] { "nabsurprised/respirations/Respiration01.mp3" };
            // set this one with a timeout
            var resp = _nabaztag.Command(seq, true, 30);
            if (resp.Status == Status.Ok)
                Console.WriteLine("Your Nabaztag is doing a respiration and a small choreography");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");

            Console.WriteLine($"Waiting {TimeToWaitBetweenOperationsMilliseconds } milliseconds");
            Thread.Sleep(TimeToWaitBetweenOperationsMilliseconds);
        }

        private static void PlayAudioChoreoMessage()
        {
            Console.WriteLine("Playing meteo: Today strom, 25 degrees");
            var signature = new Sequence() { AudioList = new string[] { "nabweatherd/signature.mp3" } };
            var body = new Sequence[] { new Sequence() { AudioList = new string[] { "nabweatherd/today.mp3", "nabweatherd/sky/stormy.mp3", "nabweatherd/temp/25.mp3", "nabweatherd/degree.mp3" }, ChoreographyList = CreateChoreography().SerializeChoreography() } };
            var resp = _nabaztag.Message(signature, body, DateTime.MinValue);
            if (resp.Status == Status.Ok)
                Console.WriteLine("List played properly");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");

            Console.WriteLine("Trying to the same meteo but with an exprired resquest. Nothing should be played");
            resp = _nabaztag.Message(signature, body, DateTime.Now.AddDays(-1));
            if (resp.Status == Status.Expired)
                Console.WriteLine("As planned, this request has expired");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");
        }

        static private void SendInfo()
        {
            Console.WriteLine(" This will send information to play when idel on the nabaztag as an info object");
            // {"type":"info","info_id":"nabairqualityd","animation":{"tempo":14,"colors":[{"left":"000000","center":"00ffff","right":"00ffff"},{"left":"00ffff","center":"00ffff","right":"000000"},{"left":"00ffff","center":"00ffff","right":"00ffff"},{"left":"00ffff","center":"00ffff","right":"00ffff"},{"left":"00ffff","center":"00ffff","right":"00ffff"},{"left":"00ffff","center":"00ffff","right":"00ffff"},{"left":"00ffff","center":"000000","right":"00ffff"},{"left":"000000","center":"000000","right":"00ffff"},{"left":"000000","center":"000000","right":"000000"},{"left":"000000","center":"00ffff","right":"000000"},{"left":"000000","center":"00ffff","right":"00ffff"},{"left":"00ffff","center":"00ffff","right":"00ffff"},{"left":"00ffff","center":"00ffff","right":"00ffff"},{"left":"00ffff","center":"00ffff","right":"00ffff"},{"left":"00ffff","center":"00ffff","right":"000000"},{"left":"00ffff","center":"000000","right":"00ffff"},{"left":"000000","center":"00ffff","right":"00ffff"}]}}
            Info info = new Info() { InfoId = ".netapplication", Animation = new Animation() };
            info.Animation.Tempo = 26;
            List<Colors> colors = new List<Colors>();
            for (int i = 0; i < 30; i++)
            {
                var color = new Colors();
                color.Left = i % 2 == 0 ? "000000" : "ffffff";
                color.Right = i % 2 == 0 ? "ffffff" : "000000";
                color.Center = i % 3 == 0 ? i % 2 == 0 ? "000000" : "ffffff" : "aaaaaa";
                color.Nose = i % 3 == 0 ? i % 2 == 0 ? "aaaaaa" : "000000" : "ffffff";
                color.Bottom = i % 3 == 0 ? i % 2 == 0 ? "ffffff" : "aaaaaa" : "000000";
                colors.Add(color);
            }

            info.Animation.Colors = colors.ToArray();

            _nabaztag.Info(info, true, 30);

            Console.WriteLine($"Waiting {TimeToWaitBetweenOperationsMilliseconds } milliseconds");
            Thread.Sleep(TimeToWaitBetweenOperationsMilliseconds);
        }

        private static void Nabaztag_AsrEvent(object sender, AsrEvent state)
        {
            Console.WriteLine($"New asr event. Intent: {state.Nlu.Intent}, Time: {state.Time}");
        }

        private static void Nabaztag_EarsEvent(object sender, EarsEvent ears)
        {
            Console.WriteLine($"New ear event. Left: {ears.Left} Right: {ears.Right} Ear: {ears.Ear}, Time: {ears.Time}");
        }

        private static void Nabaztag_ButtonEvent(object sender, ButtonEvent buttonEvent)
        {
            Console.WriteLine($"New button event: {buttonEvent.Event}, Time: {buttonEvent.Time}");
        }

        private static void Nabaztag_StateEvent(object sender, NabState state)
        {
            Console.WriteLine($"Nabaztag status changed, new status is {state.State}");
        }

        private static Choreography CreateChoreography()
        {
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

            return choreography;
        }
    }
}
