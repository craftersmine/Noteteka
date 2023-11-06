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
                RichEditorBox.Document.LoadFromStream(TextSetOptions.FormatRtf, stream.AsRandomAccessStream());
            }
        }

        private async void RichEditBox_OnTextChanged(object sender, RoutedEventArgs e)
        {
            IsChanged = true;
        }


        private void OnBoldClick(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = RichEditorBox.Document.Selection;
            if (selectedText != null)
            {
                ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Bold = FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void OnItalicClick(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = RichEditorBox.Document.Selection;
            if (selectedText != null)
            {
                ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Italic = FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void OnUnderlineClick(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = RichEditorBox.Document.Selection;
            if (selectedText != null)
            {
                ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                if (charFormatting.Underline == UnderlineType.None)
                {
                    charFormatting.Underline = UnderlineType.Single;
                }
                else {
                    charFormatting.Underline = UnderlineType.None;
                }
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void OnUndoClick(object sender, RoutedEventArgs e)
        {
            RichEditorBox.Document.Undo();
        }

        private void OnRedoClick(object sender, RoutedEventArgs e)
        {
            RichEditorBox.Document.Redo();
        }
    }
}
