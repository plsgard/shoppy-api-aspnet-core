using System;
using Shoppy.Application.Commons;
using Shoppy.Core.Auditing;

namespace Shoppy.Application.Auditing
{
    public class AuditedTimeEntityDto<TPrimaryKey> : EntityDto<TPrimaryKey>, IAuditedTime
    {
        /// <summary>
        /// Creation datetime, in UTC format.
        /// </summary>
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// Modification datetime, in UTC format.
        /// </summary>
        public DateTimeOffset? ModificationTime { get; set; }
    }
}