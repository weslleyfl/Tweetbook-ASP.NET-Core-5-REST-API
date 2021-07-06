using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetbook.Cache;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Requests.Queries;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Domain;
using Tweetbook.Extensions;
using Tweetbook.Helpers;
using Tweetbook.Services;

namespace Tweetbook.Controllers.V1
{
    // [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PostsController : ControllerBase
    {

        private readonly IPostService _postService;
        private readonly IUriService _uriService;
        private readonly IMapper _mapper;

        public PostsController(IPostService postService, IUriService uriService, IMapper mapper)
        {
            _postService = postService;
            _uriService = uriService;
            _mapper = mapper;
        }


        [HttpGet(ApiRoutes.Posts.GetAll)]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [Cached(timeToLiveSeconds: 120)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllPostsQuery query, [FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);
            var filter = _mapper.Map<GetAllPostsFilter>(query);
            var posts = await _postService.GetPostsAsync(filter, pagination);
            var postsResponse = _mapper.Map<List<PostResponse>>(posts);

            if (pagination == null || (pagination.PageNumber < 1 || pagination.PageSize < 1))
            {
                return Ok(new PagedResponse<PostResponse>(postsResponse));
            }

            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, pagination, postsResponse);

            return Ok(paginationResponse);
        }


        [HttpGet(ApiRoutes.Posts.Get)]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(new Response<PostResponse>(_mapper.Map<PostResponse>(post)));
        }


        [HttpPost(ApiRoutes.Posts.Create)]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest postRequest)
        {
            var newPostId = Guid.NewGuid();
            var post = new Post()
            {
                Id = newPostId,
                Name = postRequest.Name,
                UserId = HttpContext.GetUserId(),
                Tags = postRequest.Tags.Select(x => new PostTag { PostId = newPostId, TagName = x }).ToList()
            };

            await _postService.CreatePostAsync(post);

            var locationUri = _uriService.GetPostUri(post.Id.ToString());
            return Created(locationUri, new Response<PostResponse>(_mapper.Map<PostResponse>(post)));

        }


        [HttpPut(ApiRoutes.Posts.Update)]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<IActionResult> Update([FromRoute] Guid postId, UpdatePostRequest request)
        {

            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());
            if (userOwnsPost == false)
            {
                return BadRequest(new ErrorResponse(new ErrorModel()
                {
                    Message = "Você não é proprietario deste Post."
                })
               );
            }

            var post = await _postService.GetPostByIdAsync(postId);
            if (post is null)
            {
                return NotFound();
            }

            post.Name = request.Name;

            var updated = await _postService.UpdatePostAsync(post);

            return (updated)
                ? Ok(new Response<PostResponse>(_mapper.Map<PostResponse>(post)))
                : NotFound();

        }


        [HttpDelete(ApiRoutes.Posts.Delete)]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());
            if (userOwnsPost == false)
            {
                return BadRequest(new ErrorResponse(new ErrorModel()
                {
                    Message = "Você não é proprietario deste Post."
                })
               );
            }

            var deleted = await _postService.DeletePostAsync(postId);

            if (deleted)
                return NoContent();

            return NotFound();
        }

    }
}
