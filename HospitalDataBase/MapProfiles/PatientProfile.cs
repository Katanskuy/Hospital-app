using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;
using DAL.Entities;
using HospitalDataBase.Objects;

namespace HospitalDataBase.MapProfiles
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<PatientDto, Patient>().ReverseMap();

            CreateMap<PatientDto, PatientEntity>().ReverseMap();
        }
    }
}
