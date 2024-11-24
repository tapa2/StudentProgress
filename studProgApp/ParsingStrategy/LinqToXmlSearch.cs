using System;
using System.Linq;
using System.Xml.Linq;

namespace studProgApp.ParsingStrategy
{
    public class LinqToXmlSearch : IXmlParsing
    {
        public XDocument Search(string xmlPath, Student parameters)
        {
            if (string.IsNullOrWhiteSpace(xmlPath))
                throw new ArgumentNullException(nameof(xmlPath), "Шлях до XML файлу не може бути порожнім.");

            // Завантаження XML-файлу
            XDocument xmlDoc = XDocument.Load(xmlPath);

            // Створюємо результуючий XML-документ
            XDocument result = new XDocument(new XElement("Students"));

            // Фільтруємо студентів
            var filteredStudents = xmlDoc.Descendants("Student").Where(student =>
            {
                bool studentMatches = true;

                // Перевірка атрибутів студента
                if (!string.IsNullOrWhiteSpace(parameters.FullName))
                {
                    studentMatches &= (string)student.Attribute("fullName") == parameters.FullName;
                }
                if (!string.IsNullOrWhiteSpace(parameters.Faculty))
                {
                    studentMatches &= (string)student.Attribute("faculty") == parameters.Faculty;
                }
                if (!string.IsNullOrWhiteSpace(parameters.Specialization))
                {
                    studentMatches &= (string)student.Attribute("specialization") == parameters.Specialization;
                }
                if (!string.IsNullOrWhiteSpace(parameters.Group))
                {
                    studentMatches &= (string)student.Attribute("group") == parameters.Group;
                }

                // Перевірка дисциплін
                var disciplines = student.Elements("Discipline");
                if (disciplines.Any())
                {
                    studentMatches &= disciplines.All(d =>
                        (!parameters.MinMark.HasValue || (int)d.Attribute("Grade") >= parameters.MinMark.Value) &&
                        (!parameters.MaxMark.HasValue || (int)d.Attribute("Grade") <= parameters.MaxMark.Value));
                }

                return studentMatches;
            });

            // Додаємо відфільтрованих студентів до результату
            foreach (var student in filteredStudents)
            {
                result.Root.Add(new XElement("Student",
                    student.Attributes(),
                    student.Elements("Discipline")));
            }

            return result;
        }
    }
}
