using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Services.Implementations
{
    public class APIService : IAPIService
    {
        public string APISimpleInfo()
        {
            return "This is a simple project management API.";
        }
    }
}
