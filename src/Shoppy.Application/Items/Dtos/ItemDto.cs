using System;
using Shoppy.Application.Commons;

namespace Shoppy.Application.Items.Dtos
{
    /// <summary>
    /// An existing shopping item.
    /// </summary>
    public class ItemDto : EntitDto<Guid>
    {
        /// <summary>
        /// Unique id of the associated shopping list.
        /// </summary>
        public Guid ListId { get; set; }

        /// <summary>
        /// Name of the item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Determines if the item has been picked (or not) on the shopping list.
        /// </summary>
        public bool IsPicked { get; set; }
    }
}
