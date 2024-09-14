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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class TestCollection : List<Tuple<string, bool>>
    {
        public TestCollection() : base([
            new("https://vk.com", true),
            new("https://vk.com/eorigjeoirg/erg/wef/w/eg/ewr/ge/rwefwe/fgw", false),
            new("https://vk.com", false),
            new("https://vk.com", true),
        ]) { }
    }

    public class TestMainCollection : List<Tuple<string, double, string, int, int>>
    {
        public TestMainCollection() : base([
            new("https://vk.com/eorigjeoirg/erg/wef/w/eg/ewr/ge/rwefwe/fgw", 70.32, "Downloading", 12, 35),
            new("https://vk.com/sfcc", 100, "Idle", 16, 16),
            new("https://vk.com", 0, "Idle", 0, 105),
        ]) { }
    }
}
