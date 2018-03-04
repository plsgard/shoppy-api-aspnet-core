using System;
using Shoppy.Core.Commons;
using Shoppy.Core.Items;
using Shoppy.Core.Validation;

namespace Shoppy.Application.Items.Dtos
{
    public class GetAllItemsDto : ISorted, IShouldNormalize
    {
        public Guid? ListId { get; set; }

        public string Sorting { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrWhiteSpace(Sorting))
                Sorting = $"{nameof(Item.Index)} ASC";
        }
    }
}
