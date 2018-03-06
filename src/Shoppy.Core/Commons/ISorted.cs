namespace Shoppy.Core.Commons
{
    public interface ISorted
    {
        string Sorting { get; set; }

        SortType SortType { get; set; }

        string ToSortString();
    }

    public enum SortType
    {
        ASC,
        DESC
    }
}
