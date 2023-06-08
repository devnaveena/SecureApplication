using AutoMapper;
using Entities.Dtos.RequestDto;
using Entities.Dtos.ResponseDto;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public class Mappers : Profile
    {
        public Mappers()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<BookDto, Book>().ReverseMap();
            CreateMap<AdminDto, Book>().ReverseMap();
            CreateMap<ReaderDto, Book>().ReverseMap();
        }


    }
}