using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace Nabaztag.WebTts.Controllers
{
    public class SpeakController : ApiController
    {
        // GET api/values/5
        public string Get(string id)
        {
            var (status, resp) = TtsController.Recognize(id, System.Web.Hosting.HostingEnvironment.MapPath(TtsController.TtsConfigFile), System.Web.Hosting.HostingEnvironment.MapPath("~/" + TtsController.TtsFileName));

            return status ? "ok" : resp;
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
            Get(value);
        }
    }
}
