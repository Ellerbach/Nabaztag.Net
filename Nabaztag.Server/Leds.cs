using Nabaztag.Net.Models;
using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Device.Spi;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nabaztag.Server
{
    public class Leds
    {
        private const int LedCount = 5;
        private const int Channel = 1;

        private const int PinPwm = 13;
        private WS281x _ledStrip;

        public Leds()
        {
            var settings = new Settings(800_000, 12);
            settings.Channels[Channel] = new Channel(LedCount, PinPwm, 200, false, StripType.WS2812_STRIP);
            _ledStrip = new WS281x(settings);
        }

        public void SetLed(Led led, Color color)
        {
            try
            {
                _ledStrip.SetLEDColor(Channel, (int)led, color);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Leds exception: {ex}");
            }
        }

        public void SetLedAndDisplay(Led led, Color color)
        {
            try
            {
                _ledStrip.SetLEDColor(Channel, (int)led, color);
                _ledStrip.Render();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Leds exception: {ex}");
            }
        }

        public void SetAllLeds(Color color)
        {
            try
            {
                for (int i = 0; i < LedCount; i++)
                {
                    _ledStrip.SetLEDColor(Channel, i, color);
                }

                _ledStrip.Render();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Leds exception: {ex}");
            }
        }

        public void Render()
        {
            _ledStrip.Render();
        }
    }
}
