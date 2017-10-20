using STPresenceControl.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace STPresenceControl.Common
{
    public class Http
    {
        private static HttpClient _httpClient = new HttpClient();

        public Http()
        {

            _httpClient = new HttpClient(
            new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

            //_httpClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
            _httpClient.DefaultRequestHeaders.Add("Origin", "https://saas.hrzucchetti.it");
            //_httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");//text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8
            _httpClient.DefaultRequestHeaders.Add("Referer", "https://saas.hrzucchetti.it/hrpsolmicro/jsp/login.jsp");
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "es-ES, es;q=0.8, en;q=0.6");
            _httpClient.DefaultRequestHeaders.Add("x-dtreferer", "https://saas.hrzucchetti.it/hrpsolmicro/jsp/home.jsp");
            _httpClient.DefaultRequestHeaders.Add("x-STInfinity", "Test");
        }

        public async Task<string> GetAsync(Uri uri, NameValueCollection parameters = null)
        {
            UriBuilder uriBuilder = new UriBuilder(uri);

            if (parameters != null && parameters.Count > 0)
            {
                if (uriBuilder.Query != null && uriBuilder.Query.Length > 1)
                    uriBuilder.Query = uriBuilder.Query.Substring(1) + "&" + parameters.GenerateQueryString();
                else
                    uriBuilder.Query = parameters.GenerateQueryString();
            }
            return await _httpClient.GetStringAsync(uriBuilder.Uri);
        }
        public async Task<string> PostAsync(Uri uri, KeyValuePair<string, string>[] parameters)
        {
            var response = await _httpClient.PostAsync(uri, new FormUrlEncodedContent(parameters));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PostAsync(Uri uri, StringContent content)
        {
            var response = await _httpClient.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
