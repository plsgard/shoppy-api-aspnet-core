using System.ComponentModel.DataAnnotations;
using Shoppy.Core.Lists;

namespace Shoppy.Application.Lists.Dtos
{
    public class CreateListDto
    {
        [Required, StringLength(List.MaxNameLength)]
        public string Name { get; set; }
    }
}