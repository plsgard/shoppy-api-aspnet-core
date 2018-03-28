using System;
using System.Linq;
using System.Threading.Tasks;
using Shoppy.Application.Commons;
using Shoppy.Application.Items.Dtos;
using Shoppy.Core.Items;
using Shoppy.Core.Lists;

namespace Shoppy.Application.Items
{
    public class ItemAppService : AppService<Item, ItemDto, Guid, CreateItemDto, UpdateItemDto, GetAllItemsDto>, IItemAppService
    {
        private readonly IItemRepository _repository;
        private readonly IListRepository _listRepository;

        public ItemAppService(IItemRepository repository, IListRepository listRepository) : base(repository)
        {
            _repository = repository;
            _listRepository = listRepository;
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
            var maxIndex = await _repository.GetMaxIndexForList(input.ListId);
            entity.Index = maxIndex > 0 ? maxIndex + 10 : 0;

            return ToDto(await Repository.AddAsync(entity));
        }

        protected override void Validate(object input)
        {
            if (input != null && input is CreateItemDto create)
                if (!_listRepository.GetAllIncludingShares().Any(i => i.Id == create.ListId))
                    throw new ArgumentException("The list on which you want to create item doest not exists or is not reachable.", nameof(create.ListId));
            base.Validate(input);
        }
    }
}