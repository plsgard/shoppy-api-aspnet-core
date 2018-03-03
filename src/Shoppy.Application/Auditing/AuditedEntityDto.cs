using System;
using Shoppy.Application.Commons;
using Shoppy.Core.Auditing;

namespace Shoppy.Application.Auditing
{
    public class AuditedEntityDto<TPrimaryKey> : EntityDto<TPrimaryKey>, IAudited
    {
        public DateTimeOffset CreationTime { get; set; }
        public Guid? CreationUserId { get; set; }
        public DateTimeOffset? ModificationTime { get; set; }
        public Guid? ModificationUserId { get; set; }
    }
}
