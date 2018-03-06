using Shoppy.Core.Validation;

namespace Shoppy.Core.Commons
{
    public abstract class SortedDto : ISorted, IShouldNormalize
    {
        protected SortedDto()
        {
            SortType = Commons.SortType.ASC;
        }

        /// <summary>
        /// The property name to sort on.
        /// </summary>
        public string SortProperty { get; set; }

        /// <summary>
        /// The kind of sort : ascending (ASC) or descending (DESC)
        /// </summary>
        public SortType? SortType { get; set; }

        public virtual void Normalize()
        {
            if (!SortType.HasValue)
                SortType = Commons.SortType.ASC;
        }
    }
}