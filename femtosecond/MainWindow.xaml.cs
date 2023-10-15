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
        FileOpenPicker openPicker;
        Windows.Storage.StorageFile currentFile;

        public MainWindow()
        {
            this.InitializeComponent();

            m_AppWindow = GetAppWindowForCurrentWindow();

            InitializeOpenPicker();

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

        private void InitializeOpenPicker()
        {
            openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".txt");
            openPicker.FileTypeFilter.Add(".js");
            openPicker.FileTypeFilter.Add(".ts");
            openPicker.FileTypeFilter.Add(".css");
            openPicker.FileTypeFilter.Add(".html");
            openPicker.FileTypeFilter.Add(".svelte");

            var hWnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(openPicker, hWnd);
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

        private void OnNewFileButtonClick(object sender, RoutedEventArgs e)
        {
           
        }

        private async void OnOpenFileButtonClick(object sender, RoutedEventArgs e)
        {

            currentFile = await openPicker.PickSingleFileAsync();
            if(currentFile != null)
            {
                string contents = await Windows.Storage.FileIO.ReadTextAsync(currentFile);
                if(contents != null)
                {
                    this.Editor.Text = contents;
                }
            }
        }

        private async void OnSaveFileButtonClick(object sender, RoutedEventArgs e)
        {
            if(currentFile != null)
            {
                await Windows.Storage.FileIO.WriteTextAsync(currentFile, this.Editor.Text);
            }
        }
    }
}
