using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TI.Nav.Utils
{
    public interface IObjectDesigner
    {
        void Export(string filter, string fileName);                
        ImportResponse Import(ImportRequest request);
        void Compile();        
    }
}
