using System;

namespace Shoppy.Core.Session
{
    public class AppSession : IAppSession
    {
        public Guid? GetCurrentUserId()
        {
            return null;
        }
    }
}
