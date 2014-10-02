using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils
{
    public class ImportRequest
    {
        private List<string> mFiles = new List<string>();
        
        public List<string> Files
        {
            get { return mFiles; }
            set
            {
                mFiles = value != null ? new List<string>(value) : new List<string>();

            }
        }        
    }

    public class ImportResponse
    {
        private List<string> mFiles = new List<string>();
        private List<Exception> mExceptions = new List<Exception>();
       
        public List<Exception> Exceptions
        {
            get { return mExceptions; }
            set
            {
                mExceptions = value != null ? new List<Exception>(value) : new List<Exception>();

            }
        }

        public Boolean Result { get; set; }        
    }
}
