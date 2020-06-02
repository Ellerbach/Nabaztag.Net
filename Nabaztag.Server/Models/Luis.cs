using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nabaztag.Server.Models
{

    public class Luis
    {
        public string query { get; set; }
        public Prediction prediction { get; set; }
    }

    public class Prediction
    {
        public string topIntent { get; set; }
        public Entities entities { get; set; }
    }    

    public class Entities
    {
    }

}
