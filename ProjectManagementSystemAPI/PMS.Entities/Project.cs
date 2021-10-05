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
            Finished = false;
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; internal set; }
        public DateTime DeadlineDate { get; set; }
        public bool Finished { get; set; }

        public User Owner { get; set; }
        public int OwnerId { get; set; }

        public ICollection<ProjectTask> Tasks { get; set; }
        public ICollection<ProjectMember> Members { get; set; }
    }

    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.Property(p => p.Title).IsRequired();
            builder.Property(p => p.DeadlineDate).IsRequired();
            builder.HasMany(p => p.Tasks).WithOne(t => t.Project).HasForeignKey(t => t.ProjectId);
            builder.HasMany(p => p.Members).WithOne(t => t.Project).HasForeignKey(t => t.ProjectId);
        }
    }
}
