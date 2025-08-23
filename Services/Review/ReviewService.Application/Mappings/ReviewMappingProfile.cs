using AutoMapper;
using ReviewService.Application.Dto;
using ReviewService.Domain.Models;
using Shared.Dto;
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
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.Reviewer, opt => opt.Ignore());

            CreateMap<ReviewCreateDto, Review>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<ReviewUpdateDto, Review>()
                .ForMember(dest => dest.ReviewerId, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyId, opt => opt.Ignore())
                .ForMember(dest => dest.RoomId, opt => opt.Ignore()) // Dodane jeśli nie chcesz aktualizować tego pola
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }

}
