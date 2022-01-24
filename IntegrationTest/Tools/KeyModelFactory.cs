using System;
using IntegrationTest.Client;

namespace IntegrationTest.Tools
{
    public class KeyModelFactory {
        private readonly Openssl _openssl;

        public KeyModelFactory() {
            _openssl = new Openssl();
        }

        public KeyModel Random() {
            var keyPair = _openssl.GenKey();
            return new KeyModel {
                Key = keyPair.PubKey
            };
        }
    }
}
