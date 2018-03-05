using System;

namespace Shoppy.Core.Auditing
{
    public class FullAuditedEntity<TPrimaryKey> : AuditedEntity<TPrimaryKey>, IFullAudited
    {
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletionTime { get; set; }
        public Guid? DeletionUserId { get; set; }
    }
}