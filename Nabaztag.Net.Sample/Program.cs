using Nabaztag.Net.Models;
using Newtonsoft.Json;
using System;

namespace Nabaztag.Net.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");            
            Nabaztag nabaztag = new Nabaztag();
            nabaztag.StateEvent += Nabaztag_StateEvent;
            nabaztag.ButtonEvent += Nabaztag_ButtonEvent;
            nabaztag.EarsEvent += Nabaztag_EarsEvent;
            var resp = nabaztag.Wakeup();
            if (resp.Status == Status.Ok)
                Console.WriteLine("Your Nabaztag is awake");
            else
                Console.WriteLine("Something wrong happened :-(");

            Console.ReadKey();
        }

        private static void Nabaztag_EarsEvent(object sender, EarsEvent ears)
        {
            Console.WriteLine($"New ear event. Left: ${ears.Left} Right: {ears.Right} Ear: {ears.Ear}");
        }

        private static void Nabaztag_ButtonEvent(object sender, ButtonEvent buttonEvent)
        {
            Console.WriteLine($"New button event: {buttonEvent.Event}");
        }

        private static void Nabaztag_StateEvent(object sender, StateObject state)
        {
            Console.WriteLine($"Nabaztag status changed, new status is {state.State}");
        }
    }
}
