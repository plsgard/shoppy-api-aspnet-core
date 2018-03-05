using System;

namespace Shoppy.Core.Session
{
    public interface IAppSession
    {
        Guid? GetCurrentUserId();
    }
}
