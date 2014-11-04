using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TI.Nav.Utils.Interfaces;

namespace TI.Nav.Utils
{
    public class ExportRequest
    {
        private List<ExportFilter> mFilters = new List<ExportFilter>();
                
        public IEnumerable<ExportFilter> Filters
        {
            get { return mFilters; }
            set { mFilters = value == null ? new List<ExportFilter>() : new List<ExportFilter>(value); }
        }            
    }

    public class ExportResponse : IActionResponse
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
        public bool Successful { get; set; }
        public string[] Files { get; set; }
    }
}
