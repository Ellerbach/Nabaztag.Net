using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nabaztag.Tts
{
    public class Authentication
    {
        private string subscriptionKey;
        private string tokenFetchUri;
        private Timer accessTokenRenewer;
        private string accessToken;
        private object objLock = new object();

        //Access token expires every 10 minutes. Renew it every 9 minutes only.
        private const int RefreshTokenDuration = 1;

        public Authentication(string tokenFetchUri, string subscriptionKey)
        {
            if (string.IsNullOrWhiteSpace(tokenFetchUri))
            {
                throw new ArgumentNullException(nameof(tokenFetchUri));
            }

            if (string.IsNullOrWhiteSpace(subscriptionKey))
            {
                throw new ArgumentNullException(nameof(subscriptionKey));
            }

            this.tokenFetchUri = tokenFetchUri;
            this.subscriptionKey = subscriptionKey;

            accessToken = FetchTokenAsync().Result;

            //renew the token every specfied minutes
            accessTokenRenewer = new Timer(new TimerCallback(OnTokenExpiredCallback),
                                           this,
                                           TimeSpan.FromMinutes(RefreshTokenDuration),
                                           TimeSpan.FromMilliseconds(-1));
        }

        public string GetAccessToken()
        {
            lock (objLock)
            {
                return accessToken;
            }
        }

        private void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                lock (objLock)
                {
                    accessToken = FetchTokenAsync().Result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));
                }

            }
        }

        public async Task<string> FetchTokenAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                UriBuilder uriBuilder = new UriBuilder(tokenFetchUri);

                HttpResponseMessage result = await client.PostAsync(uriBuilder.Uri.AbsoluteUri, null).ConfigureAwait(false);
                result.EnsureSuccessStatusCode();
                return await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }
    }

}
