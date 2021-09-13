using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Entities
{
    public class Project : BaseEntity
    {
        public Project()
        {
            CreatedDate = DateTime.Now;
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; internal set; }
        public DateTime DeadlineDate { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }

        public ICollection<ProjectTask> Tasks { get; set; }
    }

    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.Property(p => p.Title).IsRequired();
            builder.Property(p => p.DeadlineDate).IsRequired();
            builder.HasMany(p => p.Tasks).WithOne(t => t.Project).HasForeignKey(t => t.ProjectId);
        }
    }
}
