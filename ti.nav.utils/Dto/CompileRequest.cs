using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils
{
    public class CompileRequest
    {
        public string Filter { get; set; }
    }

    public class CompileResponse
    {
        private List<Exception> mExceptions = new List<Exception>();

        public List<Exception> Exceptions
        {
            get { return mExceptions; }
            set
            {
                mExceptions = value != null ? new List<Exception>(value) : new List<Exception>();

            }
        }

        public bool Succesful { get; set; }
    }
}
