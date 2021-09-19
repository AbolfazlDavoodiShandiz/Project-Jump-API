using System;
using System.ComponentModel.DataAnnotations;

namespace PMS.DTO
{
    public class ProjectRegistrationDTO
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime DeadlineDate { get; set; }
    }
}
