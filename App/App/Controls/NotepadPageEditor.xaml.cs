using System.IO;
using Windows.Storage.Streams;
using App.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App.Controls
{
    public sealed partial class NotepadPageEditor : Grid
    {
        public bool IsChanged { get; private set; }
        public NotepadPage Page { get; private set; }
        public RichEditTextDocument Document
        {
            get => RichEditorBox.Document;
        }

        public NotepadPageEditor(NotepadPage page)
        {
            Page = page;

            this.InitializeComponent();

            using (MemoryStream stream = new MemoryStream(Page.Data))
            {
                RichEditorBox.Document.LoadFromStream(TextSetOptions.None, stream.AsRandomAccessStream());
            }
        }

        private async void RichEditBox_OnTextChanged(object sender, RoutedEventArgs e)
        {
            IsChanged = true;
        }
    }
}
