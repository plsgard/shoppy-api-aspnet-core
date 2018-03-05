using System;

namespace Shoppy.Core.Commons
{
    public interface IMustHaveUser
    {
        Guid UserId { get; set; }
    }
}