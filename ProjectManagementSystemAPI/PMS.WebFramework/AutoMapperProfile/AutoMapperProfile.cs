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
        }
    }
}
