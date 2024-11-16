using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studProgApp.ParsingStrategy
{
    public interface IXmlParsing
    {
        void Parse(string xmlPath, SearchParameters parameters);
    }

}
