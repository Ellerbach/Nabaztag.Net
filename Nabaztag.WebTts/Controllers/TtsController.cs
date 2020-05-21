using Nabaztag.Net.Models;
using Nabaztag.WebTts.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Nabaztag.WebTts.Controllers
{
    [Culture]
    public class TtsController : Controller
    {

        public const string TtsConfigFile = "~/ttsconfig.json";
        public const string TtsFileName = "ttsfilename.mp3";

        // GET: Tts
        
        public ActionResult Index()
        {
            return View();
        }

        public static (bool status, string error) Recognize(string ttsText, string ttsConfigPath, string ttsFilePath)
        {
            try
            {
                var ttsSetting = LoadTtsSetting(ttsConfigPath);
                Authentication auth = new Authentication(ttsSetting.EndPoint, ttsSetting.Key);
                var region = ttsSetting.EndPoint.Substring(8, ttsSetting.EndPoint.IndexOf('.') - 8);
                string host = $"https://{region}.tts.speech.microsoft.com/cognitiveservices/v1";
                SaveTextToSpeechFile(host, auth, ttsText, ttsSetting.PrefferedVoice, ttsFilePath).Wait();

                var nabaztag = new Nabaztag.Net.Nabaztag(ttsSetting.NabaztagAddress, ttsSetting.NabaztagPort);

                var signature = new Sequence() { AudioList = new string[] { "nabweatherd/signature.mp3" } };
                var body = new Sequence[] { new Sequence() { AudioList = new string[] { $"../../../../{ttsSetting.ApplicationPath}/{TtsFileName}" } } };
                var resp = nabaztag.Message(signature, body, DateTime.MinValue);
                return (resp.Status == Status.Ok, "");
            }
            catch (Exception ex)
            {
                return (false, $"Exception: {ex}");
            }

        }

        [HttpPost, ActionName("SaySomething")]
        public ActionResult SaySomething(string ttsText)
        {
            ViewBag.Tts = ttsText;
            try
            {
                var (status, resp) = Recognize(ttsText, Server.MapPath(TtsConfigFile), Server.MapPath("~/" + TtsFileName));
                if (status)
                    ViewBag.LastMessage = $"TTS played properly: {ttsText}";
                else
                    ViewBag.LastMessage = resp;
            }
            catch (Exception ex)
            {
                ViewBag.LastMessage = $"Exception: {ex}";
            }
            return View(nameof(Index));
        }

        private void PrepareViewBag(TtsSetting ttsSetting)
        {
            try
            {
                // The langauges
                ViewBag.Languages = new SelectList(CultureInfo.GetCultures(CultureTypes.NeutralCultures), nameof(CultureInfo.Name), nameof(CultureInfo.DisplayName));
                var voices = GetAllVoices(ttsSetting);
                // The voices
                ViewBag.Voices = new SelectList(voices, nameof(Voice.ShortName), nameof(Voice.Name));                
            }
            catch
            {
                // Do nothing, bag will be empty
            }

        }

        private static async Task SaveTextToSpeechFile(string host, Authentication auth, string input, Voice voice, string path)
        {
            using (HttpClient client = new HttpClient())
            {
                string accessToken;
                try
                {
                    accessToken = auth.GetAccessToken();
                }
                catch (Exception ex)
                {
                    return;
                }

                // Create SSML document.
                XDocument body = new XDocument(
                        new XElement("speak",
                            new XAttribute("version", "1.0"),
                            new XAttribute(XNamespace.Xml + "lang", $"{voice.Locale}"),
                            new XElement("voice",
                                new XAttribute(XNamespace.Xml + "lang", $"{voice.Locale}"),
                                new XAttribute(XNamespace.Xml + "gender", $"{voice.Gender}"),
                                new XAttribute("name", $"{voice.ShortName}"),
                                input)));

                using (HttpRequestMessage request = new HttpRequestMessage())
                {
                    // Set the HTTP method
                    request.Method = HttpMethod.Post;
                    // Construct the URI
                    request.RequestUri = new Uri(host);
                    // Set the content type header
                    request.Content = new StringContent(body.ToString(), Encoding.UTF8, "application/ssml+xml");
                    // Set additional header, such as Authorization and User-Agent
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    request.Headers.Add("User-Agent", "Nabaztag");
                    request.Headers.Add("Connection", "Keep-Alive");
                    // Audio output format. See API reference for full list.
                    request.Headers.Add("X-Microsoft-OutputFormat", "audio-16khz-32kbitrate-mono-mp3");
                    using (HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();

                        // Asynchronously read the response
                        using (Stream dataStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
                            {
                                await dataStream.CopyToAsync(fileStream).ConfigureAwait(false);
                                fileStream.Close();
                            }
                        }
                    }
                }
            }
        }

        // GET: Tts/Edit
        public ActionResult Edit()
        {
            TtsSetting ttsSetting = new TtsSetting();
            if (System.IO.File.Exists(Server.MapPath(TtsConfigFile)))
            {
                ttsSetting = LoadTtsSetting(Server.MapPath(TtsConfigFile));
                PrepareViewBag(ttsSetting);

            }
            return View(ttsSetting);
        }

        public static TtsSetting LoadTtsSetting(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                var ttsSettingJson = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<TtsSetting>(ttsSettingJson);
            }
        }

        private void SaveTtsSetting(TtsSetting ttsSetting)
        {
            var ser = JsonConvert.SerializeObject(ttsSetting);
            using (StreamWriter sw = new StreamWriter(Server.MapPath(TtsConfigFile)))
            {
                sw.Write(ser);
            }
        }

        // POST: Tts/Edit
        [HttpPost]
        public ActionResult Edit(TtsSetting ttsSetting)
        {
            try
            {
                if (ttsSetting != null)
                {
                    SaveTtsSetting(ttsSetting);
                }

                // Check if all is valid                
                try
                {
                    Authentication auth = new Authentication(ttsSetting.EndPoint, ttsSetting.Key);
                    var accessToken = auth.GetAccessToken();
                }
                catch (Exception ex)
                {
                    ViewBag.LastMessage = "Error validating your keys";
                    PrepareViewBag(ttsSetting);
                    return View();
                }

                // Check if we have a preferred selection, if not, then return to the view
                if (string.IsNullOrEmpty(ttsSetting.PrefferedShortName))
                {
                    ViewBag.LastMessage = "Please select a preferred voice";
                    PrepareViewBag(ttsSetting);
                    return View();
                }
                else
                {
                    // Find and save the voice details
                    ttsSetting.PrefferedVoice = GetAllVoices(ttsSetting).Where(m => m.ShortName == ttsSetting.PrefferedShortName).FirstOrDefault();
                    SaveTtsSetting(ttsSetting);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.LastMessage = $"An error occured, please try again: {ex}";
                PrepareViewBag(ttsSetting);
                return View();
            }
        }

        private List<Voice> GetAllVoices(TtsSetting ttsSetting)
        {
            var region = ttsSetting.EndPoint.Substring(8, ttsSetting.EndPoint.IndexOf('.') - 8);
            Authentication auth = new Authentication(ttsSetting.EndPoint, ttsSetting.Key);
            using (HttpClient client = new HttpClient())
            {
                var req = $"https://{region}.tts.speech.microsoft.com/cognitiveservices/voices/list";
                using (HttpRequestMessage request = new HttpRequestMessage())
                {
                    string accessToken;
                    try
                    {
                        accessToken = auth.GetAccessToken();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to obtain an access token: {ex}");
                        return null;
                    }

                    request.Method = HttpMethod.Get;
                    request.RequestUri = new Uri(req);
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    using (HttpResponseMessage response = client.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult())
                    {
                        response.EnsureSuccessStatusCode();
                        var res = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        return JsonConvert.DeserializeObject<List<Voice>>(res);
                    }
                }
            }
        }
    }
}