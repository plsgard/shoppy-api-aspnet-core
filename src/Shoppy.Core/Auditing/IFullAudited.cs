using Shoppy.Core.Commons;

namespace Shoppy.Core.Auditing
{
    public interface IFullAudited : IAudited, IDeletionAudited, ISoftDelete { }
}