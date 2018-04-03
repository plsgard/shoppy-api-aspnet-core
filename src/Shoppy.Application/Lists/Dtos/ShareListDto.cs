using System;
using System.ComponentModel.DataAnnotations;

namespace Shoppy.Application.Lists.Dtos
{
    public class ShareListDto
    {
        [Required]
        public string UserName { get; set; }

        public Guid ListId { get; set; }
    }
}
