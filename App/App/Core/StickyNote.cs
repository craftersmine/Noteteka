using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI;

namespace App.Core
{
    public class StickyNote
    {
        public StickyNoteColor Color { get; set; }
        public string Text { get; set; }
        public int Id { get; set; }
    }

    public class StickyNoteColor
    {
        public static ObservableCollection<StickyNoteColor> StickyNoteColors { get; private set; } = new ObservableCollection<StickyNoteColor>(){
            new StickyNoteColor("Yellow", Color.FromArgb(255, 175, 156, 72)),
            new StickyNoteColor("Green", Color.FromArgb(255, 97, 165, 66)),
            new StickyNoteColor("Cyan", Color.FromArgb(255, 65, 168, 146)),
            new StickyNoteColor("Blue", Color.FromArgb(255, 72, 96, 168)),
            new StickyNoteColor("Purple", Color.FromArgb(255, 150, 96, 165)),
            new StickyNoteColor("Red", Color.FromArgb(255, 165, 81, 95))
        };

        public string Name { get; set; }
        public Color Color { get; set; }

        public StickyNoteColor(string name, Color color)
        {
            Name = name;
            Color = color;
        }
    }
}
