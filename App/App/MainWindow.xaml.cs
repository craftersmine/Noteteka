using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using App.Core;
using App.Pages;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        public MainWindow()
        {
            this.InitializeComponent();

            ExtendsContentIntoTitleBar = true;

            SetTitleBar(AppTitlebar);

            NavigationView.SelectedItem = NavigationView.MenuItems.FirstOrDefault(i => (i as NavigationViewItem).Tag.ToString().ToLower() == "homepage");
        }

        private void OnNavigationViewItemChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            switch (((Control)args.SelectedItem)?.Tag.ToString()?.ToLower())
            {
                case "homepage":
                    NavigateTo<HomePage>();
                    break;
                case "calendar":
                    NavigateTo<CalendarPage>();
                    break;
                case "notes":
                    NavigateTo<NotesPage>();
                    break;
                case "notebook":
                    break;
                case "tasks":
                    break;
                case "settings":
                    NavigateTo<SettingsPage>();
                    break;
            }
        }

        private void NavigateTo<T>()
        {
            (NavigationView.Content as Frame).Navigate(typeof(T));
            NavigationView.Header = ((NavigationView.Content as Frame).Content as Page).Tag;
        }
    }
}
