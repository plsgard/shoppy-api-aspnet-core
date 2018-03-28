using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shoppy.Data.Repositories;
using Xunit;

namespace Shoppy.Tests.Data
{
    public class ListRepositoryTests : AppTestBase
    {
        private readonly ListRepository _listRepository;

        public ListRepositoryTests()
        {
            _listRepository = new ListRepository(Context);
        }

        [Fact]
        public async Task GetAllIncludingShares_OneShare_OneMine()
        {
            var userId1 = (await CreateUser()).Id;
            var currentUser = (await CreateUser(userName: "test@toto.com")).Id;

            LoginAs(userId1);

            var listId1 = (await CreateList("liste1")).Id;
            var listId2 = (await CreateList("liste2")).Id;

            await CreateShare(listId1, currentUser);

            LoginAs(currentUser);
            var listId3 = (await CreateList("liste3")).Id;

            var lists = await _listRepository.GetAllIncludingShares().ToListAsync();
            Assert.NotNull(lists);
            Assert.NotEmpty(lists);
            Assert.Equal(2, lists.Count);
            Assert.Contains(lists.Select(l => l.Id), l => l == listId1);
            Assert.Contains(lists.Select(l => l.Id), l => l == listId3);
            Assert.DoesNotContain(lists, l => l.Id == listId2);
            Assert.Equal(userId1, lists.Single(l => l.Id == listId1).UserId);
            Assert.Equal(currentUser, lists.Single(l => l.Id == listId3).UserId);
        }

        [Fact]
        public async Task GetAllIncludingShares_GetUserInfos()
        {
            var userId1 = (await CreateUser()).Id;
            var currentUser = (await CreateUser(userName: "test@toto.com")).Id;

            LoginAs(userId1);

            var listId1 = (await CreateList("liste1")).Id;

            await CreateShare(listId1, currentUser);

            LoginAs(currentUser);
            await CreateList("liste3");

            var lists = await _listRepository.GetAllIncludingShares().ToListAsync();
            Assert.NotNull(lists);
            Assert.NotEmpty(lists);
            Assert.Equal(2, lists.Count);
            var list1 = lists.SingleOrDefault(c => c.Id == listId1);
            Assert.NotNull(list1);
            Assert.NotNull(list1.User);
        }

        [Fact]
        public async Task GetAllIncludingShares_OnlyShare()
        {
            var userId1 = (await CreateUser()).Id;
            var currentUser = (await CreateUser(userName: "test@toto.com")).Id;

            LoginAs(userId1);

            var listId1 = (await CreateList("liste1")).Id;
            var listId2 = (await CreateList("liste2")).Id;

            await CreateShare(listId1, currentUser);
            await CreateShare(listId2, currentUser);

            LoginAs(currentUser);

            var lists = await _listRepository.GetAllIncludingShares().ToListAsync();
            Assert.NotNull(lists);
            Assert.NotEmpty(lists);
            Assert.Equal(2, lists.Count);
            Assert.Contains(lists.Select(l => l.Id), l => l == listId1);
            Assert.Contains(lists.Select(l => l.Id), l => l == listId2);
        }
    }
}
