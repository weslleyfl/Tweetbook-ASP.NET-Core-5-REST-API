using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Domain;

namespace Tweetbook.MappingProfiles
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Post, PostResponse>()
                .ForMember(destinationMember: postResponseDest => postResponseDest.Tags, memberOptions: opt =>
                   opt.MapFrom(postSrc => postSrc.Tags.Select(x => new TagResponse { Name = x.TagName })));

            CreateMap<Tag, TagResponse>();
        }
    }
}
