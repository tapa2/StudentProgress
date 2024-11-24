using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace studProgApp
{
    public class XmlProcessingService
    {
        public string? InputXmlPath { get; private set; }

        public XmlProcessingService() { }

        public async Task<XDocument> LoadXmlFileAsync()
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>> {
                { DevicePlatform.Android, new[] { ".xml" } },
                { DevicePlatform.iOS, new[] { ".xml" } },
                { DevicePlatform.WinUI, new[] { ".xml" } },
            }),
                PickerTitle = "Виберіть XML файл"
            });

            if (result == null)
                throw new OperationCanceledException("Файл не було вибрано.");

            InputXmlPath = result.FullPath;

            if (!File.Exists(InputXmlPath))
                throw new FileNotFoundException($"Файл {InputXmlPath} не знайдено!");

            return XDocument.Load(InputXmlPath);
        }

        public Dictionary<string, List<string>> ExtractPickerData(XDocument xmlDoc)
        {
            return new Dictionary<string, List<string>>
            {
                ["Names"] = xmlDoc.Descendants("Student")
                                  .Attributes("fullName")
                                  .Select(x => x.Value)
                                  .Distinct()
                                  .ToList(),
                ["Faculties"] = xmlDoc.Descendants("Student")
                                      .Attributes("faculty")
                                      .Select(x => x.Value)
                                      .Distinct()
                                      .ToList(),
                ["Specializations"] = xmlDoc.Descendants("Student")
                                            .Attributes("specialization")
                                            .Select(x => x.Value)
                                            .Distinct()
                                            .ToList(),
                ["Groups"] = xmlDoc.Descendants("Student")
                                   .Attributes("group")
                                   .Select(x => x.Value)
                                   .Distinct()
                                   .ToList(),
                ["Disciplines"] = xmlDoc.Descendants("Discipline")
                                        .Attributes("Name")
                                        .Select(x => x.Value)
                                        .Distinct()
                                        .ToList(),
                ["Grades"] = xmlDoc.Descendants("Discipline")
                                   .Attributes("Grade")
                                   .Select(x => x.Value)
                                   .Distinct()
                                   .OrderBy(x => int.Parse(x))
                                   .ToList()
            };
        }
    }

}
