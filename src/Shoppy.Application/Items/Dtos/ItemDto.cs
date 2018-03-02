using System;
using Shoppy.Application.Commons;

namespace Shoppy.Application.Items.Dtos
{
    public class ItemDto : EntitDto<Guid>
    {
        public Guid ListId { get; set; }

        public string Name { get; set; }
    }
}
