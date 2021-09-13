using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Entities
{
    public class User : IdentityUser<int>, IDbEntity
    {
        public User()
        {
            IsActive = true;
            RegisterDate = DateTime.Now;
        }

        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public ICollection<Project> Projects { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany(u => u.Projects).WithOne(p => p.User).HasForeignKey(p => p.UserId);
        }
    }
}
