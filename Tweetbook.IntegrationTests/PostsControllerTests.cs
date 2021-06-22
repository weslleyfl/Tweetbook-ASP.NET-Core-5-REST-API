using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tweetbook.Contracts.V1;
using FluentAssertions;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Domain;
using Xunit;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace Tweetbook.IntegrationTests
{
    public class PostsControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetAll_WithoutAnyPosts_ReturnsEmptyResponse()
        {
            // Arragne
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Posts.GetAll);


            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            // (await response.Content.ReadAsAsync<PagedResponse<PostResponse>>()).Data.Should().NotBeEmpty();
            var objectReturn = await response.Content.ReadAsAsync<PagedResponse<PostResponse>>();
            objectReturn.Data.Should().BeEmpty("O objecto PagedResponse<PostResponse> tem que retornar vazio. ");

        }

        [Fact]
        public async Task Get_ReturnsPost_WhenPostExistsInTheDatabase()
        {
            // Arrange
            await AuthenticateAsync();
            var createdPost = await CreatePostAsync(new CreatePostRequest
            {
                Name = "Test post",
                Tags = new[] { "testtag" }
            });

            // Act
            var response = await TestClient.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}", createdPost.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedPost = await response.Content.ReadAsAsync<Response<PostResponse>>();
            returnedPost.Data.Id.Should().Be(createdPost.Id);
            returnedPost.Data.Name.Should().Be("Test post");
            returnedPost.Data.Tags.Single().Name.Should().Be("testtag");


        }
    }
}
