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
        protected readonly ApplicationDbContext ApplicationDbCintext;

        public Repository(ApplicationDbContext context)
        {
            ApplicationDbCintext = context;
            Entities = ApplicationDbCintext.Set<TEntity>();
        }

        public DbSet<TEntity> Entities { get; }

        public virtual IQueryable<TEntity> Table => Entities;

        public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();

        public virtual bool Add(TEntity entity, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));

            Entities.Add(entity);

            bool result = false;

            if (saveNow)
            {
                int id = ApplicationDbCintext.SaveChanges();

                if (id > 0)
                {
                    result = true;
                }
            }

            return result;
        }

        public virtual async Task<bool> AddAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));

            await Entities.AddAsync(entity, cancellationToken);

            bool result = false;

            if (saveNow)
            {
                int id = await ApplicationDbCintext.SaveChangesAsync(cancellationToken);

                if (id > 0)
                {
                    result = true;
                }
            }

            return result;
        }

        public virtual bool AddRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));

            Entities.AddRange(entities);

            bool result = false;

            if (saveNow)
            {
                int id = ApplicationDbCintext.SaveChanges(); //TODO: What if an entity not inserted?

                if (id > 0)
                {
                    result = true;
                }
            }

            return result;
        }

        public virtual async Task<bool> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));

            await Entities.AddRangeAsync(entities, cancellationToken);

            bool result = false;

            if (saveNow)
            {
                int id = await ApplicationDbCintext.SaveChangesAsync(cancellationToken); //TODO: What if an entity not inserted?

                if (id > 0)
                {
                    result = true;
                }
            }

            return result;
        }

        public virtual void Detach(TEntity entity)
        {
            Assert.NotNull(entity, nameof(entity));

            var entry = ApplicationDbCintext.Entry(entity);

            if (entry != null)
            {
                entry.State = EntityState.Detached;
            }
        }

        public virtual void Attach(TEntity entity)
        {
            Assert.NotNull(entity, nameof(entity));

            if (ApplicationDbCintext.Entry(entity).State == EntityState.Detached)
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
                ApplicationDbCintext.SaveChanges();
            }
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));

            Entities.Remove(entity);

            if (saveNow)
            {
                await ApplicationDbCintext.SaveChangesAsync(cancellationToken);
            }
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));

            Entities.RemoveRange(entities);

            if (saveNow)
            {
                ApplicationDbCintext.SaveChanges();
            }
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));

            Entities.RemoveRange(entities);

            if (saveNow)
            {
                await ApplicationDbCintext.SaveChangesAsync(cancellationToken);
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

            var collection = ApplicationDbCintext.Entry(entity).Collection(collectionProperty);

            if (!collection.IsLoaded)
            {
                await collection.LoadAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual void LoadCollection<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty)
            where TProperty : class
        {
            Attach(entity);

            var collection = ApplicationDbCintext.Entry(entity).Collection(collectionProperty);

            if (!collection.IsLoaded)
            {
                collection.Load();
            }
        }

        public virtual async Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty, CancellationToken cancellationToken)
            where TProperty : class
        {
            Attach(entity);

            var reference = ApplicationDbCintext.Entry(entity).Reference(referenceProperty);

            if (!reference.IsLoaded)
            {
                await reference.LoadAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty)
            where TProperty : class
        {
            Attach(entity);

            var reference = ApplicationDbCintext.Entry(entity).Reference(referenceProperty);

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
                ApplicationDbCintext.SaveChanges();
            }
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));

            Entities.Update(entity);

            if (saveNow)
            {
                await ApplicationDbCintext.SaveChangesAsync(cancellationToken);
            }
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));

            Entities.UpdateRange(entities);

            if (saveNow)
            {
                ApplicationDbCintext.SaveChanges();
            }
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));

            Entities.UpdateRange(entities);

            if (saveNow)
            {
                await ApplicationDbCintext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
