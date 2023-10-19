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
    public sealed class RenameDialog : ContentDialog
    {
        public string InitialFileName {
            get => (string)GetValue(InitialFileNameProperty);
            set => SetValue(InitialFileNameProperty, value);
        }

        public string NewFileName { get; set; }

        DependencyProperty InitialFileNameProperty = DependencyProperty.Register(
            nameof(InitialFileName),
            typeof(string),
            typeof(RenameDialog),
            new PropertyMetadata(null)
        );

        public RenameDialog(string InitialFileName, XamlRoot xamlRoot)
        {
            this.DefaultStyleKey = typeof(RenameDialog);
            this.InitialFileName = InitialFileName;
            this.XamlRoot = xamlRoot;
        }
    }
}
