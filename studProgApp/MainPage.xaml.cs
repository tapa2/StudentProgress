using studProgApp.ParsingStrategy;
using System.Xml;
using System.Xml.Linq;


namespace studProgApp
{
    public partial class MainPage : ContentPage
    {
        private IXmlParsing currentStrategy;
        private string xmlPath;

        public MainPage()
        {
            InitializeComponent();
        }


        private async void OnSelectFileButtonClicked(object sender, EventArgs e)
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

            if (result != null)
            {
                await DisplayAlert("Файл обрано", $"Вибрано файл: {result.FileName}", "ОК");
                xmlPath = result.FileName;
                XDocument xmlDoc = XDocument.Load(result.FullPath);
                LoadPickers(xmlDoc);
                filters.IsVisible = true;

            }
        }

        private async void OnInfoButtonClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Інформація", "Ця програма дозволяє аналізувати XML-файли та перетворювати їх у HTML.", "ОК");
        }

        private void LoadPickers(XDocument xmlDoc)
        {
            // Отримуємо факультети
            var facultyNames = xmlDoc.Descendants("student")
                                     .Attributes("faculty")
                                     .Select(x => x.Value)
                                     .Distinct()
                                     .ToList();
            FacultyPicker.Items.Clear();
            foreach (var faculty in facultyNames)
            {
                FacultyPicker.Items.Add(faculty);
            }

            // Отримуємо спеціальності
            var specializationNames = xmlDoc.Descendants("student")
                                             .Attributes("specialization")
                                             .Select(x => x.Value)
                                             .Distinct()
                                             .ToList();
            SpecializationPicker.Items.Clear();
            foreach (var specialization in specializationNames)
            {
                SpecializationPicker.Items.Add(specialization);
            }

            // Отримуємо групи
            var groupNames = xmlDoc.Descendants("student")
                                   .Attributes("group")
                                   .Select(x => x.Value)
                                   .Distinct()
                                   .ToList();
            GroupPicker.Items.Clear();
            foreach (var group in groupNames)
            {
                GroupPicker.Items.Add(group);
            }

            // Отримуємо дисципліни
            var disciplineNames = xmlDoc.Descendants("student")
                                        .Descendants("Disciplines")
                                        .Descendants("Discipline")
                                        .Attributes("Name")
                                        .Select(x => x.Value)
                                        .Distinct()
                                        .ToList();
            DisciplinePicker.Items.Clear();
            foreach (var discipline in disciplineNames)
            {
                DisciplinePicker.Items.Add(discipline);
            }
        }

        private SearchParameters GetSearchParameters()
        {
            return new SearchParameters
            {
                Name = string.IsNullOrWhiteSpace(NameEntry.Text) ? null : NameEntry.Text,
                Faculty = FacultyPicker.SelectedItem?.ToString(),
                Specialization = SpecializationPicker.SelectedItem?.ToString(),
                Group = GroupPicker.SelectedItem?.ToString(),
                Discipline = DisciplinePicker.SelectedItem?.ToString()
            };
        }


        private void OnSearchButtonClicked(object sender, EventArgs e)
        {
            if (currentStrategy == null)
            {
                DisplayAlert("Помилка", "Оберіть стратегію обробки!", "OK");
                return;
            }

            var parameters = GetSearchParameters();

            try
            {
                currentStrategy.Parse(xmlPath, parameters);
            }
            catch (Exception ex)
            {
                DisplayAlert("Помилка", $"Сталася помилка під час обробки: {ex.Message}", "OK");
            }
        }







        private void UpdateFilters()
        {
            NameEntry.Text = string.Empty;
            FacultyPicker.SelectedItem = null;
            SpecializationPicker.SelectedItem = null;
            GroupPicker.SelectedItem = null;
            DisciplinePicker.SelectedItem = null;
            StrategyPicker.SelectedItem = "SAX";
        }

        private void OnUpdateFiltrsBtnClicked(object sender, EventArgs e)
        {
            UpdateFilters();
        }


        private async void OnQuitButtonClicked(object sender, EventArgs e)
        {
            bool confirmExit = await DisplayAlert("Підтвердження", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
            if (confirmExit)
            {
                Application.Current.Quit();
            }
        }
    }

}
