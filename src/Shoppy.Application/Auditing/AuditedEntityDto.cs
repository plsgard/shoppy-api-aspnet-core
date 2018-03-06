using System;
using Shoppy.Application.Commons;
using Shoppy.Core.Auditing;

namespace Shoppy.Application.Auditing
{
    public class AuditedEntityDto<TPrimaryKey> : EntityDto<TPrimaryKey>, IAudited
    {
        /// <summary>
        /// Creation datetime, in UTC format.
        /// </summary>
        public DateTimeOffset CreationTime { get; set; }
        
        /// <summary>
        /// Creation user unique id.
        /// </summary>
        public Guid? CreationUserId { get; set; }
        
        /// <summary>
        /// Modification datetime, in UTC format.
        /// </summary>
        public DateTimeOffset? ModificationTime { get; set; }
        
        /// <summary>
        /// Modification user unique id.
        /// </summary>
        public Guid? ModificationUserId { get; set; }
    }
}
