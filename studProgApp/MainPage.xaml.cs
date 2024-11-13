using System.Xml;


namespace studProgApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }


        private async void OnInfoButtonClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Інформація", "Ця програма дозволяє аналізувати XML-файли та перетворювати їх у HTML.", "ОК");
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
