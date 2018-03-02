using AutoMapper;
using Shoppy.Core.Items;

namespace Shoppy.Application.Items.Dtos
{
    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            CreateMap<Item, ItemDto>().ReverseMap();
            CreateMap<CreateItemDto, Item>();
        }
    }
}
