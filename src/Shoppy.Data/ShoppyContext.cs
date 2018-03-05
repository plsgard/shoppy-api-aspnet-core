﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shoppy.Core.Auditing;
using Shoppy.Core.Commons;
using Shoppy.Core.Items;
using Shoppy.Core.Lists;
using Shoppy.Core.Roles;
using Shoppy.Core.Session;
using Shoppy.Core.Users;

namespace Shoppy.Data
{
    public class ShoppyContext : IdentityDbContext<User, Role, Guid>
    {
        private readonly IAppSession _appSession;

        public DbSet<List> Lists { get; set; }

        public DbSet<Item> Items { get; set; }

        public ShoppyContext(DbContextOptions<ShoppyContext> options, IAppSession appSession) : base(options)
        {
            _appSession = appSession;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //foreach (var entityType in modelBuilder.Model.GetEntityTypes(typeof(ISoftDelete)))
            //{
            //    entityType.Fil
            //}

            //modelBuilder.Entity<ISoftDelete>().HasQueryFilter(p => !p.IsDeleted);
            //var currentUserId = _appSession.GetCurrentUserId();
            //modelBuilder.Entity<IMustHaveUser>()
            //    .HasQueryFilter(p => currentUserId.HasValue && p.UserId == currentUserId.Value);
            //modelBuilder.Entity<IMayHaveUser>()
            //    .HasQueryFilter(p => !p.UserId.HasValue || p.UserId == currentUserId);
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
                            creationAudited.CreationUserId = _appSession.GetCurrentUserId();
                        break;
                    case EntityState.Modified:
                        ((IModificationTime)entry.Entity).ModificationTime = DateTimeOffset.UtcNow;
                        if (entry.Entity is IModificationAudited modificationAudited)
                            modificationAudited.ModificationUserId = _appSession.GetCurrentUserId();
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues[nameof(ISoftDelete.IsDeleted)] = true;
                        if (entry.Entity is IDeletionTime deletedEntity)
                            deletedEntity.DeletionTime = DateTimeOffset.UtcNow;
                        if (entry.Entity is IDeletionAudited deletedAutidedEntity)
                            deletedAutidedEntity.DeletionUserId = _appSession.GetCurrentUserId();
                        break;
                }
            }
        }
    }
}
