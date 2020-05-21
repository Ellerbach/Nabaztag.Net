using Nabaztag.WebTts.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace Nabaztag.WebTts
{
    public class Culture : ActionFilterAttribute
    {
        public string Name { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var culture = Name;
            if (String.IsNullOrEmpty(culture))
                culture = GetSavedCultureOrDefault();

            // Set culture on current thread
            SetCultureOnThread(culture);

            // Proceed as usual
            base.OnActionExecuting(filterContext);
        }

        public static string GetSavedCultureOrDefault()
        {
            var ttsSetting = TtsController.LoadTtsSetting(System.Web.Hosting.HostingEnvironment.MapPath(TtsController.TtsConfigFile));
            return ttsSetting.Language;
        }

        private static void SetCultureOnThread(String language)
        {
            try
            {
                var cultureInfo = CultureInfo.CreateSpecificCulture(language);
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
            }
            catch 
            {
                // If any issue, fall back to English
                var cultureInfo = CultureInfo.CreateSpecificCulture("en-us");
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                ;
            }
        }
    }
}