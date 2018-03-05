using System;
using Shoppy.Core.Commons;

namespace Shoppy.Core.Auditing
{
    public interface IDeletionTime : ISoftDelete
    {
        DateTimeOffset? DeletionTime { get; set; }
    }
}
