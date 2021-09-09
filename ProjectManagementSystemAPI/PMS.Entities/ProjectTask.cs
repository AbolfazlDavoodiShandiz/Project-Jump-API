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
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; internal set; }
        public DateTime DeadlineDate { get; set; }

        public Project Project { get; set; }
        public int ProjectId { get; set; }
    }

    public class TaskConfiguration : IEntityTypeConfiguration<ProjectTask>
    {
        public void Configure(EntityTypeBuilder<ProjectTask> builder)
        {
            builder.Property(t => t.Title).IsRequired();
            builder.Property(t => t.Description).IsRequired();
            builder.Property(t => t.DeadlineDate).IsRequired();
        }
    }
}
