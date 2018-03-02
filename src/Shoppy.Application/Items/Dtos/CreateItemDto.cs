using System;
using System.ComponentModel.DataAnnotations;
using Shoppy.Core.Items;

namespace Shoppy.Application.Items.Dtos
{
    /// <summary>
    /// A new shopping item to create.
    /// </summary>
    public class CreateItemDto
    {
        /// <summary>
        /// Unique id of the list on which the item should created.
        /// </summary>
        public Guid ListId { get; set; }

        /// <summary>
        /// Name of the new item.
        /// </summary>
        [Required, StringLength(Item.MaxNameLength)]
        public string Name { get; set; }
    }
}