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
using App.Controls;
using App.Dialogs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public ObservableCollection<StickyNoteControl> StickyNotes = new ObservableCollection<StickyNoteControl>();
        public HomePage()
        {
            this.InitializeComponent();
        }

        private void AddNote(object sender, RoutedEventArgs e)
        {
            ContentDialog dlg = Utilities.GetParent<MainWindow>(this).ContentDialogHost;
            AddNoteDialog dlgContent = new AddNoteDialog();
            dlg.Title = "Add new note";
            dlg.Content = dlgContent;
            dlg.CloseButtonText = "Cancel";
            dlg.PrimaryButtonText = "Add";
            dlg.CloseButtonClick += (dialog, args) => dialog.Hide();
            dlg.PrimaryButtonClick += Dlg_PrimaryButtonClick;
            dlg.ShowAsync();
        }

        private void Dlg_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            AddNoteDialog dlgContent = (AddNoteDialog)sender.Content;

            StickyNotes.Add(new StickyNoteControl()
            {
                NoteText = dlgContent.NoteText,
                StickyNoteColor = StickyNoteColor.Yellow
            });
            sender.PrimaryButtonClick -= Dlg_PrimaryButtonClick;
        }
    }
}
