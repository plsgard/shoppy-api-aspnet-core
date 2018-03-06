namespace Shoppy.Core.Commons
{
    public abstract class SortedDto : ISorted
    {
        protected SortedDto()
        {
            SortType = SortType.ASC;
        }

        /// <summary>
        /// The property name to sort on.
        /// </summary>
        public string Sorting { get; set; }

        /// <summary>
        /// The kind of sort : ascending (ASC) or descending (DESC)
        /// </summary>
        public SortType SortType { get; set; }

        public string ToSortString()
        {
            return $"{Sorting.Trim()} {SortType.ToString().ToUpperInvariant()}";
        }
    }
}