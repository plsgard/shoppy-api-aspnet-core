namespace Shoppy.Application.Commons
{
    public class EntitDto<TPrimaryKey> : IEntityDto<TPrimaryKey>
    {
        /// <summary>
        /// Unique ID.
        /// </summary>
        public TPrimaryKey Id { get; set; }
    }
}