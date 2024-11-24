using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace studProgApp.ParsingStrategy
{
    public interface IXmlParsing
    {
        XDocument Search(string xmlPath, Student parameters);
    }

}
