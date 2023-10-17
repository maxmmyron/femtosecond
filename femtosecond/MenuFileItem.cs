using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace femtosecond
{
    public sealed class MenuFileItem : Control
    {
        private TextBox textBox;

        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }

        DependencyProperty FileNameProperty = DependencyProperty.Register(
            nameof(FileName),
            typeof(string),
            typeof(MenuFileItem),
            new PropertyMetadata(default(string), new PropertyChangedCallback(OnFileNameChanged))
        );

        public MenuFileItem()
        {
            this.DefaultStyleKey = typeof(MenuFileItem);
        }

        private static void OnFileNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private void RenameItemClick(object sender, RoutedEventArgs e)
        {
            // create a textbox for input
            textBox = new TextBox()
            {
                Text = FileName,
            };

            textBox.KeyDown += OnTextBoxKeyDown;

            textBox.Focus(FocusState.Keyboard);

            textBox.FocusDisengaged += OnTextBoxFocusDisengaged;

        }

        private void OnTextBoxKeyDown(object sender, KeyRoutedEventArgs e) 
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                string newFileName = (sender as TextBox).Text;

                textBox.Visibility = Visibility.Collapsed;
            }
        }

        private void OnTextBoxFocusDisengaged(object sender, RoutedEventArgs e)
        {
            textBox.Visibility = Visibility.Collapsed;
        }

        private void DeleteItemClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
