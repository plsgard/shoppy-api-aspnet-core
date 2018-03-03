using System;

namespace Shoppy.Core.Auditing
{
    public interface ICreationTime
    {
        DateTimeOffset CreationTime { get; set; }
    }
}
