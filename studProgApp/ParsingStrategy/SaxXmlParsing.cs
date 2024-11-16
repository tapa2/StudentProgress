using System.Xml;

namespace studProgApp.ParsingStrategy
{
    public class SaxXmlParsing : IXmlParsing
    {
        private string resPath = "resPath.xml";
        private readonly string _tempXmlFilePath = Path.Combine(Path.GetTempPath(), "resPath.xml");

        public string TempXmlFilePath => _tempXmlFilePath;

        public void Parse(string xmlPath, Student parameters)
        {
            if (string.IsNullOrEmpty(xmlPath))
                throw new ArgumentException("XML path cannot be null or empty.", nameof(xmlPath));

            using (XmlReader reader = XmlReader.Create(xmlPath))
            using (XmlWriter writer = XmlWriter.Create(_tempXmlFilePath, new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Students");

                bool studentMatch = false;

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "student")
                    {
                        string fullName = reader.GetAttribute("fullName") ?? "Unknown";
                        string faculty = reader.GetAttribute("faculty") ?? "Unknown";
                        string department = reader.GetAttribute("department") ?? "Unknown";
                        string specialization = reader.GetAttribute("specialization") ?? "Unknown";
                        string group = reader.GetAttribute("group") ?? "Unknown";

                        studentMatch = (string.IsNullOrEmpty(parameters.Name) || fullName.Contains(parameters.Name)) &&
                                       (string.IsNullOrEmpty(parameters.Faculty) || faculty == parameters.Faculty) &&
                                       (string.IsNullOrEmpty(parameters.Specialization) || specialization == parameters.Specialization) &&
                                       (string.IsNullOrEmpty(parameters.Group) || group == parameters.Group);

                        if (studentMatch)
                        {
                            writer.WriteStartElement("Student");
                            writer.WriteAttributeString("fullName", fullName);
                            writer.WriteAttributeString("faculty", faculty);
                            writer.WriteAttributeString("department", department);
                            writer.WriteAttributeString("specialization", specialization);
                            writer.WriteAttributeString("group", group);
                        }
                    }

                    if (studentMatch && reader.NodeType == XmlNodeType.Element && reader.Name == "Discipline")
                    {
                        string disciplineName = reader.GetAttribute("Name") ?? "Unknown";
                        string grade = reader.GetAttribute("Grade") ?? "Unknown";

                        writer.WriteStartElement("Discipline");
                        writer.WriteAttributeString("Name", disciplineName);
                        writer.WriteAttributeString("Grade", grade);
                        writer.WriteEndElement();
                    }

                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "student" && studentMatch)
                    {
                        writer.WriteEndElement(); // Закриваємо тег Student
                        studentMatch = false;
                    }
                }

                writer.WriteEndElement(); // Закриваємо тег Students
                writer.WriteEndDocument();
            }
        }
    }
}
