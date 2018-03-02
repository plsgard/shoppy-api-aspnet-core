namespace Shoppy.Core.Commons
{
    public class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }
    }
}