using System.ComponentModel.DataAnnotations;
using Shoppy.Core.Users;

namespace Shoppy.Application.Users.Dtos
{
    public class CreateUserDto
    {
        [Required, StringLength(User.MaxFirstNameLength)]
        public string FirstName { get; set; }

        [Required, StringLength(User.MaxLastNameLength)]
        public string LastName { get; set; }

        [Required, StringLength(User.MaxEmailLength), DataType(DataType.EmailAddress), EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(User.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required, MinLength(User.MinPasswordLength), DataType(DataType.Password)]
        public string Password { get; set; }
    }
}