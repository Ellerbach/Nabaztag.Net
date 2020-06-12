using Nabaztag.Net.Models;
using Nabaztag.WebTts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.Http.Filters;

namespace Nabaztag.WebTts.Models
{
    /// <summary>
    /// This class is a static as used by different sessions, processes and can handle events from 
    /// the punad service
    /// </summary>
    public static class NabaztagConnection
    {
        private static Nabaztag.Net.Nabaztag _nabaztag;
        private static string _address;
        private static int _port;

        public static string TtsFilePathSettings { get; set; }

        public static bool IsInitalized { get; internal set; }

        public static bool InitializeConnection()
        {
            try
            {
                var ttsSetting = TtsController.LoadTtsSetting(TtsFilePathSettings);
                _address = ttsSetting.NabaztagAddress;
                _port = ttsSetting.NabaztagPort;
                _nabaztag = new Nabaztag.Net.Nabaztag(_address, _port);
                IsInitalized = true;
            }
            catch (Exception)
            {
                IsInitalized = false;
            }

            return IsInitalized;
        }

        public static bool UpdateConnection()
        {
            var ttsSetting = TtsController.LoadTtsSetting(TtsFilePathSettings);

            if ((_address != ttsSetting.NabaztagAddress) || (_port != ttsSetting.NabaztagPort))
            {
                return InitializeConnection();
            }
            else if (!IsInitalized)
            {
                return InitializeConnection();
            }
            return IsInitalized;
        }

        public static (bool status, string error) Speak(string fileToPlay)
        {
            var signature = new Sequence() { AudioList = new string[] { "nabweatherd/signature.mp3" } };
            var body = new Sequence[] { new Sequence() { AudioList = new string[] { fileToPlay } } };
            try
            {
                var resp = _nabaztag.Message(signature, body, DateTime.MinValue);
                return (resp.Status == Status.Ok, "");
            }
            catch (Exception ex)
            {
                return (false, ex.ToString());

            }


        }
    }
}