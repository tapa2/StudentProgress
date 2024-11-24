using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace studProgApp.ParsingStrategy
{
    public class SaxXmlSearch : IXmlParsing
    {
        public XDocument Search(string xmlPath, Student parameters)
        {
            if (string.IsNullOrWhiteSpace(xmlPath))
                throw new ArgumentNullException(nameof(xmlPath), "Шлях до XML файлу не може бути порожнім.");

            // Створюємо результуючий XML-документ
            XDocument result = new XDocument(new XElement("Students"));

            using (XmlReader reader = XmlReader.Create(xmlPath))
            {
                XElement rootElement = result.Root;

                while (reader.Read())
                {
                    // Знаходимо вузол Student
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Student")
                    {
                        XElement studentElement = XElement.ReadFrom(reader) as XElement;

                        if (studentElement != null)
                        {
                            // Перевіряємо, чи відповідає студент переданим параметрам
                            bool studentMatches = true;

                            if (!string.IsNullOrWhiteSpace(parameters.FullName))
                            {
                                studentMatches &= (string)studentElement.Attribute("fullName") == parameters.FullName;
                            }

                            if (!string.IsNullOrWhiteSpace(parameters.Faculty))
                            {
                                studentMatches &= (string)studentElement.Attribute("faculty") == parameters.Faculty;
                            }

                            if (!string.IsNullOrWhiteSpace(parameters.Specialization))
                            {
                                studentMatches &= (string)studentElement.Attribute("specialization") == parameters.Specialization;
                            }

                            if (!string.IsNullOrWhiteSpace(parameters.Group))
                            {
                                studentMatches &= (string)studentElement.Attribute("group") == parameters.Group;
                            }

                            if (studentMatches)
                            {
                                // Перевіряємо, чи всі дисципліни задовольняють умови
                                var disciplines = studentElement.Elements("Discipline");

                                bool allDisciplinesMatch = disciplines.All(d =>
                                    (!parameters.MinMark.HasValue || (int)d.Attribute("Grade") >= parameters.MinMark.Value) &&
                                    (!parameters.MaxMark.HasValue || (int)d.Attribute("Grade") <= parameters.MaxMark.Value));

                                // Якщо всі дисципліни відповідають критеріям, додаємо студента
                                if (allDisciplinesMatch)
                                {
                                    XElement filteredStudent = new XElement("Student",
                                        studentElement.Attributes(),
                                        disciplines);
                                    rootElement.Add(filteredStudent);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
