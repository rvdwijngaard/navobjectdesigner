using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils.Exceptions
{
    public class ObjectDesignerDeadlockException : Exception
    {
        public ObjectDesignerDeadlockException(string source, string message) : base(message) { this.Source = source; }
    }
}
