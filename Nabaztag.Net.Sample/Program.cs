// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Nabaztag.Net.Models;
using System;
using System.Threading;

namespace Nabaztag.Net.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            const int TimeToWaitBetweenOperationsMilliseconds = 10_000;

            Console.WriteLine("Hello Nabaztag!");
            Nabaztag nabaztag = new Nabaztag("192.168.1.145", 10543);
            nabaztag.StateEvent += Nabaztag_StateEvent;
            nabaztag.ButtonEvent += Nabaztag_ButtonEvent;
            nabaztag.EarsEvent += Nabaztag_EarsEvent;
            nabaztag.AsrEvent += Nabaztag_AsrEvent;

            // Asking Nabaztag to sleep
            Console.WriteLine("Nabaztag, please sleep :-)");
            var resp = nabaztag.Sleep();
            if (resp.Status == Status.Ok)
                Console.WriteLine("Your Nabaztag is sleeping");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");

            Console.WriteLine($"Waiting {TimeToWaitBetweenOperationsMilliseconds } milliseconds");
            Thread.Sleep(TimeToWaitBetweenOperationsMilliseconds);

            Console.WriteLine("Nabaztag, please wake up!");
            resp = nabaztag.Wakeup();
            if (resp.Status == Status.Ok)
                Console.WriteLine("Your Nabaztag is awake");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");

            // Test ears and leds
            Console.WriteLine("Test ears");
            nabaztag.Test(TestType.Ears);
            Console.WriteLine("Test leds");
            nabaztag.Test(TestType.Leds);

            // Set interactive mode and all the events
            Console.WriteLine("Setting interactive mode and all events, press a key to change mode");
            resp = nabaztag.EventMode(ModeType.Interactive, new EventType[] { EventType.Button, EventType.Ears, EventType.Asr });
            if (resp.Status == Status.Ok)
                Console.WriteLine("Your Nabaztag is in interactive mode and will receive all events");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");

            Console.ReadKey();

            //Console.WriteLine("Setting up Idle mode and all events, press a key to change mode");
            resp = nabaztag.EventMode(ModeType.Idle, new EventType[] { EventType.Button, EventType.Ears, EventType.Asr });
            if (resp.Status == Status.Ok)
                Console.WriteLine("Your Nabaztag is in interactive mode and will receive all events");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");

            Console.ReadKey();

            //// Playing a Choreography and streaming music
            //Sequence seq = new Sequence();
            ////seq.ChoreographyList = new string[] { CreateChoreography().SerializeChoreography() };
            //seq.ChoreographyList = CreateChoreography().SerializeChoreography();
            //seq.AudioList = new string[] { "nabsurprised/respirations/Respiration01.mp3" };
            //// set this one with a timeout
            //resp = nabaztag.Command(seq, true, 30);
            //if (resp.Status == Status.Ok)
            //    Console.WriteLine("Your Nabaztag is doing a respiration and a small choreography");
            //else
            //    Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");

            //Console.WriteLine($"Waiting {TimeToWaitBetweenOperationsMilliseconds } milliseconds");
            //Thread.Sleep(TimeToWaitBetweenOperationsMilliseconds);

            Console.WriteLine("Playing meteo: Today strom, 25 degrees");
            var signature = new Sequence() { AudioList = new string[] { "nabweatherd/signature.mp3" } };
            var body = new Sequence[] { new Sequence() { AudioList = new string[] { "nabweatherd/today.mp3", "nabweatherd/sky/stormy.mp3", "nabweatherd/temp/25.mp3", "nabweatherd/degree.mp3" }, ChoreographyList = CreateChoreography().SerializeChoreography() } };
            resp = nabaztag.Message(signature, body, DateTime.MinValue);            
            if (resp.Status == Status.Ok)
                Console.WriteLine("List played properly");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");

            Console.WriteLine("Trying to the same meteo but with an exprired resquest. Nothing should be played");
            resp = nabaztag.Message(signature, body, DateTime.Now.AddDays(-1));
            if (resp.Status == Status.Expired)
                Console.WriteLine("As planned, this request has expired");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");

            //Reset all events
            Console.WriteLine("Reset all events");
            resp = nabaztag.EventMode(ModeType.Idle, new EventType[] { });
            if (resp.Status == Status.Ok)
                Console.WriteLine("Your Nabaztag is in idle mode");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");

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
