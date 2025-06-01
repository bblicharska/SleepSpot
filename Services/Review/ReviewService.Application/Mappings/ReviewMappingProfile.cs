using AutoMapper;
using ReviewService.Application.Dto;
using ReviewService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewService.Application.Mappings
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            // Review -> ReviewDto
            CreateMap<Review, ReviewDto>()
                // pomijamy mapowanie Reviewer i Reviewed, bo będą ustawiane ręcznie
                .ForMember(dest => dest.Reviewer, opt => opt.Ignore())
                .ForMember(dest => dest.Reviewed, opt => opt.Ignore());

            // ReviewCreateDto -> Review
            CreateMap<ReviewCreateDto, Review>()
                // Id i CreatedAt będą ustawiane w serwisie, więc ignorujemy
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // ReviewUpdateDto -> Review
            CreateMap<ReviewUpdateDto, Review>()
                .ForMember(dest => dest.ReviewerId, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewedId, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewedRole, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }

}
