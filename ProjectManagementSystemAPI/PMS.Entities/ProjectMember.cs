using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Entities
{
    public class ProjectMember : BaseEntity
    {
        public Project Project { get; set; }
        public int ProjectId { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }
    }

    public class ProjectTeamConfiguration : IEntityTypeConfiguration<ProjectMember>
    {
        public void Configure(EntityTypeBuilder<ProjectMember> builder)
        {

        }
    }
}
