using System;

namespace Shoppy.Core.Auditing
{
    public interface ICreationAudited : ICreationTime
    {
        Guid? CreationUserId { get; set; }
    }
}