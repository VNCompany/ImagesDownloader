using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ImagesDownloader.ViewModels;

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

            EventManager.AddHandler(DataContext, HandleViewModelEvent);
            tbUrl.KeyDown += TbUrl_KeyDown;
        }

        private void TbUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext is CollectionViewModel collectionVM)
            {
                tbUrl.GetBindingExpression(TextBox.TextProperty).UpdateSource();

                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    if (collectionVM.Analyze.CanExecute(true))
                        collectionVM.Analyze.Execute(true);
                }
                else if (collectionVM.Analyze.CanExecute(false))
                    collectionVM.Analyze.Execute(false);
            }
        }

        private void HandleViewModelEvent(object? sender, ViewModelEventArgs args)
        {
            switch (args.EventId)
            {
                case 1:
                    btnAdd.Focus();
                    break;
            }
        }
    }
}
