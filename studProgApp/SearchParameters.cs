using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studProgApp
{
    public class SearchParameters
    {
        public string Name { get; set; }
        public string Faculty { get; set; }
        public string Specialization { get; set; }
        public string Group { get; set; }
        public string Discipline { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(Name) &&
                   string.IsNullOrWhiteSpace(Faculty) &&
                   string.IsNullOrWhiteSpace(Specialization) &&
                   string.IsNullOrWhiteSpace(Group) &&
                   string.IsNullOrWhiteSpace(Discipline);
        }
    }

}
