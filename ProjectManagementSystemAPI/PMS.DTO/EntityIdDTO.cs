using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.DTO
{
    public class EntityIdDTO
    {
        [Required]
        public int Id { get; set; }
    }
}
