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
using App.Core;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddEditTaskDialog : Page
    {
        public ObservableCollection<ToDoTaskPriority> Priorities { get; private set; } = new ObservableCollection<ToDoTaskPriority>();

        public string TaskTitle => ToDoTask?.Title;
        public string TaskDescription => ToDoTask?.Description;
        public ToDoTaskPriority TaskPriority => ToDoTask is not null ? ToDoTask.Priority : ToDoTaskPriority.Normal;

        public bool IsEditing { get; private set; }

        public ToDoTask? ToDoTask { get; private set; }

        public AddEditTaskDialog(ToDoTask? task)
        {
            foreach (ToDoTaskPriority genre in (ToDoTaskPriority[])Enum.GetValues(typeof(ToDoTaskPriority)))
            {
                Priorities.Add(genre);
            }

            ToDoTask = task;

            this.InitializeComponent();

            if (ToDoTask is not null)
            {
                IsEditing = true;
                TaskTitleTextBox.Text = ToDoTask.Title;
                TaskDescriptionTextBox.Text = ToDoTask.Description;
                TaskPriorityComboBox.SelectedItem = ToDoTask.Priority;
            }
        }
    }
}
