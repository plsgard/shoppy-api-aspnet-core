using System;
using System.Linq;
using System.Threading.Tasks;
using Shoppy.Application.Commons;
using Shoppy.Application.Lists.Dtos;
using Shoppy.Core.Items;
using Shoppy.Core.Lists;

namespace Shoppy.Application.Lists
{
    public class ListAppService : AppService<List, ListDto, Guid, CreateListDto, UpdateListDto, GetAllListsDto>, IListAppService
    {
        private readonly IListRepository _repository;
        private readonly IItemRepository _itemRepository;

        public ListAppService(IListRepository repository, IItemRepository itemRepository) : base(repository)
        {
            _repository = repository;
            _itemRepository = itemRepository;
        }

        public async Task<ListDto> Duplicate(DuplicateListDto input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            
            Validate(input);
            if(!(await Repository.AnyAsync(l=>l.Id == input.ExistingListId)))
                throw new ArgumentException($"Unable to duplicate list with id '{input.ExistingListId}' because it does not exists.", nameof(input.ExistingListId));
            Normalize(input);

            var existingId = input.ExistingListId;

            var listDto = await Create(input);
            var newId = listDto.Id;

            await _itemRepository.DuplicateOnList(existingId, newId);

            return listDto;
        }

        protected override IQueryable<List> CreateFilteredQuery(GetAllListsDto input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return input.LoadShares ? base.CreateFilteredQuery(input) : _repository.GetAllIncludingShares();
        }
    }
}