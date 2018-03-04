using Shoppy.Core.Validation;

namespace Shoppy.Core.Data
{
    public abstract class BaseRepository
    {
        protected void Normalize(object input)
        {
            if (input == null)
                return;

            if (input is IShouldNormalize normalize)
                normalize.Normalize();
        }
    }
}
