using System;

namespace Shoppy.Core.Auditing
{
    public interface IModificationTime
    {
        DateTimeOffset? ModificationTime { get; set; }
    }
}