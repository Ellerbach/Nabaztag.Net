using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"audio":audio_list,"choreography":choreography}
    /// </summary>
    public class Sequence
    {
        /// <summary>
        /// An audio list can be:
        /// * a path of sound separated by ; example: "nabmastodon/communion.wav;otherpath/othersound.mp3". 
        ///   if the sound resource end with "*" or "*.suffixe", a random sound is played in this resource
        /// * a text to speech: "tts:language,text"
        /// </summary>
        [JsonProperty(PropertyName = "audio_list", NullValueHandling = NullValueHandling.Ignore)]
        public string AudioList { get; set; }

        /// <summary>
        /// A choreography list can be:
        /// * like for sound, a list of choreography from the choreographies directory of the various apps
        /// * "urn:x-chor:streaming" to stream a choreography with a random color
        /// * "urn:x-chor:streaming:N" to stream a choreography with a specific color N
        /// </summary>
        [JsonProperty(PropertyName = "choreography", NullValueHandling = NullValueHandling.Ignore)]
        public string Choreography { get; set; }                             
    }
}
