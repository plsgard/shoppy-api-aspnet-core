using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shoppy.Core.Auditing;
using Shoppy.Core.Commons;
using Shoppy.Core.Items;
using Shoppy.Core.Lists;
using Shoppy.Core.Users;

namespace Shoppy.Data
{
    public class ShoppyContext : DbContext
    {
        private readonly IUserManager _userManager;

        public DbSet<List> Lists { get; set; }

        public DbSet<Item> Items { get; set; }

        public ShoppyContext(DbContextOptions<ShoppyContext> options, IUserManager userManager) : base(options)
        {
            _userManager = userManager;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ISoftDelete>().HasQueryFilter(p => !p.IsDeleted);
            var currentUserId = _userManager.GetCurrentUserId();
            modelBuilder.Entity<IMustHaveUser>()
                .HasQueryFilter(p => currentUserId.HasValue && p.UserId == currentUserId.Value);
            modelBuilder.Entity<IMayHaveUser>()
                .HasQueryFilter(p => !p.UserId.HasValue || p.UserId == currentUserId);
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
            var entries = ChangeTracker.Entries().Where(e =>
                e.Entity is ICreationTime && e.State == EntityState.Added ||
                e.Entity is IModificationTime && e.State == EntityState.Modified ||
                e.Entity is ISoftDelete && e.State == EntityState.Deleted);

            foreach (var entry in entries)
            {
                if (entry.State != EntityState.Deleted && entry.Entity is ISoftDelete auditedDelete)
                {
                    auditedDelete.IsDeleted = false;
                    if (entry.Entity is IDeletionTime deletedEntity)
                        deletedEntity.DeletionTime = null;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        ((ICreationTime)entry.Entity).CreationTime = DateTimeOffset.UtcNow;
                        if (entry.Entity is ICreationAudited creationAudited)
                            creationAudited.CreationUserId = _userManager.GetCurrentUserId();
                        break;
                    case EntityState.Modified:
                        ((IModificationTime)entry.Entity).ModificationTime = DateTimeOffset.UtcNow;
                        if (entry.Entity is IModificationAudited modificationAudited)
                            modificationAudited.ModificationUserId = _userManager.GetCurrentUserId();
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues[nameof(ISoftDelete.IsDeleted)] = true;
                        if (entry.Entity is IDeletionTime deletedEntity)
                            deletedEntity.DeletionTime = DateTimeOffset.UtcNow;
                        break;
                }
            }
        }
    }
}
