using System;
using Shoppy.Application.Commons;
using Shoppy.Core.Auditing;

namespace Shoppy.Application.Users.Dtos
{
    /// <summary>
    /// An application user.
    /// </summary>
    public class UserDto : EntityDto<Guid>, ICreationTime, IModificationTime
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
