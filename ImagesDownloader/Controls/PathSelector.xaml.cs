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
            TB.TextChanged += TB_TextChanged;
        }

        private void TB_TextChanged(object sender, TextChangedEventArgs e)
        {
            SelectedPath = TB.Text;
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

        private static void SelectedPath_PropertyChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var pathSelector = (PathSelector)s!;
            string newValue = (string)e.NewValue;
            if (newValue != pathSelector.TB.Text)
            {
                if (ValidatePathValue(newValue))
                {
                    // TODO
                }    
            }
        }

        private static char[] _invalidPathChars = [.. Path.GetInvalidPathChars(), '*', '?', '"', '<', '>'];
        private static bool ValidatePathValue(string input) => input.IndexOfAny(_invalidPathChars) == -1;
    }
}
