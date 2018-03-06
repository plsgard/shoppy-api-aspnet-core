namespace Shoppy.Core.Commons
{
    public interface ISorted
    {
        string SortProperty { get; set; }

        SortType? SortType { get; set; }
    }

    public enum SortType
    {
        ASC,
        DESC
    }
}
