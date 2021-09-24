using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Entities
{
    public class ProjectTask : BaseEntity
    {
        public ProjectTask()
        {
            CreatedDate = DateTime.Now;
            Done = false;
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; internal set; }
        public DateTime DeadlineDate { get; set; }
        public DateTime? CompleteDate { get; set; }
        public bool Done { get; set; }

        public User Owner { get; set; }
        public int OwnerId { get; set; }

        public Project Project { get; set; }
        public int ProjectId { get; set; }

        public ICollection<UserTask> UserTasks { get; set; }
    }

    public class TaskConfiguration : IEntityTypeConfiguration<ProjectTask>
    {
        public void Configure(EntityTypeBuilder<ProjectTask> builder)
        {
            builder.Property(t => t.Title).IsRequired();
            builder.Property(t => t.Description).IsRequired();
            builder.Property(t => t.DeadlineDate).IsRequired();
            builder.HasMany(pt => pt.UserTasks).WithOne(ut => ut.ProjectTask).HasForeignKey(ut => ut.TaskId);
        }
    }
}
