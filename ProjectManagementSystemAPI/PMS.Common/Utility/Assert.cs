using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Common.Utility
{
    public static class Assert
    {
        public static void NotNull<T>(T obj, string name) where T : class
        {
            if(obj is null)
            {
                throw new ArgumentNullException($"{name}:{typeof(T)}");
            }
        }
    }
}
