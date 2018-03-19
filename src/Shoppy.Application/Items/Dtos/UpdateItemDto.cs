using System;
using Shoppy.Application.Commons;

namespace Shoppy.Application.Items.Dtos
{
    /// <summary>
    /// An item to update.
    /// </summary>
    public class UpdateItemDto : CreateItemDto, IEntityDto<Guid>
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Determines if the item has been picked (or not) on the shopping list.
        /// </summary>
        public bool Picked { get; set; }

        /// <summary>
        /// The index position of the item in the list.
        /// </summary>
        public int Index { get; set; }
    }
}