using GroupService.Application.Dto;
using GroupService.Domain.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GroupService.Application.Mappings
{

    public class GroupMappingProfile : Profile
    {
        public GroupMappingProfile()
        {
            // Group
            CreateMap<Group, GroupDto>();
            CreateMap<CreateGroupDto, Group>();
            CreateMap<UpdateGroupDto, Group>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Zakładamy, że nie zmieniasz ID

                // Ignorujemy CreatedAt i CreatedByUserId w update
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore());

            // GroupMember
            CreateMap<GroupMember, GroupMemberDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<AddGroupMemberDto, GroupMember>();
            CreateMap<UpdateGroupMemberDto, GroupMember>()
                .ForMember(dest => dest.GroupId, opt => opt.Ignore()) // Ignorujemy GroupId – nie powinien być aktualizowany
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            // GroupListing
            CreateMap<GroupListing, GroupListingDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateGroupListingDto, GroupListing>();
            CreateMap<UpdateGroupListingDto, GroupListing>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status)) // enum jako enum
                .ForMember(dest => dest.Group, opt => opt.Ignore());

            // RoomApplication
            CreateMap<RoomApplication, RoomApplicationDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateRoomApplicationDto, RoomApplication>();
            CreateMap<UpdateRoomApplicationDto, RoomApplication>()
                .ForMember(dest => dest.ApplicantUserId, opt => opt.Ignore()) // Bez zmiany autora
                .ForMember(dest => dest.ListingId, opt => opt.Ignore());
        }
    }
}
