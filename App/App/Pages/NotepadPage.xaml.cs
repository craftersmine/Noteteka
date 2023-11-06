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
using Windows.Storage.Streams;
using App.Controls;
using App.Dialogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotepadPage : Page
    {
        private DispatcherTimer _timer = new DispatcherTimer();

        public NotepadPage()
        {
            this.InitializeComponent();
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private async void _timer_Tick(object sender, object e)
        {
            foreach (TabViewItem tab in NotepadPagesTabView.TabItems)
            {
                NotepadPageEditor pageEditor = (tab.Content as NotepadPageEditor);
                if (pageEditor.IsChanged)
                {
                    using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                    {
                        pageEditor.Document.SaveToStream(TextGetOptions.FormatRtf, stream);

                        DataReader reader = new DataReader(stream.GetInputStreamAt(0));
                        pageEditor.Page.Data = new byte[stream.Size];
                        reader.LoadAsync((uint)stream.Size);
                        reader.ReadBytes(pageEditor.Page.Data);

                        App.DatabaseContext.NotepadPages.Entry(pageEditor.Page).State = EntityState.Modified;
                        await App.DatabaseContext.SaveChangesAsync();
                    }
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UpdatePages();
        }

        public void UpdatePages()
        {
            NotepadPagesTabView.TabItems.Clear();

            foreach (Core.NotepadPage page in App.DatabaseContext.NotepadPages)
            {
                TabViewItem tab = new TabViewItem();
                tab.ContextFlyout = TabContextMenu;
                tab.CloseRequested += OnTabClosed;

                NotepadPageEditor editor = new NotepadPageEditor(page);
                tab.Content = editor;
                tab.Header = page.Title;
                tab.Tag = page.Id;
                NotepadPagesTabView.TabItems.Add(tab);
            }
        }

        private async void OnAddNotepadPageClick(TabView sender, object args)
        {
            TabViewItem tab = new TabViewItem();
            tab.ContextFlyout = TabContextMenu;
            tab.CloseRequested += OnTabClosed;
            Core.NotepadPage page = new Core.NotepadPage();
            page.Title = "New Page";
            tab.Tag = page.Id;

            NotepadPageEditor editor = new NotepadPageEditor(page);
            tab.Content = editor;
            tab.Header = page.Title;
            sender.TabItems.Add(tab);

            App.DatabaseContext.NotepadPages.Add(page);
            await App.DatabaseContext.SaveChangesAsync();

            sender.SelectedItem = tab;
            tab.Tag = page.Id;
        }

        private async void OnTabClosed(TabViewItem sender, TabViewTabCloseRequestedEventArgs args)
        {
            ContentDialog dlg = new ContentDialog();
            AddEditNoteDialog dlgContent = new AddEditNoteDialog(null);
            dlg.XamlRoot = this.XamlRoot;
            dlg.Title = "Remove page";
            dlg.Content = "Are you sure you want to remove this page?";
            dlg.CloseButtonText = "No";
            dlg.PrimaryButtonText = "Yes";
            dlg.CloseButtonClick += (dialog, args) => dialog.Hide();
            switch (await dlg.ShowAsync())
            {
                case ContentDialogResult.Primary:
                    Core.NotepadPage page = App.DatabaseContext.NotepadPages.First(p => p.Id == int.Parse(sender.Tag.ToString()));
                    App.DatabaseContext.NotepadPages.Remove(page);
                    await App.DatabaseContext.SaveChangesAsync();

                    if (NotepadPagesTabView.TabItems.Any())
                        NotepadPagesTabView.SelectedIndex = NotepadPagesTabView.TabItems.IndexOf(sender) - 1;

                    NotepadPagesTabView.TabItems.Remove(sender);
                    break;
            }
        }

        private async void OnEditClick(object sender, RoutedEventArgs e)
        {
            TabViewItem selectedItem = NotepadPagesTabView.SelectedItem as TabViewItem;
            int selectedPageIndex = NotepadPagesTabView.SelectedIndex;
            if (selectedItem is not null)
            {
                Core.NotepadPage page =
                    App.DatabaseContext.NotepadPages.First(p => p.Id == int.Parse(selectedItem.Tag.ToString()));

                ContentDialog dlg = new ContentDialog();
                AddEditNoteDialog dlgContent = new AddEditNoteDialog(null);
                dlg.XamlRoot = this.XamlRoot;
                dlg.Title = "Edit page name";

                TextBox pageTitleTextBox = new TextBox();
                pageTitleTextBox.Text = page.Title;

                dlg.Content = pageTitleTextBox;
                dlg.CloseButtonText = "Cancel";
                dlg.PrimaryButtonText = "Ok";
                dlg.CloseButtonClick += (dialog, args) => dialog.Hide();
                switch (await dlg.ShowAsync())
                {
                    case ContentDialogResult.Primary:

                        page.Title = pageTitleTextBox.Text;

                        App.DatabaseContext.NotepadPages.Entry(page).State = EntityState.Modified;

                        await App.DatabaseContext.SaveChangesAsync();

                        UpdatePages();

                        NotepadPagesTabView.SelectedIndex = selectedPageIndex;

                        break;
                }
            }
        }
    }
}
