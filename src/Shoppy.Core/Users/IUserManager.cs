using System;

namespace Shoppy.Core.Users
{
    public interface IUserManager
    {
        Guid? GetCurrentUserId();
    }
}
