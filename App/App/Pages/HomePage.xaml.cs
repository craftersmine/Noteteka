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
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using App.Core;
using App.Dialogs;
using Microsoft.EntityFrameworkCore;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();

            UpdateNotesList();
            UpdateToDoTasks();
            UpdateUpcomingTasks();
        }

        private void AddNote(object sender, RoutedEventArgs e)
        {
            ContentDialog dlg = new ContentDialog();
            AddNoteDialog dlgContent = new AddNoteDialog(null);
            dlg.XamlRoot = this.XamlRoot;
            dlg.Title = "Add new note";
            dlg.Content = dlgContent;
            dlg.CloseButtonText = "Cancel";
            dlg.PrimaryButtonText = "Add";
            dlg.CloseButtonClick += (dialog, args) => dialog.Hide();
            dlg.PrimaryButtonClick += Dlg_PrimaryButtonClick;
            dlg.ShowAsync();
        }

        private async void Dlg_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            AddNoteDialog dlgContent = (AddNoteDialog)sender.Content;

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

        private void UpdateToDoTasks()
        {
            //App.DatabaseContext.ToDoTasks.Add(new ToDoTask() { Title = "New task", Description = "Test description" });
            //App.DatabaseContext.ToDoTasks.Add(new ToDoTask() { Title = "New task", Description = "Test description" });
            //App.DatabaseContext.ToDoTasks.Add(new ToDoTask() { Title = "Completed task", Description = "Test description that is completed", IsCompleted = true });
            //App.DatabaseContext.ToDoTasks.Add(new ToDoTask() { Title = "Old task", Description = "Test description for old", IsCompleted = true });
            //App.DatabaseContext.ToDoTasks.Add(new ToDoTask() { Title = "Old task", Description = "Test description for old" });
            //App.DatabaseContext.SaveChanges();

            ToDoEventsGridView.ItemsSource = null;
            ToDoEventsGridView.Items.Clear();

            if (App.DatabaseContext.ToDoTasks.Any())
            {
                ToDoEventsGridView.Visibility = Visibility.Visible;
                ObservableCollection<ToDoTask> tasks = new ObservableCollection<ToDoTask>();
                ToDoEventsGridView.ItemsSource = tasks;

                foreach (ToDoTask task in App.DatabaseContext.ToDoTasks.Where(t => !t.IsCompleted).AsEnumerable())
                {
                    tasks.Add(task);
                }
                NoToDoEventsLabel.Visibility = Visibility.Collapsed;
            }
            else
            {
                ToDoEventsGridView.Visibility = Visibility.Collapsed;
                NoToDoEventsLabel.Visibility = Visibility.Visible;
            }
        }

        private void UpdateUpcomingTasks()
        {

        }

        private void EditNoteClick(object sender, RoutedEventArgs e)
        {
            ContentDialog dlg = new ContentDialog();

            StickyNote note = App.DatabaseContext.StickyNotes.First(n => n.Id == (int)((Button)sender).Tag);
            AddNoteDialog dlgContent = new AddNoteDialog(note);
            dlg.XamlRoot = this.XamlRoot;
            dlg.Title = "Edit note";
            dlg.Content = dlgContent;
            dlg.CloseButtonText = "Cancel";
            dlg.PrimaryButtonText = "Edit";
            dlg.CloseButtonClick += (dialog, args) => dialog.Hide();
            dlg.PrimaryButtonClick += Dlg_PrimaryButtonClick;
            dlg.ShowAsync();
        }

        private void OnTaskDoneChecked(object sender, RoutedEventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            int taskId = (int)check.Tag;

            ToDoTask task = App.DatabaseContext.ToDoTasks.FirstOrDefault(n => n.Id == taskId);
            task.IsCompleted = check.IsChecked ?? false;
            App.DatabaseContext.ToDoTasks.Entry(task).State = EntityState.Modified;
            App.DatabaseContext.SaveChanges();

            UpdateToDoTasks();
        }
    }
}
