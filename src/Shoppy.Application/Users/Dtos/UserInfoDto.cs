using System;
using Shoppy.Application.Commons;

namespace Shoppy.Application.Users.Dtos
{
    public class UserInfoDto : EntityDto<Guid>
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
    }
}