using App.Core;
using App.Dialogs;

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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotesPage : Page
    {
        public NotesPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateNotesList();
            base.OnNavigatedTo(e);
        }

        private void UpdateNotesList()
        {
            NotesGridView.ItemsSource = null;
            NotesGridView.Items.Clear();

            if (App.DatabaseContext.StickyNotes.Any())
            {
                NotesGridView.Visibility = Visibility.Visible;
                NotesGridView.ItemsSource = App.DatabaseContext.StickyNotes;
                NoNotesLabel.Visibility = Visibility.Collapsed;
            }
            else
            {
                NotesGridView.Visibility = Visibility.Collapsed;
                NoNotesLabel.Visibility = Visibility.Visible;
            }
        }

        private void AddNote(object sender, RoutedEventArgs e)
        {
            ContentDialog dlg = new ContentDialog();
            AddEditNoteDialog dlgContent = new AddEditNoteDialog(null);
            dlg.XamlRoot = this.XamlRoot;
            dlg.Title = "Add new note";
            dlg.Content = dlgContent;
            dlg.CloseButtonText = "Cancel";
            dlg.PrimaryButtonText = "Add";
            dlg.CloseButtonClick += (dialog, args) => dialog.Hide();
            dlg.PrimaryButtonClick += Dlg_PrimaryButtonClick;
            dlg.IsPrimaryButtonEnabled = false;
            dlg.ShowAsync();
        }

        private async void Dlg_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            AddEditNoteDialog dlgContent = (AddEditNoteDialog)sender.Content;

            if (!dlgContent.IsEditing)
            {
                App.DatabaseContext.StickyNotes.Add(new StickyNote()
                {
                    Text = dlgContent.NoteText,
                    Color = dlgContent.Color.Color
                });
            }
            else
            {
                StickyNote note = App.DatabaseContext.StickyNotes.First(n => n.Id == dlgContent.StickyNote.Id);
                note.Text = dlgContent.NoteText;
                note.Color = dlgContent.Color.Color;
            }
            await App.DatabaseContext.SaveChangesAsync();
            sender.PrimaryButtonClick -= Dlg_PrimaryButtonClick;

            UpdateNotesList();
        }

        private void DeleteNoteConfirmClick(object sender, RoutedEventArgs e)
        {
            StickyNote noteToDelete = App.DatabaseContext.StickyNotes.First(n => n.Id == (int)((Button)sender).Tag);
            App.DatabaseContext.StickyNotes.Remove(noteToDelete);
            App.DatabaseContext.SaveChanges();

            UpdateNotesList();
        }

        private void EditNoteClick(object sender, RoutedEventArgs e)
        {
            ContentDialog dlg = new ContentDialog();

            StickyNote note = App.DatabaseContext.StickyNotes.First(n => n.Id == (int)((Button)sender).Tag);
            AddEditNoteDialog dlgContent = new AddEditNoteDialog(note);
            dlg.XamlRoot = this.XamlRoot;
            dlg.Title = "Edit note";
            dlg.Content = dlgContent;
            dlg.CloseButtonText = "Cancel";
            dlg.PrimaryButtonText = "Edit";
            dlg.CloseButtonClick += (dialog, args) => dialog.Hide();
            dlg.PrimaryButtonClick += Dlg_PrimaryButtonClick;
            dlg.ShowAsync();
        }
    }
}
