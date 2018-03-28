using System;
using Shoppy.Core.Auditing;

namespace Shoppy.Application.Users.Dtos
{
    /// <summary>
    /// An application user.
    /// </summary>
    public class UserDto : UserInfoDto, ICreationTime, IModificationTime
    {
        /// <summary>
        /// Username of the user account.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// When the user has been created.
        /// </summary>
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// When the user has been updated, if it was.
        /// </summary>
        public DateTimeOffset? ModificationTime { get; set; }
    }
}
