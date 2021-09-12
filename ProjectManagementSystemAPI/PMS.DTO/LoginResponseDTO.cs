using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.DTO
{
    public class LoginResponseDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpirationDate { get; set; }
    }
}
