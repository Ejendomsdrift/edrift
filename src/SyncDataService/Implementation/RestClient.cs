using Infrastructure.Extensions;
using SyncDataService.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Constants;
using Infrastructure.Helpers;

namespace SyncDataService.Implementation
{
    public class RestClient
    {
        private const string ContentType = "application/json";
        private static readonly MediaTypeWithQualityHeaderValue JsonMediaType = new MediaTypeWithQualityHeaderValue(ContentType);
        private readonly string syncUrl;

        private readonly IRestClientConfiguration configuration;

        public RestClient(IRestClientConfiguration configuration, IAppSettingHelper appSettingHelper)
        {
            this.configuration = configuration;
            syncUrl = appSettingHelper.GetAppSetting<string>(Constants.AppSetting.SyncUrl);
        }

        public async Task Get(string query)
        {
            await ExecuteRequest(HttpMethod.Get, $"{syncUrl}{query}");
        }

        public async Task<T> Get<T>(string query)
        {
            var response = await ExecuteRequest(HttpMethod.Get, $"{syncUrl}{query}");
            return response.Deserialize<T>();
        }

        public async Task<TO> Post<TO, TI>(string query, Task<TI> task)
        {
            var result = await task;
            var response = await ExecuteRequest(HttpMethod.Post, syncUrl + query, new StringContent(result.ToJson(), Encoding.UTF8, ContentType));
            return response.Deserialize<TO>();
        }

        public async Task Post<TI>(string query, Task<TI> task)
        {
            var result = await task;
            await ExecuteRequest(HttpMethod.Post, syncUrl + query, new StringContent(result.ToJson(), Encoding.UTF8, ContentType));
        }

        public async Task Post(string query)
        {
            await ExecuteRequest(HttpMethod.Post, syncUrl + query);
        }

        public async Task Post<T>(string query, T result)
        {
            await ExecuteRequest(HttpMethod.Post, syncUrl + query, new StringContent(result.ToJson(), Encoding.UTF8, ContentType));
        }

        public async Task SendFile(string query, string fileName, byte[] fileContent)
        {
            using (var content = new MultipartFormDataContent($"Upload----{DateTime.UtcNow.Ticks}"))
            {
                content.Add(new ByteArrayContent(fileContent), "media", fileName);
                await ExecuteRequest(HttpMethod.Post, syncUrl + query, content);
            }
        }

        private async Task<string> ExecuteRequest(HttpMethod method, string command, HttpContent data = null)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(method, command))
            {
                request.Headers.Accept.Add(JsonMediaType);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", configuration.AccessToken);
                if (data != null)
                {
                    request.Content = data;
                }
                using (var response = await client.SendAsync(request))
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return content;
                    }
                    throw new Exception($"{(int)response.StatusCode} {response.StatusCode} - Error during web request.");
                }
            }
        }
    }
}