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
    public class RecipeProfile : Profile
    {
        public RecipeProfile()
        {
            CreateMap<RecipeDto, Recipe>().ReverseMap();

            CreateMap<RecipeDto, RecipeEntity>().ReverseMap();
        }
    }
}
