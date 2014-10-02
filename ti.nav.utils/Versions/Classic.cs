using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils
{
    public class Classic : IObjectDesigner
    {
        public Classic(IObjectDesignerRequest request)
        {

        }

        public void Export(string filter, string fileName)
        {
            throw new NotImplementedException();
        }

        public ImportResponse Import(ImportRequest request)
        {
            throw new NotImplementedException();
        }

        public void Compile()
        {
            throw new NotImplementedException();
        }
    }
}
