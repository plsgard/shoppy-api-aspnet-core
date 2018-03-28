using System;
using System.Threading.Tasks;
using Shoppy.Application.Commons;
using Shoppy.Application.Lists.Dtos;
using Shoppy.Core.Data;
using Shoppy.Core.Items;
using Shoppy.Core.Lists;

namespace Shoppy.Application.Lists
{
    public class ListAppService : AppService<List, ListDto, Guid, CreateListDto, UpdateListDto>, IListAppService
    {
        private readonly IItemRepository _itemRepository;

        public ListAppService(IRepository<List, Guid> repository, IItemRepository itemRepository) : base(repository)
        {
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
    }
}