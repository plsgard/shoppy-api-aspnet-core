namespace Shoppy.Application.Commons
{
    public class EntityDto<TPrimaryKey> : IEntityDto<TPrimaryKey>
    {
        /// <summary>
        /// Unique ID.
        /// </summary>
        public TPrimaryKey Id { get; set; }
    }
}