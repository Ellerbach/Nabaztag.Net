using Nabaztag.Net.Models;
using Newtonsoft.Json;
using System;

namespace Nabaztag.Net.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Nabaztag!");
            Nabaztag nabaztag = new Nabaztag("192.168.1.145", 10543);
            nabaztag.StateEvent += Nabaztag_StateEvent;
            nabaztag.ButtonEvent += Nabaztag_ButtonEvent;
            nabaztag.EarsEvent += Nabaztag_EarsEvent;
            var resp = nabaztag.Sleep();
            if (resp.Status == Status.Ok)
                Console.WriteLine("Your Nabaztag is sleeping");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");
            
            resp = nabaztag.Wakeup();
            if (resp.Status == Status.Ok)
                Console.WriteLine("Your Nabaztag is awake");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");

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

        private static void Nabaztag_StateEvent(object sender, NabState state)
        {
            Console.WriteLine($"Nabaztag status changed, new status is {state.State}");
        }
    }
}
