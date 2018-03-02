using System;
using System.ComponentModel.DataAnnotations;
using Shoppy.Core.Commons;
using Shoppy.Core.Lists;

namespace Shoppy.Core.Items
{
    public class Item : Entity<Guid>
    {
        public const int MaxNameLength = 100;

        [Required, StringLength(MaxNameLength)]
        public string Name { get; set; }

        public bool Picked { get; set; }

        public Guid ListId { get; set; }
        public virtual List List { get; set; }

        public Item()
        {
            Id = Guid.NewGuid();
        }
    }
}
