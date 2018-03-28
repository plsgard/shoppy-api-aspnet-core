using AutoMapper;
using Shoppy.Core.Lists;
using Shoppy.Core.Shares;

namespace Shoppy.Application.Lists.Dtos
{
    public class ListProfile : Profile
    {
        public ListProfile()
        {
            CreateMap<List, ListDto>().ReverseMap();
            CreateMap<CreateListDto, List>();
            CreateMap<UpdateListDto, List>();
            CreateMap<ShareListDto, Share>();
        }
    }
}