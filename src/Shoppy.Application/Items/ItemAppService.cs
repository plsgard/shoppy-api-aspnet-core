using System;
using System.Linq;
using System.Threading.Tasks;
using Shoppy.Application.Commons;
using Shoppy.Application.Items.Dtos;
using Shoppy.Core.Data;
using Shoppy.Core.Items;

namespace Shoppy.Application.Items
{
    public class ItemAppService : AppService<Item, ItemDto, Guid, CreateItemDto, UpdateItemDto, GetAllItemsDto>, IItemAppService
    {
        public ItemAppService(IRepository<Item, Guid> repository) : base(repository)
        {
        }

        protected override IQueryable<Item> CreateFilteredQuery(GetAllItemsDto input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            var filteredQuery = base.CreateFilteredQuery(input);
            return input.ListId.HasValue ? filteredQuery.Where(i => i.ListId == input.ListId.Value) : filteredQuery;
        }

        public override async Task<ItemDto> Create(CreateItemDto input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            Normalize(input);
            Validate(input);

            var entity = ToEntity(input);
            entity.Index = Repository.GetAll().Any() ? Repository.GetAll().Max(i => i.Index) + 10 : 0;

            return ToDto(await Repository.AddAsync(entity));
        }
    }
}