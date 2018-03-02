namespace Shoppy.Application.Commons
{
    public interface IEntityDto<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
    }
}