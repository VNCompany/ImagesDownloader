using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImagesDownloader.Views
{
    /// <summary>
    /// Логика взаимодействия для AddCollectionWindow.xaml
    /// </summary>
    public partial class AddCollectionWindow : Window
    {
        public AddCollectionWindow()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var tb = (sender as TextBox)!;
                tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }
}
