using System;
using System.ComponentModel.DataAnnotations;
using Shoppy.Application.Commons;
using Shoppy.Core.Users;

namespace Shoppy.Application.Users.Dtos
{
    public class UpdateUserDto : EntityDto<Guid>
    {
        [Required, StringLength(User.MaxFirstNameLength)]
        public string FirstName { get; set; }

        [Required, StringLength(User.MaxLastNameLength)]
        public string LastName { get; set; }
    }
}