using System;
using Shoppy.Application.Auditing;
using Shoppy.Application.Users.Dtos;

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

        /// <summary>
        /// User who owns the list.
        /// </summary>
        public UserInfoDto User { get; set; }
    }
}
