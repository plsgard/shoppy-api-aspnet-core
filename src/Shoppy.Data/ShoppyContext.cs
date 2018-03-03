using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shoppy.Core.Auditing;
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

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ApplyAuditedProperties();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            ApplyAuditedProperties();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void ApplyAuditedProperties()
        {
            var auditedEntities = ChangeTracker.Entries().Where(e =>
                e.Entity is ICreationTime && e.State == EntityState.Added ||
                e.Entity is IModificationTime && e.State == EntityState.Modified);

            foreach (var auditedEntity in auditedEntities)
            {
                if (auditedEntity.State == EntityState.Added)
                    ((ICreationTime)auditedEntity.Entity).CreationTime = DateTimeOffset.UtcNow;
                else if (auditedEntity.State == EntityState.Modified)
                    ((IModificationTime)auditedEntity.Entity).ModificationTime = DateTimeOffset.UtcNow;
            }
        }
    }
}
