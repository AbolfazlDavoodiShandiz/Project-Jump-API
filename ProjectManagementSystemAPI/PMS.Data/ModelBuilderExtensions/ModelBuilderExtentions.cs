using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Pluralize.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Data.ModelBuilderExtensions
{
    internal static class ModelBuilderExtentions
    {
        internal static void RegisterAllEntities<BaseType>(this ModelBuilder modelBuilder, params Assembly[] assemblies)
        {
            IEnumerable<Type> entities = assemblies.SelectMany(t => t.GetExportedTypes())
                .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic && typeof(BaseType).IsAssignableFrom(t));

            foreach (Type entity in entities)
            {
                modelBuilder.Entity(entity);
            }
        }

        internal static void RegisterAllEntityConfigurations(this ModelBuilder modelBuilder, params Assembly[] assemblies)
        {
            IEnumerable<Type> types = assemblies.SelectMany(t => t.GetExportedTypes())
                .Where(t => t.IsClass && t.IsPublic && !t.IsAbstract);

            MethodInfo builderMethod = typeof(ModelBuilder).GetMethods().First(m => m.Name == nameof(ModelBuilder.ApplyConfiguration));

            foreach (Type type in types)
            {
                foreach (Type innerType in type.GetInterfaces())
                {
                    if (innerType.IsConstructedGenericType && innerType.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                    {
                        MethodInfo applyMethod = builderMethod.MakeGenericMethod(innerType.GenericTypeArguments[0]);
                        applyMethod.Invoke(modelBuilder, new object[] { Activator.CreateInstance(type) });
                    }
                }
            }
        }

        internal static void AddRestricDeleteConvention(this ModelBuilder modelBuilder)
        {
            IEnumerable<IMutableForeignKey> cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(f => !f.IsOwnership && f.DeleteBehavior == DeleteBehavior.Cascade);

            foreach(var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        internal static void AddPluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            Pluralizer pluralizer = new Pluralizer();
            
            foreach(IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                string tableName = entity.GetTableName();
                entity.SetTableName(pluralizer.Pluralize(tableName));
            }
        }
    }
}
