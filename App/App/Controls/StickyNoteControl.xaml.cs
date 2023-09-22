using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App.Controls
{
    public sealed partial class StickyNoteControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty StickyNoteColorProperty = DependencyProperty.Register(nameof(StickyNoteColor), typeof(StickyNoteColor), typeof(StickyNoteControl), new PropertyMetadata(Controls.StickyNoteColor.Yellow));
        public static readonly DependencyProperty ForegroundColorProperty = DependencyProperty.Register(nameof(ForegroundColor), typeof(Color), typeof(StickyNoteControl), new PropertyMetadata(Color.FromArgb(255, 0, 0, 0)));
        public static readonly DependencyProperty NoteTextProperty = DependencyProperty.Register(nameof(NoteText), typeof(string), typeof(StickyNoteControl), new PropertyMetadata(string.Empty));
        //public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(nameof(BackgroundColor), typeof(Color), typeof(StickyNoteColor), new PropertyMetadata(Color.FromArgb(255, 0, 0, 0)));

        public StickyNoteColor StickyNoteColor
        {
            get => (StickyNoteColor)GetValue(StickyNoteColorProperty);
            set
            {
                SetValue(StickyNoteColorProperty, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }

        public Color ForegroundColor
        {
            get => (Color)GetValue(ForegroundColorProperty);
            set
            {
                SetValue(ForegroundColorProperty, value);
                OnPropertyChanged();
            }
        }

        public string NoteText
        {
            get => (string)GetValue(NoteTextProperty);
            set
            {
                SetValue(NoteTextProperty, value);
                OnPropertyChanged();
            }
        }

        public Color BackgroundColor
        {
            get
            {
                switch (StickyNoteColor)
                {
                    case StickyNoteColor.Yellow:
                    default:
                        return Color.FromArgb(255, 175, 156, 72);
                    case StickyNoteColor.Green:
                        return Color.FromArgb(255, 97, 165, 66);
                    case StickyNoteColor.Cyan:
                        return Color.FromArgb(255, 65, 168, 146);
                    case StickyNoteColor.Blue:
                        return Color.FromArgb(255, 72, 96, 168);
                    case StickyNoteColor.Purple:
                        return Color.FromArgb(255, 150, 96, 165);
                    case StickyNoteColor.Red:
                        return Color.FromArgb(255, 165, 81, 95);
                }
            }
        }

        public StickyNoteControl()
        {
            this.InitializeComponent();
            Background = new SolidColorBrush(new Color() { R = 255, G = 240, B = 0, A = 255 });
            //BackgroundColor = new Color() { R = 255, G = 240, B = 0, A = 255};
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public enum StickyNoteColor
    {
        Yellow,
        Green,
        Red,
        Blue,
        Purple,
        Cyan
    }
}
