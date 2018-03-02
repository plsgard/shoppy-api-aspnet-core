using System;
using System.ComponentModel.DataAnnotations;
using Shoppy.Core.Items;

namespace Shoppy.Application.Items.Dtos
{
    public class CreateItemDto
    {
        public Guid ListId { get; set; }

        [Required, StringLength(Item.MaxNameLength)]
        public string Name { get; set; }
    }
}