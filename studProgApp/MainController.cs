using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studProgApp
{
    public class MainController
    {
        private readonly XmlProcessingService _xmlService;

        public MainController(XmlProcessingService xmlService)
        {
            _xmlService = xmlService;
        }

        public async Task<Dictionary<string, List<string>>> LoadDataAsync()
        {
            var xmlDoc = await _xmlService.LoadXmlFileAsync();
            return _xmlService.ExtractPickerData(xmlDoc);
        }

        public string? GetFilePath()
        {
            return _xmlService.InputXmlPath;
        }
    }

}
