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
    [TemplatePart(Name = nameof(FileNameTextBox), Type = typeof(TextBox))]
    public sealed class MenuFileItem : Control
    {
        private TextBox? FileNameTextBox { get; set; }

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

        private void OnRenameContextItemClick(object sender, RoutedEventArgs e)
        {
            FileNameTextBox.Visibility = Visibility.Visible;
            FileNameTextBox.Focus(FocusState.Keyboard);
        }

        private void OnTextBoxKeyDown(object sender, KeyRoutedEventArgs e) 
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                string newFileName = (sender as TextBox).Text;

                FileNameTextBox.Visibility = Visibility.Collapsed;
            }
        }

        private void OnTextBoxFocusDisengaged(object sender, RoutedEventArgs e)
        {
            FileNameTextBox.Visibility = Visibility.Collapsed;
        }

        private void OnDeleteContextItemClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
