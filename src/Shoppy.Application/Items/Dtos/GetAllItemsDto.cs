using System;
using Shoppy.Core.Commons;
using Shoppy.Core.Items;
using Shoppy.Core.Validation;

namespace Shoppy.Application.Items.Dtos
{
    /// <summary>
    /// Uses to filter and sort items.
    /// </summary>
    public class GetAllItemsDto : SortedDto, IShouldNormalize
    {
        /// <summary>
        /// List unique id of the items to filter.
        /// </summary>
        public Guid? ListId { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrWhiteSpace(Sorting))
                Sorting = $"{nameof(Item.Index)}";
        }
    }
}
