using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace ImagesDownloader.Controls
{
    /// <summary>
    /// Логика взаимодействия для PathSelector.xaml
    /// </summary>
    public partial class PathSelector : UserControl
    {
        public static readonly DependencyProperty SelectedPathProperty = DependencyProperty.Register(
            "SelectedPath", typeof(string), typeof(PathSelector), 
            new PropertyMetadata(string.Empty, SelectedPath_PropertyChanged));

        public string SelectedPath
        {
            get => (string)GetValue(SelectedPathProperty);
            set => SetValue(SelectedPathProperty, value);
        }

        public PathSelector()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFolderDialog();
            dlg.Multiselect = false;
            if (!string.IsNullOrEmpty(SelectedPath) && Directory.Exists(SelectedPath))
                dlg.InitialDirectory = SelectedPath;

            if (dlg.ShowDialog() == true)
                SelectedPath = dlg.FolderName;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TB.Text != SelectedPath)
            {
                if (!ValidatePathValue(TB.Text))
                {
                    TB.Foreground = Brushes.Red;
                    return;
                }
                else if (TB.Foreground == Brushes.Red)
                    TB.Foreground = Brushes.Black;
                SelectedPath = TB.Text;
            }
        }

        private static void SelectedPath_PropertyChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            string newValue = (string)e.NewValue;
            if (ValidatePathValue(newValue) && s is PathSelector control && newValue != control.TB.Text)
                control.TB.Text = (string)e.NewValue;
        }


        private static char[] _invalidPathChars = [.. Path.GetInvalidPathChars(), '*', '?', '"', '<', '>'];
        private static bool ValidatePathValue(string input) => input.IndexOfAny(_invalidPathChars) == -1;
    }
}
