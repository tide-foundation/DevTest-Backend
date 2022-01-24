using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntegrationTest.Client
{
    class KeyClient
    {
        private readonly string _url;

        public KeyClient(string url) {
            _url = url;
        }
        
        public async Task<List<KeyModel>> Get()
        {
            using var client = new HttpClient();

            var resp = await client.GetAsync(FullUrl("/Key"));
            if (!resp.IsSuccessStatusCode) return null;

            string result = resp.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<List<KeyModel>>(result, 
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true,
                });
        }

        public async Task<KeyModel> Get(uint id) {
            using var client = new HttpClient();

            var resp = await client.GetAsync(FullUrl($"/Key/{id}"));
            if (!resp.IsSuccessStatusCode) return null;

            string result = resp.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<KeyModel>(result, 
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true,
                });
        }

        public async Task<bool> Insert(KeyModel model) {
            var json = JsonSerializer.Serialize(model);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();

            var resp = await client.PostAsync(FullUrl($"/Key"), data);
            if (!resp.IsSuccessStatusCode) return false;

            string result = resp.Content.ReadAsStringAsync().Result;
            var newModel = JsonSerializer.Deserialize<KeyModel>(result, 
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true,
                });

            model.Id = newModel.Id;
            return true;
        }

        public async Task<bool> Update(KeyModel model) {
            var json = JsonSerializer.Serialize(model);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();

            var resp = await client.PutAsync(FullUrl($"/Key"), data);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(uint id)         {
            using var client = new HttpClient();

            var resp = await client.DeleteAsync(FullUrl($"/Key/{id}"));
            return resp.IsSuccessStatusCode;
        }

        private string FullUrl(string path) => $"{_url}{path}";
    }
}
