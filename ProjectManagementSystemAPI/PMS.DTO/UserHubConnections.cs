using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.DTO
{
    public class UserHubConnections
    {
        public int UserId { get; set; }
        public List<string> Connections { get; set; }
    }
}
