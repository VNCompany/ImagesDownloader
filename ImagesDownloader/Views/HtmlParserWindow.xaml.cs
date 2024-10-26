using System.Windows;

namespace ImagesDownloader.Views
{
    /// <summary>
    /// Логика взаимодействия для HtmlParserWindow.xaml
    /// </summary>
    public partial class HtmlParserWindow : Window
    {
        public HtmlParserWindow()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
