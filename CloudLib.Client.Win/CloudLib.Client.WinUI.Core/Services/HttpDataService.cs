using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CloudLib.Client.WinUI.Core.Helpers;

namespace CloudLib.Client.WinUI.Core.Services
{
    // This class provides a wrapper around common functionality for HTTP actions.
    // Learn more at https://docs.microsoft.com/windows/uwp/networking/httpclient
    public class HttpDataService
    {
        private readonly Dictionary<string, object?> _responseCache;
        private HttpClient _client;

        public HttpDataService(string defaultBaseUrl = "")
        {
            _client = new HttpClient();

            if (!string.IsNullOrEmpty(defaultBaseUrl))
            {
                _client.BaseAddress = new Uri($"{defaultBaseUrl}/");
            }

            _responseCache = new Dictionary<string, object?>();
        }

        public async Task<T?> GetAsync<T>(string uri, string? accessToken = null, bool forceRefresh = false)
        {
            T? result = default;

            // The responseCache is a simple store of past responses to avoid unnecessary requests for the same resource.
            // Feel free to remove it or extend this request logic as appropraite for your app.
            if (forceRefresh || !_responseCache.ContainsKey(uri))
            {
                AddAuthorizationHeader(accessToken);
                var jsonStream = await _client.GetStreamAsync(uri);
                result = await Task.Run(() => Json.ToObjectAsync<T>(jsonStream));

                if (_responseCache.ContainsKey(uri))
                {
                    _responseCache[uri] = result;
                }
                else
                {
                    _responseCache.Add(uri, result);
                }
            }
            else
            {
                result = (T)_responseCache[uri];
            }

            return result;
        }

        public async Task<bool> PostAsync<T>(string uri, T item)
        {
            if (item == null)
            {
                return false;
            }

            var serializedStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(serializedStream, item);

            var response = await _client.PostAsync(uri, new StreamContent(serializedStream));

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PostAsJsonAsync<T>(string uri, T item)
        {
            if (item == null)
            {
                return false;
            }

            var serializedStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(serializedStream, item);

            var response = await _client.PostAsync(uri, new StringContent(serializedStream.ToString(), Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutAsync<T>(string uri, T item)
        {
            if (item == null)
            {
                return false;
            }

            var serializedStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(serializedStream, item);

            var response = await _client.PutAsync(uri, new StreamContent(serializedStream));

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutAsJsonAsync<T>(string uri, T item)
        {
            if (item == null)
            {
                return false;
            }

            var serializedStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(serializedStream, item);

            var response = await _client.PutAsync(uri, new StringContent(serializedStream.ToString(), Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string uri)
        {
            var response = await _client.DeleteAsync(uri);

            return response.IsSuccessStatusCode;
        }

        // Add this to all public methods
        private void AddAuthorizationHeader(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = null;
                return;
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
