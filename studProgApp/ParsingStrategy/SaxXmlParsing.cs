using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace studProgApp.ParsingStrategy
{
    public class SaxParsing : IXmlParsing
    {
        public void Parse(string xmlPath, SearchParameters parameters)
        {
            using (XmlReader reader = XmlReader.Create(xmlPath))
            {
                Console.WriteLine("SAX Parsing Started:");

                bool studentMatch = false;

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "student")
                    {
                        string fullName = reader.GetAttribute("fullName");
                        string faculty = reader.GetAttribute("faculty");
                        string department = reader.GetAttribute("department");
                        string specialization = reader.GetAttribute("specialization");
                        string group = reader.GetAttribute("group");

                        // Перевірка відповідності критеріям пошуку
                        studentMatch = (string.IsNullOrEmpty(parameters.Name) || fullName.Contains(parameters.Name)) &&
                                       (string.IsNullOrEmpty(parameters.Faculty) || faculty == parameters.Faculty) &&
                                       (string.IsNullOrEmpty(parameters.Specialization) || specialization == parameters.Specialization) &&
                                       (string.IsNullOrEmpty(parameters.Group) || group == parameters.Group);

                        if (studentMatch)
                        {
                            Console.WriteLine($"Student: {fullName}, Faculty: {faculty}, Department: {department}, Specialization: {specialization}, Group: {group}");
                        }
                    }

                    if (studentMatch && reader.NodeType == XmlNodeType.Element && reader.Name == "Discipline")
                    {
                        string disciplineName = reader.GetAttribute("Name");
                        string grade = reader.GetAttribute("Grade");
                        Console.WriteLine($"  Discipline: {disciplineName}, Grade: {grade}");
                    }
                }

                Console.WriteLine("SAX Parsing Completed.");
            }
        }
    }
}
