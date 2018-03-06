using System;
using Shoppy.Core.Commons;
using Shoppy.Core.Items;

namespace Shoppy.Application.Items.Dtos
{
    /// <summary>
    /// Uses to filter and sort items.
    /// </summary>
    public class GetAllItemsDto : SortedDto
    {
        /// <summary>
        /// List unique id of the items to filter.
        /// </summary>
        public Guid? ListId { get; set; }

        public override void Normalize()
        {
            base.Normalize();
            if (string.IsNullOrWhiteSpace(SortProperty))
                SortProperty = $"{nameof(Item.Index)}";
        }
    }
}
