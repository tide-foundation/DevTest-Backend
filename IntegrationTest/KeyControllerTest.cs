using System;
using System.Threading.Tasks;
using IntegrationTest.Client;
using IntegrationTest.Tools;
using Xunit;

namespace IntegrationTest
{
    public class KeyControllerTest
    {
        private readonly string _url;
        private readonly string _user;
        private readonly KeyClient _repo;
        private readonly KeyModelFactory _modelFact;

        public KeyControllerTest() {
            _url = Environment.GetEnvironmentVariable("URL_BASE") ?? "http://localhost:8080";
            _user = Environment.GetEnvironmentVariable("USER_BASE") ?? "tide";
            _repo = new KeyClient(_url);
            _modelFact = new KeyModelFactory();
        }

        [Fact]
        public async Task TestInsert()
        {
            var key = _modelFact.Random(_user);
            
            var wasInserted = await _repo.Insert(key);
            Assert.True(wasInserted, "Key insertion was unsuccessful");

            var keyInserted = await _repo.Get(key.Id);

            Assert.NotNull(keyInserted);
            Assert.Equal(key.Id, keyInserted.Id);
            Assert.Equal(key.Key, keyInserted.Key);
        }

        [Fact]
        public async Task TestUpdate()
        {
            var key = _modelFact.Random(_user);
            var wasInserted = await _repo.Insert(key);

            Assert.True(wasInserted, "Key insertion was unsuccessful");

            var keyToUpdate = _modelFact.Random(_user);
            key.Key = keyToUpdate.Key;

            var wasUpdated = await _repo.Update(key);

            Assert.True(wasUpdated, "Key update was unsuccessful");

            var keyUpdated = await _repo.Get(key.Id);

            Assert.NotNull(keyUpdated);
            Assert.Equal(key.Id, keyUpdated.Id);
            Assert.Equal(key.Key, keyUpdated.Key);
        }

        [Fact]
        public async Task TestDelete()
        {
            var key = _modelFact.Random(_user);
            var wasInserted = await _repo.Insert(key);

            Assert.True(wasInserted, "Key insertion was unsuccessful");

            var wasDeleted = await _repo.Delete(key.Id);

            Assert.True(wasDeleted, "Key delete was unsuccessful");

            var keyDeleted = await _repo.Get(key.Id);

            Assert.Null(keyDeleted);
        }
    }
}
