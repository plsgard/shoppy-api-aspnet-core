using System;
using Shoppy.Application.Commons;

namespace Shoppy.Application.Lists.Dtos
{
    /// <summary>
    /// An existing shopping list.
    /// </summary>
    public class ListDto : EntitDto<Guid>
    {
        /// <summary>
        /// Name of the list.
        /// </summary>
        public string Name { get; set; }
    }
}
