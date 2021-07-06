using System;
using System.Threading.Tasks;
using Refit;
using Tweetbook.Contracts.V1.Requests;

namespace Tweetbook.Sdk.Sample
{
    public static class Program
    {
        /// <summary>
        /// https://renatogroffe.medium.com/asp-net-core-httpclientfactory-refit-simplificando-o-consumo-de-apis-rest-em-web-apps-800220dd1b5d
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {


            var cachedToken = string.Empty;

            // O Refit é uma biblioteca que permite representar uma REST API através de uma interface.
            // Esta configuraçao pode ser feita na classe startup
            var identityApi = RestService.For<IIdentityApi>("https://localhost:5001");
            var tweetbookApi = RestService.For<ITweetbookApi>("https://localhost:5001", new RefitSettings
            {
                AuthorizationHeaderValueGetter = () => Task.FromResult(cachedToken)
            });


            //var registerResponse = await identityApi.RegisterAsync(new UserRegistrationRequest
            //{
            //    Email = "sdkaccount@gmail.com",
            //    Password = "Test123!"
            //});

            var loginResponse = await identityApi.LoginAsync(new UserLoginRequest
            {
                Email = "sdkaccount@gmail.com",
                Password = "Test123!"
            });

            cachedToken = loginResponse.Content.Token;

            var allPosts = await tweetbookApi.GetAllAsync();

            var createdPost = await tweetbookApi.CreateAsync(new CreatePostRequest
            {
                Name = "This is created by the SDK",
                Tags = new[] { "sdk" }
            });

            var retrievedPost = await tweetbookApi.GetAsync(createdPost.Content.Data.Id);

            var updatedPost = await tweetbookApi.UpdateAsync(createdPost.Content.Data.Id, new UpdatePostRequest
            {
                Name = "This is updated by the SDK"
            });

            var deletePost = await tweetbookApi.DeleteAsync(createdPost.Content.Data.Id);

        }
    }
}
