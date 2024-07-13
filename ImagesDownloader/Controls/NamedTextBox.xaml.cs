using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ImagesDownloader.Controls
{
    /// <summary>
    /// Логика взаимодействия для NamedTextBox.xaml
    /// </summary>
    public partial class NamedTextBox : UserControl
    {
        private bool _outerMode = false;
        private bool _innerMode = false;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(NamedTextBox),
            new PropertyMetadata(OnTextPropertyChanged));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(NamedTextBox),
            new PropertyMetadata(OnTitlePropertyChanged));

        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register(
            "ButtonText",
            typeof(string),
            typeof(NamedTextBox),
            new PropertyMetadata(OnButtonTextPropertyChanged));

        public static readonly DependencyProperty ShowButtonProperty = DependencyProperty.Register(
            "ShowButton",
            typeof(bool),
            typeof(NamedTextBox),
            new PropertyMetadata(false, OnShowButtonPropertyChanged));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(NamedTextBox),
            new PropertyMetadata(OnCommandPropertyChanged));

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter",
            typeof(ICommand),
            typeof(NamedTextBox),
            new PropertyMetadata(OnCommandParameterPropertyChanged));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public bool ShowButton
        {
            get => (bool)GetValue(ShowButtonProperty);
            set => SetValue(ShowButtonProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public NamedTextBox()
        {
            InitializeComponent();

            tb.TextChanged += (d, e) =>
            {
                if (!_innerMode)
                {
                    _outerMode = true;
                    Text = tb.Text;
                    _outerMode = false;
                }
            };
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NamedTextBox sender && !sender._outerMode)
            {
                sender._innerMode = true;
                sender.tb.Text = (string)e.NewValue;
                sender._innerMode = false;
            }
        }

        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NamedTextBox sender)
                sender.t.Text = (string)e.NewValue;
        }

        private static void OnButtonTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NamedTextBox sender)
                sender.btn.Content = e.NewValue;
        }

        private static void OnShowButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NamedTextBox sender)
            {
                if ((bool)e.NewValue)
                    sender.btn.Visibility = Visibility.Visible;
                else
                    sender.btn.Visibility = Visibility.Collapsed;
            }
        }

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NamedTextBox sender)
                sender.btn.Command = (ICommand)e.NewValue;
        }

        private static void OnCommandParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NamedTextBox sender)
                sender.btn.CommandParameter = e.NewValue;
        }
    }
}
