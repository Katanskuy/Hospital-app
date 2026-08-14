using AutoMapper;
using BLL.DTO;
using DAL.Entities;
using HospitalDataBase.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalDataBase.MapProfiles
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            CreateMap<DoctorDto, Doctor>().ReverseMap();

            CreateMap<DoctorDto, DoctorEntity>().ReverseMap();

        }
    }
}
