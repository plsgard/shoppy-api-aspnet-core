namespace Shoppy.Application.Lists.Dtos
{
    /// <summary>
    /// Used to filter lists to return.
    /// </summary>
    public class GetAllListsDto
    {
        /// <summary>
        /// Determines if shared lists will also be returned.
        /// </summary>
        public bool LoadShares { get; set; }
    }
}
