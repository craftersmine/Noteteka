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
        public bool IsEditing
        {
            get => !RichEditorBox.IsReadOnly;
        }
        public NotepadPage Page { get; private set; }
        public RichEditTextDocument Document
        {
            get => RichEditorBox.Document;
        }

        public NotepadPageEditor(NotepadPage page)
        {
            Page = page;

            this.InitializeComponent();

            if (Page.Data is not null && Page.Data.Length > 0)
            {
                using (MemoryStream stream = new MemoryStream(Page.Data))
                {
                    RichEditorBox.Document.LoadFromStream(TextSetOptions.FormatRtf, stream.AsRandomAccessStream());
                }
            }
        }

        private async void RichEditBox_OnTextChanged(object sender, RoutedEventArgs e)
        {
            IsChanged = true;
        }


        private void OnBoldClick(object sender, RoutedEventArgs e)
        {
            if (!IsEditing)
                return;

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
            if (!IsEditing)
                return;

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
            if (!IsEditing)
                return;

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
            if (!IsEditing)
                return;

            RichEditorBox.Document.Undo();
        }

        private void OnRedoClick(object sender, RoutedEventArgs e)
        {
            if (!IsEditing)
                return;

            RichEditorBox.Document.Redo();
        }

        private void OnEditSwitchClick(object sender, RoutedEventArgs e)
        {
            if (!IsEditing)
            {
                RichEditorBox.IsReadOnly = false;
            }
            else
                RichEditorBox.IsReadOnly = true;
        }

        private void OnAlignLeftClick(object sender, RoutedEventArgs e)
        {
            if (!IsEditing)
                return;

            ITextSelection selectedText = RichEditorBox.Document.Selection;
            if (selectedText != null)
            {
                ITextParagraphFormat selectedParagraphFormat = selectedText.ParagraphFormat;
                selectedParagraphFormat.Alignment = ParagraphAlignment.Left;
            }
        }

        private void OnAlignCenterClick(object sender, RoutedEventArgs e)
        {
            if (!IsEditing)
                return;

            ITextSelection selectedText = RichEditorBox.Document.Selection;
            if (selectedText != null)
            {
                ITextParagraphFormat selectedParagraphFormat = selectedText.ParagraphFormat;
                selectedParagraphFormat.Alignment = ParagraphAlignment.Center;
            }
        }

        private void OnAlignRightClick(object sender, RoutedEventArgs e)
        {
            if (!IsEditing)
                return;

            ITextSelection selectedText = RichEditorBox.Document.Selection;
            if (selectedText != null)
            {
                ITextParagraphFormat selectedParagraphFormat = selectedText.ParagraphFormat;
                selectedParagraphFormat.Alignment = ParagraphAlignment.Right;
            }
        }

        private void OnAlignJustifyClick(object sender, RoutedEventArgs e)
        {
            if (!IsEditing)
                return;

            ITextSelection selectedText = RichEditorBox.Document.Selection;
            if (selectedText != null)
            {
                ITextParagraphFormat selectedParagraphFormat = selectedText.ParagraphFormat;
                selectedParagraphFormat.Alignment = ParagraphAlignment.Justify;
            }
        }

        private void OnCutClick(object sender, RoutedEventArgs e)
        {
            if (!IsEditing) 
                return;

            ITextSelection selectedText = RichEditorBox.Document.Selection;
            if (selectedText != null)
            {
                selectedText.Cut();
            }
        }

        private void OnCopyClick(object sender, RoutedEventArgs e)
        {
            if (!IsEditing) 
                return;

            ITextSelection selectedText = RichEditorBox.Document.Selection;
            if (selectedText != null)
            {
                selectedText.Copy();
            }
        }

        private void OnPasteClick(object sender, RoutedEventArgs e)
        {
            if (!IsEditing) 
                return;

            RichEditorBox.Document.Selection.Paste(0);
        }

        private void OnFontDecreaseClick(object sender, RoutedEventArgs e)
        {
            if (!IsEditing) 
                return;

            ITextSelection selectedText = RichEditorBox.Document.Selection;
            if (selectedText != null)
            {
                selectedText.CharacterFormat.Size -= 1;
            }
        }

        private void OnFontIncreaseClick(object sender, RoutedEventArgs e)
        {
            if (!IsEditing) 
                return;

            ITextSelection selectedText = RichEditorBox.Document.Selection;
            if (selectedText != null)
            {
                selectedText.CharacterFormat.Size += 1;
            }
        }
    }
}
