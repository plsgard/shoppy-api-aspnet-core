using System;

namespace Shoppy.Core.Commons
{
    public interface IMayHaveUser
    {
        Guid? UserId { get; set; }
    }
}