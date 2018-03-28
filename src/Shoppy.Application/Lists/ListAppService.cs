using System;
using System.Linq;
using System.Threading.Tasks;
using Shoppy.Application.Commons;
using Shoppy.Application.Lists.Dtos;
using Shoppy.Core.Items;
using Shoppy.Core.Lists;
using Shoppy.Core.Shares;

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

            Normalize(input);
            Validate(input);

            var existingId = input.ExistingListId;

            var listDto = await Create(input);
            var newId = listDto.Id;

            await _itemRepository.DuplicateOnList(existingId, newId);

            return listDto;
        }

        public async Task Share(ShareListDto input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            Normalize(input);
            Validate(input);

            if (!await Repository.AnyAsync(l => l.Id == input.ListId))
                throw new ArgumentException("Unable to share. The provided list does not exists or is not reachable.", nameof(input.ListId));

            var share = ObjectMapper.Map<Share>(input);
            await _repository.AddShareAsync(share);
        }

        public override Task<ListDto> Get(Guid id)
        {
            return Task.FromResult(ToDto(_repository.GetAllIncludingShares().SingleOrDefault(l => l.Id == id)));
        }

        protected override void Validate(object input)
        {
            base.Validate(input);

            if (input != null && input is DuplicateListDto duplicate)
                if (!Repository.Any(l => l.Id == duplicate.ExistingListId))
                    throw new ArgumentException($"Unable to duplicate list with id '{duplicate.ExistingListId}' because it does not exists.", nameof(duplicate.ExistingListId));
        }

        protected override IQueryable<List> CreateFilteredQuery(GetAllListsDto input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return input.LoadShares ? _repository.GetAllIncludingShares() : base.CreateFilteredQuery(input);
        }
    }
}