using Microsoft.Extensions.Configuration;
using Nabaztag.Net.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nabaztag.Tts
{
    class Program
    {

        static List<Voice> Voices = new List<Voice>();
        const string FileName = @"tosay.mp3";
        static Nabaztag.Net.Nabaztag _nabaztag;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello Text to Speech!");

            var builder = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json", true, true)
            .AddJsonFile($"appsettings.development.json", true, true);
            var configuration = builder.Build();

            var key = configuration["CognitiveServices:Key"];
            var endpoint = configuration["CognitiveServices:EndPoint"];
            var region = endpoint.Substring(8, endpoint.IndexOf('.') - 8);
            var voiceShortName = configuration["CognitiveServices:PreferedVoiceShortName"];
            // Create an authentication object
            Authentication auth = new Authentication(endpoint, key);
            string host = $"https://{region}.tts.speech.microsoft.com/cognitiveservices/v1";
            string path = configuration["Path"];
            // Un comment if you want to save all voices available in your region
            //SaveAllVoices(region, auth);

            Voices = JsonConvert.DeserializeObject<List<Voice>>(GetAllVoices(region, auth));

            Console.Write("Type the text for Nabaztag to say: ");
            string input = Console.ReadLine();

            SaveTextToSpeechFile(host, auth, input, voiceShortName).Wait();
            // Ask the rabbit to say it

            if (args.Length > 0)
            {
                _nabaztag = new Nabaztag.Net.Nabaztag(args[0], 10543);
            }
            else
            {
                _nabaztag = new Nabaztag.Net.Nabaztag();
            }

            var signature = new Sequence() { AudioList = new string[] { "nabweatherd/signature.mp3" } };
            var body = new Sequence[] { new Sequence() { AudioList = new string[] { $"../../../../{path}/{FileName}" } } };
            var resp = _nabaztag.Message(signature, body, DateTime.MinValue);
            if (resp.Status == Status.Ok)
                Console.WriteLine("TTS played properly");
            else
                Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");
        }

        static async Task SaveTextToSpeechFile(string host, Authentication auth, string input, string voiceShortName)
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
                    Console.WriteLine($"Failed to obtain an access token: {ex}");
                    return;
                }

                // Find the voice details
                var voice = Voices.Where(m => m.ShortName == voiceShortName).FirstOrDefault();

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
                            using (FileStream fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.Write))
                            {
                                await dataStream.CopyToAsync(fileStream).ConfigureAwait(false);
                                fileStream.Close();
                            }
                        }
                    }
                }
            }
        }

        static string GetAllVoices(string region, Authentication auth)
        {
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
                        return res;
                    }
                }
            }
        }

        static void SaveAllVoices(string region, Authentication auth)
        {
            File.WriteAllText(@"voice.json", GetAllVoices(region, auth));
        }
    }
}
