using System;
using Shoppy.Application.Commons;

namespace Shoppy.Application.Lists.Dtos
{
    public class ListDto : EntitDto<Guid>
    {
        public string Name { get; set; }
    }
}
