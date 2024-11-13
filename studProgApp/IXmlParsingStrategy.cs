using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studProgApp
{
    internal interface IXmlParsingStrategy
    {
        void Parse(string filePath);
    }
}
    