using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLib.Exceptions
{
    public class UserExceptions : Exception
    {
        public UserExceptions() { }
        public UserExceptions(string message) : base(message) { }
        public UserExceptions(string message, Exception inner) : base(message, inner) { }
    }
}
