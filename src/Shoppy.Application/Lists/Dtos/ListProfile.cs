using AutoMapper;
using Shoppy.Core.Lists;

namespace Shoppy.Application.Lists.Dtos
{
    public class ListProfile : Profile
    {
        public ListProfile()
        {
            CreateMap<List, ListDto>().ReverseMap();
            CreateMap<CreateListDto, List>();
            CreateMap<UpdateListDto, List>();
        }
    }
}