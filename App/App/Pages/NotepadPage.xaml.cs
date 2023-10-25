using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using App.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotepadPage : Page
    {
        public NotepadPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        public void UpdatePages()
        {

        }

        private async void OnAddNotepadPageClick(TabView sender, object args)
        {
            TabViewItem tab = new TabViewItem();
            Core.NotepadPage page = new Core.NotepadPage();
            page.Title = "Test";

            NotepadPageEditor editor = new NotepadPageEditor(page);
            tab.Content = editor;
            tab.Header = page.Title;
            sender.TabItems.Add(tab);

            App.DatabaseContext.NotepadPages.Add(page);
            await App.DatabaseContext.SaveChangesAsync();
        }
    }
}
