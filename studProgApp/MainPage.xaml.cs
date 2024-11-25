using studProgApp.ParsingStrategy;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;


namespace studProgApp
{

    

    public partial class MainPage : ContentPage
    {

        private string tempResultFile = "";
        private IXmlParsing currentStrategy;
        private string inputXmlPath;
        private readonly MainController _controller;
        private XDocument resultsXmlPath;

        public MainPage()
        {
            InitializeComponent();
            _controller = new MainController(new XmlProcessingService());
        }



        private async void OnSelectFileButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Завантаження даних через Controller
                var pickerData = await _controller.LoadDataAsync();

                // Завантажуємо дані у фільтри
                LoadPickers(pickerData);

                // Виводимо повідомлення про файл
                inputXmlPath = _controller.GetFilePath() ?? "Шлях відсутній";
                await DisplayAlert("Файл обрано", $"Вибрано файл: {inputXmlPath}", "OK");
                filters.IsVisible = true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"Помилка: {ex.Message}", "ОК");
            }
        }

        private Student GetSearchParameters()
        {
            return new Student
            {
                FullName = NamePicker.SelectedIndex != -1 ? NamePicker.SelectedItem.ToString() : null,
                Faculty = FacultyPicker.SelectedIndex != -1 ? FacultyPicker.SelectedItem.ToString() : null,
                Specialization = SpecializationPicker.SelectedIndex != -1 ? SpecializationPicker.SelectedItem.ToString() : null,
                Group = GroupPicker.SelectedIndex != -1 ? GroupPicker.SelectedItem.ToString() : null,
                Discipline = DisciplinePicker.SelectedIndex != -1 ? DisciplinePicker.SelectedItem.ToString() : null,
                MinMark = MinMarkPicker.SelectedIndex != -1 ? int.Parse(MinMarkPicker.SelectedItem.ToString()) : null,
                MaxMark = MaxMarkPicker.SelectedIndex != -1 ? int.Parse(MaxMarkPicker.SelectedItem.ToString()) : null
            };
        }

        private void LoadPickers(Dictionary<string, List<string>> pickerData)
        {
            NamePicker.ItemsSource = pickerData["Names"];
            FacultyPicker.ItemsSource = pickerData["Faculties"];
            SpecializationPicker.ItemsSource = pickerData["Specializations"];
            GroupPicker.ItemsSource = pickerData["Groups"];
            DisciplinePicker.ItemsSource = pickerData["Disciplines"];

            var grades = pickerData["Grades"];
            MinMarkPicker.ItemsSource = grades;
            MaxMarkPicker.ItemsSource = grades;

            MinMarkPicker.SelectedIndex = 0;
            MaxMarkPicker.SelectedIndex = grades.Count - 1;
        }






        private async void OnInfoButtonClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Інформація", "Ця програма дозволяє аналізувати XML-файли та перетворювати їх у HTML.", "ОК");
        }




        private void UpdateFilters()
        {
            NamePicker.SelectedItem = null;
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

        public void DisplayResults(XDocument xmlDoc)
        {
            try
            {
                // Завантаження студентів
                var students = xmlDoc.Descendants("Student"); // Виправлено регістр

                // Очищення старих результатів перед додаванням нових
                StudentsLayout.Children.Clear();

                // Перебір студентів і додавання їх даних
                foreach (var student in students)
                {
                    // Отримуємо атрибути студента
                    var fullName = student.Attribute("fullName")?.Value ?? "Невідомо";
                    var faculty = student.Attribute("faculty")?.Value ?? "Невідомо";
                    var specialization = student.Attribute("specialization")?.Value ?? "Невідомо";
                    var group = student.Attribute("group")?.Value ?? "Невідомо";

                    // Створення віконечка (Frame) для студента
                    var studentFrame = new Frame
                    {
                        Padding = new Thickness(10),
                        Margin = new Thickness(0, 10),
                        CornerRadius = 5,
                        BackgroundColor = Color.FromArgb("#1D1D1D"),
                        Content = new StackLayout
                        {
                            Children = {
                        new Label { Text = $"ПІБ: {fullName}", TextColor = Colors.White },
                        new Label { Text = $"Факультет: {faculty}", TextColor = Colors.White },
                        new Label { Text = $"Спеціальність: {specialization}", TextColor = Colors.White },
                        new Label { Text = $"Група: {group}", TextColor = Colors.White }
                    }
                        }
                    };

                    // Додаємо Frame до основного контейнера
                    StudentsLayout.Children.Add(studentFrame);

                    var successHeader = new Label
                    {
                        Text = "Успішність:",
                        TextColor = Colors.White,
                        FontAttributes = FontAttributes.Bold
                    };
                    ((StackLayout)studentFrame.Content).Children.Add(successHeader);

                    // Перебір дисциплін і додавання їх до результату
                    var disciplines = student.Elements("Discipline"); // Без "Disciplines", дисципліни зберігаються напряму
                    if (disciplines != null)
                    {
                        foreach (var discipline in disciplines)
                        {
                            var disciplineName = discipline.Attribute("Name")?.Value ?? "Невідомий предмет";
                            var grade = discipline.Attribute("Grade")?.Value ?? "0";

                            var disciplineLabel = new Label
                            {
                                Text = $"\t{disciplineName}: {grade}",
                                TextColor = Colors.White
                            };

                            // Додаємо дисципліну до Frame
                            ((StackLayout)studentFrame.Content).Children.Add(disciplineLabel);
                        }
                    }
                }

                // Робимо ScrollView видимим
                StudentsScrollView.IsVisible = true;
            }
            catch (Exception ex)
            {
                // Обробка помилок, наприклад, якщо файл не знайдений
                Console.WriteLine($"Помилка при завантаженні файлу: {ex.Message}");
            }
        }


        private void OnSearchButtonClicked(object sender, EventArgs e)
        {
            IXmlParsing? parser = null;

            if (StrategyPicker.SelectedIndex == 0)
            {
                parser = new SaxXmlSearch();
            }
            else if (StrategyPicker.SelectedIndex == 1)
            {
                parser = new DomXmlSearch();
            }
            else if (StrategyPicker.SelectedIndex == 2)
            {
                parser = new LinqToXmlSearch();
            }

            if (parser == null)
            {
                DisplayAlert("Помилка", "Оберіть стратегію парсингу.", "ОК");
                return;
            }

            if (string.IsNullOrWhiteSpace(inputXmlPath))
            {
                DisplayAlert("Помилка", "Шлях до файлу не вказано.", "ОК");
                return;
            }

            try
            {
                var parameters = GetSearchParameters();
                resultsXmlPath = parser.Search(inputXmlPath, parameters);
                DisplayResults(resultsXmlPath);
            }
            catch (Exception ex)
            {
                DisplayAlert("Помилка", $"Помилка під час пошуку: {ex.Message}", "ОК");
            }
        }


        public class HtmlTransformer
        {
            public void TransformXmlToHtml(XDocument xmlDocument, string xslFilePath, string outputHtmlPath)
            {
                try
                {
                    // Логування
                    Console.WriteLine($"Трансформація XML в HTML. Шлях до XSL: {xslFilePath}");

                    // Перетворення XDocument у XmlReader
                    using (var xmlReader = xmlDocument.CreateReader())
                    {
                        XslCompiledTransform xslTransform = new XslCompiledTransform();

                        // Перевірка існування XSL-файлу
                        if (!File.Exists(xslFilePath))
                        {
                            throw new FileNotFoundException($"XSL-файл не знайдено: {xslFilePath}");
                        }

                        xslTransform.Load(xslFilePath);

                        // Виконання трансформації
                        using (var writer = new StreamWriter(outputHtmlPath))
                        {
                            xslTransform.Transform(xmlReader, null, writer);
                        }
                    }

                    Console.WriteLine($"HTML-файл успішно створено за адресою: {outputHtmlPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Сталася помилка при трансформації: {ex.Message}");
                    throw; // Повторне кидання винятку
                }
            }
        }




        private void OnTransformToHtmlClicked(object sender, EventArgs e)
        {
            try
            {
                // Відносний шлях до XSL
                string xslFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "template.xsl");

                // Шлях для збереження HTML
                string outputHtmlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "searchResults.html");

                // Перевірка результатів
                if (resultsXmlPath == null || !resultsXmlPath.Root.HasElements)
                {
                    DisplayAlert("Помилка", "Результати пошуку відсутні або порожні!", "OK");
                    return;
                }


                // Виконання трансформації
                var transformer = new HtmlTransformer();
                transformer.TransformXmlToHtml(resultsXmlPath, xslFilePath, outputHtmlPath);



                DisplayAlert("Результат", $"HTML-файл створено: {outputHtmlPath}", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlert("Помилка", $"Не вдалося створити HTML-файл: {ex.Message}", "OK");
            }
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
