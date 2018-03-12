﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shoppy.Application.Lists;
using Shoppy.Application.Lists.Dtos;
using Shoppy.Core.Lists;
using Shoppy.Data.Repositories;
using Xunit;

namespace Shoppy.Tests.Services
{
    public class ListAppServiceTests : AppTestBase
    {
        private readonly ListAppService _listAppService;

        public ListAppServiceTests()
        {
            _listAppService = new ListAppService(new Repository<List, Guid>(Context));
        }

        [Theory]
        [InlineData("  test", "test")]
        [InlineData("  test  ", "test")]
        [InlineData("test  ", "test")]
        public async Task Create_Name_ShouldBeNormalized_Trimmed(string val, string expectedResult)
        {
            var listDto = await _listAppService.Create(new CreateListDto
            {
                Name = val
            });
            Assert.Equal(expectedResult, listDto.Name);

            await UseDbContextAsync(async context =>
                Assert.Equal(expectedResult, (await context.Lists.IgnoreQueryFilters().FirstAsync(l => l.Id == listDto.Id)).Name));
        }

        [Theory]
        [InlineData("  test", "test")]
        [InlineData("  test  ", "test")]
        [InlineData("test  ", "test")]
        public async Task Update_Name_ShouldBeNormalized_Trimmed(string val, string expectedResult)
        {
            var listId = (await CreateList("toto")).Id;
            var listDto = await _listAppService.Update(new UpdateListDto
            {
                Id = listId,
                Name = val
            });
            Assert.Equal(expectedResult, listDto.Name);

            await UseDbContextAsync(async context =>
                Assert.Equal(expectedResult, (await context.Lists.IgnoreQueryFilters().FirstAsync(l => l.Id == listDto.Id)).Name));
        }
    }
}
