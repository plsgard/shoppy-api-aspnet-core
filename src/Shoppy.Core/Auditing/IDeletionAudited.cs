using System;

namespace Shoppy.Core.Auditing
{
    public interface IDeletionAudited : IDeletionTime
    {
        Guid? DeletionUserId { get; set; }
    }
}