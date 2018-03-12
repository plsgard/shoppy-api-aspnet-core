using System.ComponentModel.DataAnnotations;
using Shoppy.Core.Lists;
using Shoppy.Core.Validation;

namespace Shoppy.Application.Lists.Dtos
{
    /// <summary>
    /// A shopping list to create.
    /// </summary>
    public class CreateListDto : IShouldNormalize
    {
        /// <summary>
        /// Name of the new list.
        /// </summary>
        [Required, StringLength(List.MaxNameLength)]
        public string Name { get; set; }

        public void Normalize()
        {
            Name = Name?.Trim();
        }
    }
}