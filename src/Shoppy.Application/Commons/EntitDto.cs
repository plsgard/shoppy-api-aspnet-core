using Shoppy.Application.Items.Dtos;

namespace Shoppy.Application.Commons
{
    public class EntitDto<TPrimaryKey>:IEntityDto<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }
    }
}