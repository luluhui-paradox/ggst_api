using Microsoft.Net.Http.Headers;
using System.Net.Http;

namespace ggst_api.utils
{
    public class RatingUpdateHttpUtil:IRatingUpdateHttpUtil
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly HttpClient _httpClient;

        private readonly ILogger<RatingUpdateHttpUtil> _logger;

        private const string BASE_URL= "https://puddle.farm";

        public RatingUpdateHttpUtil(IHttpClientFactory httpClientFactory,ILogger<RatingUpdateHttpUtil> logger) { 
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _httpClient=_httpClientFactory.CreateClient();
        }

        public async Task<HttpResponseMessage> sendHttpAsync(string apiString) {
            if (!apiString.StartsWith('/'))
            {
                // Add '/' at the beginning
                apiString = '/' + apiString;
            }
            //make httpRequestMessage
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, BASE_URL+apiString)
            {
                Headers =
                {
                    { HeaderNames.Accept, "application/json" },
                    { HeaderNames.UserAgent, "Mozilla/5.0.html (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.html.2171.71 Safari/537.36" }
                }
            };
            //http Async
            var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);
            
            return httpResponseMessage;
        }
    }
}
