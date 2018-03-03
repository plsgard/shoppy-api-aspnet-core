using System;

namespace Shoppy.Core.Auditing
{
    public interface IModificationAudited : IModificationTime
    {
        Guid? ModificationUserId { get; set; }
    }
}