﻿using System.Windows;
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

            tbUrl.KeyDown += TbUrl_KeyDown;
        }

        private void TbUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext is CollectionViewModel collectionVM)
            {
                tbUrl.GetBindingExpression(TextBox.TextProperty).UpdateSource();

                bool isCtrlPressed = Keyboard.Modifiers == ModifierKeys.Control;
                if (collectionVM.Analyze.CanExecute(isCtrlPressed))
                    collectionVM.Analyze.Execute(isCtrlPressed);
            }
        }
    }
}
