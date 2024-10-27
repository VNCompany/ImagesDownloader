using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using ImagesDownloader.ViewModels;
using System.Windows.Data;

namespace ImagesDownloader.Views
{
    /// <summary>
    /// Логика взаимодействия для CollectionWindow.xaml
    /// </summary>
    public partial class CollectionWindow : Window
    {
        public CollectionWindow()
        {
            InitializeComponent();

            tbUrl.KeyDown += TbUrl_KeyDown;
            tbUrl.Focus();
        }

        private void TbUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var vm = (DataContext as CollectionViewModel) ?? throw new InvalidCastException();
                tbUrl.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                bool isControlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
                if (vm.LoadHtml.CanExecute(isControlPressed))
                    vm.LoadHtml.Execute(isControlPressed);
            }
        }
    }
}
