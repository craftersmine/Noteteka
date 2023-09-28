using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using App.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddNoteDialog : Page
    {
        public string NoteText => NoteTextTextBox.Text;
        public StickyNoteColor Color => ColorComboBox.SelectedItem as StickyNoteColor;
        public bool IsEditing { get; }
        public StickyNote? StickyNote { get; }

        public AddNoteDialog(StickyNote? note)
        {
            this.InitializeComponent();

            StickyNote = note;

            if (StickyNote is not null)
            {
                NoteTextTextBox.Text = StickyNote.Text;
                IsEditing = true;
            }
        }
    }
}
