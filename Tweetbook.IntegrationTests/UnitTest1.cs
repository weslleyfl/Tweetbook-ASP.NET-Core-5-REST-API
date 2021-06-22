using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tweetbook.Contracts.V1;
using Xunit;

namespace Tweetbook.IntegrationTests
{
    public class UnitTest1
    {

        private readonly HttpClient _client;

        public UnitTest1()
        {
            var appFactory = new WebApplicationFactory<Startup>();
            _client = appFactory.CreateClient();
        }

        [Fact]
        public async Task Test1()
        {
            // Arragne
            // Act
            // Assert
            var resposne = await _client.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}", "1"));
        }
    }
}
