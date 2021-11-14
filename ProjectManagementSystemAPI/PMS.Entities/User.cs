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
        public string LastCreatedToken { get; set; }
        public DateTime? LastTokenExpireDate { get; set; }

        public ICollection<Project> Projects { get; set; }
        public ICollection<ProjectTask> Tasks { get; set; }
        public ICollection<UserTask> UserTasks { get; set; }
        public ICollection<ProjectMember> ProjectMembers { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany(u => u.Projects).WithOne(p => p.Owner).HasForeignKey(p => p.OwnerId);
            builder.HasMany(u => u.Tasks).WithOne(pt => pt.Owner).HasForeignKey(pt => pt.OwnerId);
            builder.HasMany(u => u.UserTasks).WithOne(ut => ut.User).HasForeignKey(ut => ut.UserId);
            builder.HasMany(u => u.ProjectMembers).WithOne(pt => pt.User).HasForeignKey(pt => pt.UserId);
            builder.HasMany(u => u.Notifications).WithOne(n => n.RecieverUser).HasForeignKey(n => n.RecieverUserId);
        }
    }
}
