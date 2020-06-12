using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nabaztag.Server.Models
{
    public class Settings
    {
        public string CognitiveKey { get; set; }
        public string CognitiveRegion { get; set; }
        public string LuisRegion { get; set; }
        public string LuisStage { get; set; }
        public string LuisAddId { get; set; }
        public string LuisKey { get; set; }
        public string Locale { get; set; }
        public string ClockPhrase { get; set; }
        public Voice PrefferedVoice { get; set; }
    }

}
