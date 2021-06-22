using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Domain;
using Tweetbook.Extensions;
using Tweetbook.Services;

namespace Tweetbook.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class TagsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;

        public TagsController(IPostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }


        /// <summary>
        /// Retornar todas as Tags cadastradas no sistema
        /// </summary>
        /// <response code="200">Retornar todas as Tags cadastradas no sistema</response>
        [HttpGet(ApiRoutes.Tags.GetAll)]
        //[Authorize(Roles = "Poster")]
        [Authorize(Policy = "TagViewer")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _postService.GetAllTagsAsync();
            return Ok(_mapper.Map<List<TagResponse>>(tags));
        }

        /// <summary>
        /// Retorna uma Tag por nome da Tag
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        [HttpGet(ApiRoutes.Tags.Get)]
        public async Task<IActionResult> Get([FromRoute] string tagName)
        {
            var tag = await _postService.GetTagByNameAsync(tagName);
            if (tag is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<TagResponse>(tag));

        }


        /// <summary>
        /// Cria uma nova Tag no sistema
        /// </summary>
        /// <response code="201">Creates a tag in the system</response>
        /// <response code="400">Unable to create the tag due to validation error</response>
        [HttpPost(ApiRoutes.Tags.Create)]
        [ProducesResponseType(typeof(TagResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTagRequest request)
        {
            var newTag = new Tag
            {
                Name = request.TagName,
                CreatorId = HttpContext.GetUserId(),
                CreatedOn = DateTime.UtcNow
            };

            var created = await _postService.CreateTagAsync(newTag);
            if (created == false)
            {
                return BadRequest(new ErrorResponse(new ErrorModel { Message = "Incapaz de criar Tag" }));
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = $"{baseUrl}/{ApiRoutes.Tags.Get.Replace("{tagName}", newTag.Name)}";

            return Created(locationUri, _mapper.Map<TagResponse>(newTag));

        }

        /// <summary>
        /// Deletar uma tag no sistema
        /// </summary>
        /// <param name="tagName"></param>
        /// <response code="204">Tag deletado com sucesso</response>
        /// <response code="404">Tag não deletada, pois não foi encontrada ou gerou erro </response>
        [HttpDelete(ApiRoutes.Tags.Delete)]
        [Authorize(Policy = "MustWorkForChapsas")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<IActionResult> Delete([FromRoute] string tagName)
        {
            var deleted = await _postService.DeleteTagAsync(tagName);

            if (deleted)
                return NoContent();

            return NotFound();
        }

    }
}
