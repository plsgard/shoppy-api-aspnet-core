using System;
using Shoppy.Application.Auditing;

namespace Shoppy.Application.Users.Dtos
{
    /// <summary>
    /// An application user.
    /// </summary>
    public class UserDto : AuditedEntityDto<Guid>
    {
        /// <summary>
        /// Firstname of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Lastname of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Url of the user account picture.
        /// </summary>
        public string PictureUrl { get; set; }

        /// <summary>
        /// Email of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Username of the user account.
        /// </summary>
        public string UserName { get; set; }
    }
}
