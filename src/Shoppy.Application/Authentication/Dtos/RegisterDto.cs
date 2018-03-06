using System.ComponentModel.DataAnnotations;
using Shoppy.Core.Users;

namespace Shoppy.Application.Authentication.Dtos
{
    /// <summary>
    /// A new user account to create and register.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Firstname of the user.
        /// </summary>
        [Required, StringLength(User.MaxFirstNameLength)]
        public string FirstName { get; set; }

        /// <summary>
        /// Lastname of the user.
        /// </summary>
        [Required, StringLength(User.MaxLastNameLength)]
        public string LastName { get; set; }

        /// <summary>
        /// Unique email of the user.
        /// </summary>
        [Required, StringLength(User.MaxEmailLength), DataType(DataType.EmailAddress), EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Password of the user account.
        /// </summary>
        [Required, MinLength(User.MinPasswordLength), DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Password confirmation. Must be identical of the password field.
        /// </summary>
        [Required, MinLength(User.MinPasswordLength), DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
