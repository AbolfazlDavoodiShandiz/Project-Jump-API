using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Entities
{
    public abstract class BaseEntity : IDbEntity
    {
        public int Id { get; set; }
    }
}
