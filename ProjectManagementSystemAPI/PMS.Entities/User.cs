using Microsoft.AspNetCore.Identity;
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
    }
}
