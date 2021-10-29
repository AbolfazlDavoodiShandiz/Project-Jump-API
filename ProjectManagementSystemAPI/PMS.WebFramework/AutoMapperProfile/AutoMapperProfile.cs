using AutoMapper;
using PMS.DTO;
using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.WebFramework.AutoMapperProfile
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Project, ProjectDTO>().ReverseMap();
            CreateMap<Project, ProjectRegistrationDTO>().ReverseMap();
            CreateMap<ProjectTask, ProjectTaskDTO>().ReverseMap();
            CreateMap<ProjectTask, ProjectTaskRegistrationDTO>().ReverseMap();
            CreateMap<UserTask, UserTaskDTO>().ReverseMap();

            CreateMap<User, UserSearchResponseDTO>()
                .ForMember(d => d.UserEmail, opt =>
                {
                    opt.MapFrom(s => s.Email);
                })
                .ForMember(d => d.UserPhoneNumber, opt =>
                {
                    opt.MapFrom(s => s.PhoneNumber);
                })
                .ForMember(d => d.UserId, opt =>
                {
                    opt.MapFrom(s => s.Id);
                }).ReverseMap();

            CreateMap<User, ProjectMemberDTO>()
                .ForMember(pmd => pmd.UserId, opt =>
                {
                    opt.MapFrom(pms => pms.Id);
                })
                .ForMember(pmd => pmd.Mobile, opt =>
                {
                    opt.MapFrom(pms => pms.PhoneNumber);
                }).ReverseMap();
        }
    }
}
