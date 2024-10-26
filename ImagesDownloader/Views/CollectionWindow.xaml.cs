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
                var parameter = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
                var vm = (DataContext as CollectionVM) ?? throw new InvalidCastException();
                tbUrl.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                
                vm.TrySetHtml(vm.Url).ContinueWith(t =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (vm.HtmlParse.CanExecute(parameter))
                            vm.HtmlParse.Execute(parameter);
                    });
                });
            }
        }
    }
}
