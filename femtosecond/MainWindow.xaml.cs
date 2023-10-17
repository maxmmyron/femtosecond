using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml.Controls;
using System;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace femtosecond
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private AppWindow m_AppWindow;
        FolderPicker folderPicker;

        string currentPath;

        public AppViewModel ViewModel { get; set; } = new AppViewModel();
        public static MainWindow Current { get; set; }

        public MainWindow()
        {
            this.InitializeComponent();
            Current = this;

            m_AppWindow = GetAppWindowForCurrentWindow();

            InitializeFolderPicker();

            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                var titleBar = m_AppWindow.TitleBar;
                titleBar.ExtendsContentIntoTitleBar = true;
                ExtendsContentIntoTitleBar = true;

                AppTitleBar.Loaded += AppTitleBar_Loaded;
                AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
            } 
            else
            {
                AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }

        private void InitializeFolderPicker() 
        {
            folderPicker = new FolderPicker();
            folderPicker.ViewMode = PickerViewMode.List;
            folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            var hWnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(folderPicker, hWnd);
        }
        
        private AppWindow GetAppWindowForCurrentWindow() 
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }

        private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppWindowTitleBar.IsCustomizationSupported()) 
            {
                SetDragRegionForCustomTitleBar(m_AppWindow);
            }
        }

        private void AppTitleBar_SizeChanged(object sender, RoutedEventArgs e)
        {
            if(AppWindowTitleBar.IsCustomizationSupported()
                && m_AppWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                SetDragRegionForCustomTitleBar(m_AppWindow);
            }
        }

        private void SetDragRegionForCustomTitleBar(AppWindow appWindow)
        {
            if(AppWindowTitleBar.IsCustomizationSupported()
                && appWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                double scaleAdjustment = GetScaleAdjustment();

                RightPaddingColumn.Width = new GridLength(appWindow.TitleBar.RightInset / scaleAdjustment);
                LeftPaddingColumn.Width = new GridLength(appWindow.TitleBar.LeftInset / scaleAdjustment);

                List<Windows.Graphics.RectInt32> dragRectsList = new();
         
                Windows.Graphics.RectInt32 dragRectL;
                dragRectL.X = (int)(LeftPaddingColumn.ActualWidth * scaleAdjustment);
                dragRectL.Y = 0;
                dragRectL.Width = (int)((IconColumn.ActualWidth + TitleColumn.ActualWidth) * scaleAdjustment);
                dragRectL.Height = (int)(AppTitleBar.ActualHeight * scaleAdjustment);

                dragRectsList.Add(dragRectL);

                Windows.Graphics.RectInt32 dragRectR;
                dragRectR.X = (int)((LeftPaddingColumn.ActualWidth + IconColumn.ActualWidth + TitleColumn.ActualWidth + MenuColumn.ActualWidth) * scaleAdjustment);
                dragRectR.Y = 0;
                dragRectR.Width = (int)(RightDragColumn.ActualWidth * scaleAdjustment);
                dragRectR.Height = (int)(AppTitleBar.ActualHeight * scaleAdjustment);

                dragRectsList.Add(dragRectR);

                Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();

                appWindow.TitleBar.SetDragRectangles(dragRects);
            }
        }

        [DllImport("Shcore.dll", SetLastError = true)]
        internal static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

        internal enum Monitor_DPI_Type : int
        {
            MDT_Effective_DPI = 0,
            MDT_Angular_DPI = 1,
            MDT_Raw_DPI = 2,
            MDT_Default = MDT_Effective_DPI
        }

        private double GetScaleAdjustment() 
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
            IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

            // get DPI
            int result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out uint dpiX, out uint _);
            if(result != 0)
            {
                throw new Exception("Could not get DPI for monitor.");
            }
            uint scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
            return scaleFactorPercent / 100.0;
        }

        private async void OnNewWorkspaceButtonClick(object sender, RoutedEventArgs e)
        {
            if(ViewModel.WorkspaceDirectory == null)
            {
                CreateNewWorkspace();
                return;
            }

            string fileContents = System.IO.File.ReadAllText(currentPath);
            if(fileContents != ViewModel.EditorContents)
            {
                Boolean canOpen = await AskToSaveChanges();
                if (canOpen) CreateNewWorkspace();
            }
        }

        private void CreateNewWorkspace()
        {
            ViewModel.WorkspaceDirectory = null;
            ViewModel.EditorContents = "";
        }

        private async System.Threading.Tasks.Task<Boolean> AskToSaveChanges()
        {
            ContentDialog saveDialog = new ContentDialog()
            {
                Title = "Save your changes?",
                Content = "You have unsaved changes in your current file. Save the file before opening a new one to prevnet losing your chanages.",
                PrimaryButtonText = "Save",
                SecondaryButtonText = "Don't Save",
                CloseButtonText = "Cancel",
            };

            ContentDialogResult result = await saveDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                System.IO.File.WriteAllText(currentPath, ViewModel.EditorContents);
            }

            if (result != ContentDialogResult.None)
            {
                return false;
            }

            return true;
        }

        private async void OnOpenFolderButtonClicked(object sender, RoutedEventArgs e)
        {
            string fileContents = System.IO.File.ReadAllText(currentPath);
            Boolean canOpen = true;
            if (ViewModel.WorkspaceDirectory != null && ViewModel.EditorContents != fileContents)
            {
                canOpen = await AskToSaveChanges();
            }

            if (!canOpen) return;

            (App.Current as App).workingDirectory = await folderPicker.PickSingleFolderAsync();
            CreateWorkspaceFromAppDirectory();
        }

        private void OnSaveFileButtonClick(object sender, RoutedEventArgs e)
        {
            if(currentPath != null)
            {
                System.IO.File.WriteAllText(currentPath, ViewModel.EditorContents);
                // await Windows.Storage.FileIO.WriteTextAsync(currentFile, ViewModel.EditorContents);
            }
        }

        private void CreateWorkspaceFromAppDirectory()
        {
            (App.Current as App).Files = System.IO.Directory.EnumerateFiles((App.Current as App).workingDirectory.Path, "*", System.IO.SearchOption.AllDirectories);

            int dirLen = (App.Current as App).workingDirectory.Path.Length;

            foreach (string file in (App.Current as App).Files)
            {
                MenuFileItem item = new MenuFileItem();
                item.FileName = file.Substring(dirLen);
                NavigationView.MenuItems.Add(item);
            }

            ContentFrame.Navigate(typeof(Workspace));
        }

        private void OnNavigationViewSelectionChanged(NavigationView sender, 
            NavigationViewSelectionChangedEventArgs args)
        {
            currentPath = (App.Current as App).workingDirectory.Path + args.SelectedItemContainer.Content;
            ViewModel.EditorContents = System.IO.File.ReadAllText(currentPath);
        }
    }
}
