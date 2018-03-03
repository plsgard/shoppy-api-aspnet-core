using System;
using Shoppy.Application.Commons;
using Shoppy.Core.Auditing;

namespace Shoppy.Application.Items.Dtos
{
    /// <summary>
    /// An existing shopping item.
    /// </summary>
    public class ItemDto : EntityDto<Guid>, ICreationTime, IModificationTime
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

        /// <summary>
        /// The index position of the item in the list.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// When the item has been created.
        /// </summary>
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// When the item has been updated, if it was.
        /// </summary>
        public DateTimeOffset? ModificationTime { get; set; }
    }
}
