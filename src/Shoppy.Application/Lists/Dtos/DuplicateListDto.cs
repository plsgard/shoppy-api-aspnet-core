using System;

namespace Shoppy.Application.Lists.Dtos
{
    public class DuplicateListDto : CreateListDto
    {
        /// <summary>
        /// ID of the existing list.
        /// </summary>
        public Guid ExistingListId { get; set; }
    }
}