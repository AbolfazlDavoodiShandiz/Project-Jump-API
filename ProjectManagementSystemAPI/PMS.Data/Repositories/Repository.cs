using Microsoft.EntityFrameworkCore;
using PMS.Common.Utility;
using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IDbEntity
    {
        protected readonly ApplicationDbContext ApplicationDbContext;

        public Repository(ApplicationDbContext context)
        {
            ApplicationDbContext = context;
            Entities = ApplicationDbContext.Set<TEntity>();
        }

        public DbSet<TEntity> Entities { get; }

        public virtual IQueryable<TEntity> Table => Entities;

        public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();

        public virtual void Add(TEntity entity, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));

            Entities.Add(entity);

            if (saveNow)
            {
                ApplicationDbContext.SaveChanges();
            }
        }

        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));

            await Entities.AddAsync(entity, cancellationToken);

            if (saveNow)
            {
                await ApplicationDbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public virtual void AddRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));

            Entities.AddRange(entities);

            if (saveNow)
            {
                ApplicationDbContext.SaveChanges(); //TODO: What if an entity not inserted?
            }
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));

            await Entities.AddRangeAsync(entities, cancellationToken);

            if (saveNow)
            {
                await ApplicationDbContext.SaveChangesAsync(cancellationToken); //TODO: What if an entity not inserted?
            }
        }

        public virtual void Detach(TEntity entity)
        {
            Assert.NotNull(entity, nameof(entity));

            var entry = ApplicationDbContext.Entry(entity);

            if (entry != null)
            {
                entry.State = EntityState.Detached;
            }
        }

        public virtual void Attach(TEntity entity)
        {
            Assert.NotNull(entity, nameof(entity));

            if (ApplicationDbContext.Entry(entity).State == EntityState.Detached)
            {
                Entities.Attach(entity);
            }
        }

        public virtual void Delete(TEntity entity, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));

            Entities.Remove(entity);

            if (saveNow)
            {
                ApplicationDbContext.SaveChanges();
            }
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));

            Entities.Remove(entity);

            if (saveNow)
            {
                await ApplicationDbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));

            Entities.RemoveRange(entities);

            if (saveNow)
            {
                ApplicationDbContext.SaveChanges();
            }
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));

            Entities.RemoveRange(entities);

            if (saveNow)
            {
                await ApplicationDbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public virtual TEntity GetById(params object[] ids)
        {
            return Entities.Find(ids);
        }

        public virtual ValueTask<TEntity> GetByIdAsync(CancellationToken cancellationToken, params object[] ids)
        {
            return Entities.FindAsync(ids, cancellationToken);
        }

        public virtual async Task LoadCollectionAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty, CancellationToken cancellationToken)
            where TProperty : class
        {
            Attach(entity);

            var collection = ApplicationDbContext.Entry(entity).Collection(collectionProperty);

            if (!collection.IsLoaded)
            {
                await collection.LoadAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual void LoadCollection<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty)
            where TProperty : class
        {
            Attach(entity);

            var collection = ApplicationDbContext.Entry(entity).Collection(collectionProperty);

            if (!collection.IsLoaded)
            {
                collection.Load();
            }
        }

        public virtual async Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty, CancellationToken cancellationToken)
            where TProperty : class
        {
            Attach(entity);

            var reference = ApplicationDbContext.Entry(entity).Reference(referenceProperty);

            if (!reference.IsLoaded)
            {
                await reference.LoadAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty)
            where TProperty : class
        {
            Attach(entity);

            var reference = ApplicationDbContext.Entry(entity).Reference(referenceProperty);

            if (!reference.IsLoaded)
            {
                reference.Load();
            }
        }

        public virtual void Update(TEntity entity, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));

            Entities.Update(entity);

            if (saveNow)
            {
                ApplicationDbContext.SaveChanges();
            }
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));

            Entities.Update(entity);

            if (saveNow)
            {
                await ApplicationDbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));

            Entities.UpdateRange(entities);

            if (saveNow)
            {
                ApplicationDbContext.SaveChanges();
            }
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));

            Entities.UpdateRange(entities);

            if (saveNow)
            {
                await ApplicationDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
