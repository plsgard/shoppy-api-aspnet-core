using System;
using Shoppy.Core.Commons;

namespace Shoppy.Core.Auditing
{
    public class AuditedEntity<TPrimaryKey> : Entity<TPrimaryKey>, IAudited
    {
        public DateTimeOffset CreationTime { get; set; }
        public Guid? CreationUserId { get; set; }
        public DateTimeOffset? ModificationTime { get; set; }
        public Guid? ModificationUserId { get; set; }
    }
}
