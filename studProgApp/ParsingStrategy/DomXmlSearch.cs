using System;
using System.Xml;
using System.Xml.Linq;

namespace studProgApp.ParsingStrategy
{
    public class DomXmlSearch : IXmlParsing
    {
        public XDocument Search(string xmlPath, Student parameters)
        {
            if (string.IsNullOrWhiteSpace(xmlPath))
                throw new ArgumentNullException(nameof(xmlPath), "Шлях до XML файлу не може бути порожнім.");

            // Завантаження XML у пам'ять
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            // Створюємо результуючий XML-документ
            XDocument result = new XDocument(new XElement("Students"));

            // Отримуємо всі вузли Student
            XmlNodeList students = xmlDoc.SelectNodes("//Student");

            foreach (XmlNode student in students)
            {
                bool studentMatches = true;

                // Перевіряємо атрибути студента
                if (!string.IsNullOrWhiteSpace(parameters.FullName))
                {
                    studentMatches &= student.Attributes["fullName"]?.InnerText == parameters.FullName;
                }
                if (!string.IsNullOrWhiteSpace(parameters.Faculty))
                {
                    studentMatches &= student.Attributes["faculty"]?.InnerText == parameters.Faculty;
                }
                if (!string.IsNullOrWhiteSpace(parameters.Specialization))
                {
                    studentMatches &= student.Attributes["specialization"]?.InnerText == parameters.Specialization;
                }
                if (!string.IsNullOrWhiteSpace(parameters.Group))
                {
                    studentMatches &= student.Attributes["group"]?.InnerText == parameters.Group;
                }

                // Перевіряємо дисципліни
                if (studentMatches)
                {
                    XmlNodeList disciplines = student.SelectNodes("Discipline");

                    bool allDisciplinesMatch = true;
                    foreach (XmlNode discipline in disciplines)
                    {
                        int grade = int.Parse(discipline.Attributes["Grade"].InnerText);

                        if (parameters.MinMark.HasValue && grade < parameters.MinMark.Value)
                        {
                            allDisciplinesMatch = false;
                            break;
                        }
                        if (parameters.MaxMark.HasValue && grade > parameters.MaxMark.Value)
                        {
                            allDisciplinesMatch = false;
                            break;
                        }
                    }

                    // Додаємо студента до результату, якщо всі дисципліни відповідають критеріям
                    if (allDisciplinesMatch)
                    {
                        XElement studentElement = new XElement("Student",
                            new XAttribute("fullName", student.Attributes["fullName"]?.InnerText),
                            new XAttribute("faculty", student.Attributes["faculty"]?.InnerText),
                            new XAttribute("specialization", student.Attributes["specialization"]?.InnerText),
                            new XAttribute("group", student.Attributes["group"]?.InnerText));

                        foreach (XmlNode discipline in disciplines)
                        {
                            studentElement.Add(new XElement("Discipline",
                                new XAttribute("Name", discipline.Attributes["Name"]?.InnerText),
                                new XAttribute("Grade", discipline.Attributes["Grade"]?.InnerText)));
                        }

                        result.Root.Add(studentElement);
                    }
                }
            }

            return result;
        }
    }
}
