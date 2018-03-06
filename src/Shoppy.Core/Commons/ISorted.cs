namespace Shoppy.Core.Commons
{
    public interface ISorted
    {
        /// <summary>
        /// The property name to sort on.
        /// </summary>
        string SortProperty { get; set; }

        /// <summary>
        /// The kind of sort : ascending (ASC) or descending (DESC)
        /// </summary>
        SortType? SortType { get; set; }
    }

    public enum SortType
    {
        ASC,
        DESC
    }
}
