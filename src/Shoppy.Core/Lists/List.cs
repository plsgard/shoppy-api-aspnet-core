using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shoppy.Core.Auditing;
using Shoppy.Core.Items;

namespace Shoppy.Core.Lists
{
    public class List : AuditedEntity<Guid>
    {
        public const int MaxNameLength = 50;

        [Required, StringLength(MaxNameLength)]
        public string Name { get; set; }

        public virtual ICollection<Item> Items { get; set; }

        public List()
        {
            Id = Guid.NewGuid();
        }
    }
}
