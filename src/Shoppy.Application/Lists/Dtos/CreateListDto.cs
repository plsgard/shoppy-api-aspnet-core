using System.ComponentModel.DataAnnotations;
using Shoppy.Core.Lists;

namespace Shoppy.Application.Lists.Dtos
{
    /// <summary>
    /// A shopping list to create.
    /// </summary>
    public class CreateListDto
    {
        /// <summary>
        /// Name of the new list.
        /// </summary>
        [Required, StringLength(List.MaxNameLength)]
        public string Name { get; set; }
    }
}