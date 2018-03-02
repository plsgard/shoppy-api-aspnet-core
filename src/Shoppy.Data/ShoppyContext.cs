using Microsoft.EntityFrameworkCore;
using Shoppy.Core.Items;
using Shoppy.Core.Lists;

namespace Shoppy.Data
{
    public class ShoppyContext : DbContext
    {
        public DbSet<List> Lists { get; set; }
        public DbSet<Item> Items { get; set; }

        public ShoppyContext(DbContextOptions<ShoppyContext> options) : base(options)
        {
        }
    }
}
