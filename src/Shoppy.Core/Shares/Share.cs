using System;
using Shoppy.Core.Lists;
using Shoppy.Core.Users;

namespace Shoppy.Core.Shares
{
    public class Share
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public Guid ListId { get; set; }
        public virtual List List { get; set; }
    }
}
