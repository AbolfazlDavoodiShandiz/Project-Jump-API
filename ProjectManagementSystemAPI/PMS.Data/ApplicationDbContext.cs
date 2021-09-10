using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PMS.Data.ModelBuilderExtensions;
using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int>
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var entitiesAssembly = typeof(IDbEntity).Assembly;

            modelBuilder.RegisterAllEntities<IDbEntity>(entitiesAssembly);
            modelBuilder.RegisterAllEntityConfigurations(entitiesAssembly);
            modelBuilder.AddRestricDeleteConvention();
            modelBuilder.AddPluralizingTableNameConvention();
        }
    }
}
