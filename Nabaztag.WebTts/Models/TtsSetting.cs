using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nabaztag.WebTts.Models
{
    /// <summary>
    /// Settings for the TTS paramters
    /// </summary>
    public class TtsSetting
    {
        /// <summary>
        /// Azure Cognitive services key
        /// </summary>
        [Display(Name = nameof(Resources.Tts.Key), ResourceType = typeof(Resources.Tts))]
        public string Key { get; set; }

        /// <summary>
        /// Specific endpoint
        /// </summary>
        [Display(Name = nameof(Resources.Tts.EndPoint), ResourceType = typeof(Resources.Tts))]
        public string EndPoint { get; set; }

        /// <summary>
        /// The preferred short name
        /// </summary>
        [Display(Name = nameof(Resources.Tts.PrefferedShortName), ResourceType = typeof(Resources.Tts))]
        public string PrefferedShortName { get; set; }

        /// <summary>
        /// Preferred voice details, stored to avoid a request to Azure
        /// </summary>
        public Voice PrefferedVoice { get; set; }

        /// <summary>
        /// Application path from ~/home
        /// </summary>
        [Display(Name = nameof(Resources.Tts.ApplicationPath), ResourceType = typeof(Resources.Tts))]
        public string ApplicationPath { get; set; }

        /// <summary>
        /// Nabaztag IP address
        /// </summary>
        [Display(Name = nameof(Resources.Tts.NabaztagAddress), ResourceType = typeof(Resources.Tts))]
        public string NabaztagAddress { get; set; } = "localhost";

        /// <summary>
        /// Nabaztag Port
        /// </summary>
        [Display(Name = nameof(Resources.Tts.NabaztagPort), ResourceType = typeof(Resources.Tts))]
        public int NabaztagPort { get; set; } = 10543;

        /// <summary>
        /// The language to display information
        /// </summary>
        [Display(Name = nameof(Resources.Tts.Language), ResourceType = typeof(Resources.Tts))]
        public string Language { get; set; }
    }
}