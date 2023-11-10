using App.Core;
using App.Dialogs;

using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
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
    public sealed partial class ToDoTasksPage : Page
    {
        public ObservableCollection<ToDoTaskPriority> Priorities { get; private set; } = new ObservableCollection<ToDoTaskPriority>();

        public ToDoTasksPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            foreach (ToDoTaskPriority genre in (ToDoTaskPriority[])Enum.GetValues(typeof(ToDoTaskPriority)))
            {
                Priorities.Add(genre);
            }
            UpdateToDoTasks();
            base.OnNavigatedTo(e);
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

        private void UpdateToDoTasks()
        {
            ToDoEventsGridView.ItemsSource = null;
            ToDoEventsGridView.Items.Clear();

            if (App.DatabaseContext.ToDoTasks.Any())
            {
                App.DatabaseContext.SaveChanges();
                ToDoEventsGridView.Visibility = Visibility.Visible;
                ToDoEventsGridView.ItemsSource = App.DatabaseContext.ToDoTasks.OrderByDescending(t => t.Priority).ToList();
                NoToDoEventsLabel.Visibility = Visibility.Collapsed;
            }
            else
            {
                ToDoEventsGridView.Visibility = Visibility.Collapsed;
                NoToDoEventsLabel.Visibility = Visibility.Visible;
            }
        }

        private void OnPriorityComboboxClosed(object sender, object e)
        {
            ComboBox comboBox = (ComboBox)sender;
            int taskId = (int)comboBox.Tag;

            ToDoTask task = App.DatabaseContext.ToDoTasks.FirstOrDefault(n => n.Id == taskId);

            if (task.Priority != (ToDoTaskPriority)comboBox.SelectedItem)
            {
                task.Priority = (ToDoTaskPriority)comboBox.SelectedItem;
                App.DatabaseContext.ToDoTasks.Entry(task).State = EntityState.Modified;
                App.DatabaseContext.SaveChanges();

                UpdateToDoTasks();
            }
        }

        private void AddTaskClick(object sender, RoutedEventArgs e)
        {
            ContentDialog dlg = new ContentDialog();
            AddEditTaskDialog dlgContent = new AddEditTaskDialog(null);
            dlg.XamlRoot = this.XamlRoot;
            dlg.Title = "Add new task";
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
            AddEditTaskDialog dlgContent = (AddEditTaskDialog)sender.Content;

            if (!dlgContent.IsEditing)
            {
                App.DatabaseContext.ToDoTasks.Add(new ToDoTask()
                {
                    Description = dlgContent.TaskDescription,
                    Title = dlgContent.TaskTitle,
                    Priority = dlgContent.TaskPriority
                });
            }
            else
            {
                // TODO: fix editing of task
                ToDoTask task = App.DatabaseContext.ToDoTasks.First(n => n.Id == dlgContent.ToDoTask.Id);
                task.Title = dlgContent.TaskTitle;
                task.Description = dlgContent.TaskDescription;
                task.Priority = dlgContent.TaskPriority;
                App.DatabaseContext.ToDoTasks.Entry(task).State = EntityState.Modified;

            }
            await App.DatabaseContext.SaveChangesAsync();
            sender.PrimaryButtonClick -= Dlg_PrimaryButtonClick;

            UpdateToDoTasks();
        }

        private async void ContextDeleteClick(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;

            ToDoTask task = App.DatabaseContext.ToDoTasks.FirstOrDefault(t => t.Id == int.Parse(item.Tag.ToString()));

            ContentDialog dlg = new ContentDialog();
            AddEditNoteDialog dlgContent = new AddEditNoteDialog(null);
            dlg.XamlRoot = this.XamlRoot;
            dlg.Title = "Remove task";
            dlg.Content = "Are you sure you want to remove this task?";
            dlg.CloseButtonText = "No";
            dlg.PrimaryButtonText = "Yes";
            dlg.CloseButtonClick += (dialog, args) => dialog.Hide();
            switch (await dlg.ShowAsync())
            {
                case ContentDialogResult.Primary:
                    App.DatabaseContext.ToDoTasks.Remove(task);
                    await App.DatabaseContext.SaveChangesAsync();
                    UpdateToDoTasks();
                    break;
            }
        }

        private void ContextEditClick(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;

            ToDoTask task = App.DatabaseContext.ToDoTasks.FirstOrDefault(t => t.Id == (int)item.Tag);

            ContentDialog dlg = new ContentDialog();
            AddEditTaskDialog dlgContent = new AddEditTaskDialog(task);
            dlg.XamlRoot = this.XamlRoot;
            dlg.Title = "Edit task";
            dlg.Content = dlgContent;
            dlg.CloseButtonText = "Cancel";
            dlg.PrimaryButtonText = "Edit";
            dlg.CloseButtonClick += (dialog, args) => dialog.Hide();
            dlg.PrimaryButtonClick += Dlg_PrimaryButtonClick;
            dlg.ShowAsync();
        }
    }
}
