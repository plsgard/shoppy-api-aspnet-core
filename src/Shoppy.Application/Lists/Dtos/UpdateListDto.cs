using System;
using Shoppy.Application.Commons;

namespace Shoppy.Application.Lists.Dtos
{
    /// <summary>
    /// A shopping list to update.
    /// </summary>
    public class UpdateListDto : CreateListDto, IEntityDto<Guid>
    {
        public Guid Id { get; set; }
    }
}