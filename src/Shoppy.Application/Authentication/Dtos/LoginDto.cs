using System.ComponentModel.DataAnnotations;
using Shoppy.Core.Users;

namespace Shoppy.Application.Authentication.Dtos
{
    public class LoginDto
    {
        [Required, StringLength(User.MaxLoginLength)]
        public string Ùsername { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
