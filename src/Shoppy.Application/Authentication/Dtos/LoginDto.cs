using System.ComponentModel.DataAnnotations;
using Shoppy.Core.Users;

namespace Shoppy.Application.Authentication.Dtos
{
    /// <summary>
    /// Account login informations.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Username used to authenticate. Usually the email account.
        /// </summary>
        [Required, StringLength(User.MaxLoginLength)]
        public string Username { get; set; }

        /// <summary>
        /// Password used to authenticate.
        /// </summary>
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
