using System;
using System.Text;
using System.Threading.Tasks;
using IntegrationTest.Client;
using IntegrationTest.Tools;
using Xunit;

namespace IntegrationTest
{
    public class SignatureControllerTest
    {
        private readonly string _url;
        private readonly KeyClient _keyRepo;
        private readonly SignatureClient _signRepo;
        private readonly Openssl _openssl;

        public SignatureControllerTest() {
            _url = Environment.GetEnvironmentVariable("URL") ?? "http://localhost:8080";
            _keyRepo = new KeyClient(_url);
            _signRepo = new SignatureClient(_url);
            _openssl = new Openssl();
        }

        [Fact]
        public async Task TestSignature()
        {
            var keyPair = _openssl.GenKey();
            var keyModel = new KeyModel { Key = keyPair.PrvKey };
            var message = Encoding.UTF8.GetBytes($"Random message {Guid.NewGuid()}");

            await _keyRepo.Insert(keyModel);
            var signature = await _signRepo.Sign(keyModel.Id, message);
            Assert.True(signature is not null, "The signature endpoint was not successful");
            
            var sw = _openssl.Verify(message, signature, keyPair.PubKey);
            Assert.True(sw, "The signature was not valid");
        }
    }
}
