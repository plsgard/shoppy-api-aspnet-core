using System;
using Shoppy.Application.Auditing;

namespace Shoppy.Application.Lists.Dtos
{
    /// <summary>
    /// An existing shopping list.
    /// </summary>
    public class ListDto : AuditedTimeEntityDto<Guid>
    {
        /// <summary>
        /// Name of the list.
        /// </summary>
        public string Name { get; set; }
    }
}
