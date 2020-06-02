﻿using Nabaztag.Net.Models;
using Nabaztag.Server.Models;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nabaztag.Server
{
    public class Button : IDisposable
    {
        private const int HoldTimeout = 2000;
        private const int ClickAndHoldTimeout = 2000;
        private const int DoubleClickTimeout = 150;
        private const int TripleClickTimeout = 150;

        private GpioController _gpioController;
        private bool _shouldDispose;
        private int _pin;
        private Timer _lastActionTimer;
        private int _lastSequence = 0;

        public delegate void ButtonEventHandler(object sender, ButtonEventArguments buttonEventArgs);
        public event ButtonEventHandler ButtonEvent;

        public Button(int pin, GpioController gpioController = null, bool shoudlDispose = true)
        {
            //var unixDriver = UnixDriver.Create();
            var unixDriver = new LibGpiodDriver();
            _gpioController = gpioController ?? new GpioController(PinNumberingScheme.Logical, unixDriver);
            _shouldDispose = shoudlDispose;
            _pin = pin;
            _gpioController.OpenPin(_pin, PinMode.Input);
            _gpioController.RegisterCallbackForPinValueChangedEvent(_pin, PinEventTypes.Falling | PinEventTypes.Rising, pinChangeEvent);
            _lastActionTimer = new Timer(ButtonEventTimer);
        }

        public PinValue PinValue => _gpioController.Read(_pin);

        private void ButtonEventTimer(object state)
        {
            ((Timer)state).Change(Timeout.Infinite, 0);            
            if (_lastSequence == 1)
            {
                ButtonEvent?.Invoke(this, new ButtonEventArguments() { ButtonEventType = ButtonEventType.Hold, DateTimeEvent = DateTime.Now });
            }
            else if (_lastSequence == 2)
            {
                ButtonEvent?.Invoke(this, new ButtonEventArguments() { ButtonEventType = ButtonEventType.Click, DateTimeEvent = DateTime.Now });
            }
            else if (_lastSequence == 3)
            {
                ButtonEvent?.Invoke(this, new ButtonEventArguments() { ButtonEventType = ButtonEventType.ClickAndHold, DateTimeEvent = DateTime.Now });
            }
            else if (_lastSequence == 4)
            {
                ButtonEvent?.Invoke(this, new ButtonEventArguments() { ButtonEventType = ButtonEventType.DoubleClick, DateTimeEvent = DateTime.Now });
            }
            else if (_lastSequence == 5)
            {
                ButtonEvent?.Invoke(this, new ButtonEventArguments() { ButtonEventType = ButtonEventType.ClickAndHold, DateTimeEvent = DateTime.Now });
            }
            else if (_lastSequence == 6)
            {
                ButtonEvent?.Invoke(this, new ButtonEventArguments() { ButtonEventType = ButtonEventType.TripleClick, DateTimeEvent = DateTime.Now });
            }

            _lastSequence = 0;
        }

        private void pinChangeEvent(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            //
            // (0) -- down -> (1) -- timer -> hold
            //                 |
            //                 -- up -> (2) -- timer -> click + goto state 0
            //                           |
            // _________________________|
            //  |
            //  -- down -> (3) -- timer -> click_and_hold
            //              |
            //              -- up -> (4) -- timer -> double click
            //                        |
            //                        -- down -> (5) -- timer -> click_and_hold
            //                                    |
            //                                    -- up -> (6) -- timer -> tripple click!
            //Console.WriteLine($"Pin change: {pinValueChangedEventArgs.ChangeType}");
            var buttonValue = pinValueChangedEventArgs.ChangeType;
            switch (buttonValue)
            {
                case PinEventTypes.Rising:
                    if (_lastSequence == 0)
                    {
                        ButtonEvent?.Invoke(this, new ButtonEventArguments() { ButtonEventType = ButtonEventType.Up, DateTimeEvent = DateTime.Now });
                    }
                    else if (_lastSequence == 1)
                    {
                        ButtonEvent?.Invoke(this, new ButtonEventArguments() { ButtonEventType = ButtonEventType.Up, DateTimeEvent = DateTime.Now });
                        _lastActionTimer.Change(DoubleClickTimeout, 0);
                        _lastSequence++;
                    }
                    else if (_lastSequence == 3)
                    {
                        ButtonEvent?.Invoke(this, new ButtonEventArguments() { ButtonEventType = ButtonEventType.Up, DateTimeEvent = DateTime.Now });
                        _lastActionTimer.Change(TripleClickTimeout, 0);
                        _lastSequence++;
                    }
                    else if (_lastSequence == 5)
                    {
                        ButtonEvent?.Invoke(this, new ButtonEventArguments() { ButtonEventType = ButtonEventType.Up, DateTimeEvent = DateTime.Now });
                        _lastActionTimer.Change(ClickAndHoldTimeout, 0);
                        _lastSequence++;
                    }

                    break;
                case PinEventTypes.Falling:
                    if (_lastSequence == 0)
                    {
                        ButtonEvent?.Invoke(this, new ButtonEventArguments() { ButtonEventType = ButtonEventType.Down, DateTimeEvent = DateTime.Now });
                        _lastActionTimer.Change(HoldTimeout, 0);
                        _lastSequence++;
                    }
                    else if (_lastSequence == 2)
                    {
                        ButtonEvent?.Invoke(this, new ButtonEventArguments() { ButtonEventType = ButtonEventType.Down, DateTimeEvent = DateTime.Now });
                        _lastActionTimer.Change(ClickAndHoldTimeout, 0);
                        _lastSequence++;
                    }
                    else if (_lastSequence == 4)
                    {
                        ButtonEvent?.Invoke(this, new ButtonEventArguments() { ButtonEventType = ButtonEventType.Down, DateTimeEvent = DateTime.Now });
                        _lastActionTimer.Change(TripleClickTimeout, 0);
                        _lastSequence++;
                    }

                    break;
                case PinEventTypes.None:
                default:
                    _lastSequence = 0;
                    break;
            }
        }

        public void Dispose()
        {
            _gpioController?.ClosePin(_pin);
            if (_shouldDispose)
            {
                _gpioController?.Dispose();
                _gpioController = null;
            }
        }
    }
}
