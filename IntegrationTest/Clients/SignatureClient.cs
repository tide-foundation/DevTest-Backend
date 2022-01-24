using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IntegrationTest.Client
{
    class SignatureClient
    {
        private readonly string _url;

        public SignatureClient(string url) {
            _url = url;
        }
        
        public async Task<byte[]> Sign(uint keyId, byte[] message)
        {
            using var client = new HttpClient();

            var messageEncoded = EncodeBase64UrlSafe(message);
            var resp = await client.GetAsync(FullUrl($"/signature?keyId={keyId}&message={messageEncoded}"));
            if (!resp.IsSuccessStatusCode) return null;

            string result = resp.Content.ReadAsStringAsync().Result;
            return Convert.FromBase64String(result);
        }

        string FullUrl(string path) => $"{_url}{path}";

        private static string EncodeBase64UrlSafe(byte[] data) =>
            Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
