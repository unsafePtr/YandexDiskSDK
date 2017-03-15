using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YandexDiskSDK.Exceptions;
using YandexDiskSDK.ResponseModels;

namespace YandexDiskSDK
{
    public class YandexOAuth
    {
        private HttpClient Client { get; set; } = new HttpClient();
        public string OAuthToken { get; private set; }
        public string TokenType { get; private set; }
        public TimeSpan Duration { get; private set; }

        public YandexOAuth() { }
        
        public async Task<string> GetTokenAsync(string confirmationCode, string clientId, string clientSecret)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "code", confirmationCode },
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };

            FormUrlEncodedContent content = new FormUrlEncodedContent(parameters);
            string api = "https://oauth.yandex.ru/token";
            HttpResponseMessage response = await Client.PostAsync(api, content);
            
            string jsonString = await response.Content.ReadAsStringAsync();
            JToken json = JObject.Parse(jsonString);

            JToken error = json.SelectToken("error");
            if(error != null)
            {
                ErrorResponse errorResponse = new ErrorResponse();
                errorResponse.Error = error.ToString();
                errorResponse.Description = json.SelectToken("error_description").ToString();

                throw new HttpDiskException(errorResponse);
            }
            else
            {
                this.OAuthToken = json.SelectToken("access_token").ToString();
                this.TokenType = json.SelectToken("token_type").ToString();
                this.Duration = TimeSpan.FromSeconds((int)json.SelectToken("expires_in"));
            }

            return this.OAuthToken;
        }
    }
}
