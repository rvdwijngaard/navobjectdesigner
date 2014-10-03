using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils.Exceptions
{
    public class CompilationException : Exception
    {
        public CompilationException(string source, string message) : base(message) { this.Source = source; }
    }
}
