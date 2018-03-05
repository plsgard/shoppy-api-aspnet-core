using System;
using Shoppy.Application.Auditing;

namespace Shoppy.Application.Users.Dtos
{
    public class UserDto : AuditedEntityDto<Guid>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PictureUrl { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }
    }
}
