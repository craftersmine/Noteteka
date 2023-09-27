using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI;
using App.Extensions;
using CommunityToolkit.WinUI.Helpers;

namespace App.Core
{
    public class StickyNote
    {
        [NotMapped]
        public Color Color
        {
            get
            {
                return ColorValue.ToColor();
            }
            set
            {
                ColorValue = value.ToInt();
            }
        }
        public int ColorValue { get; set; }
        public string Text { get; set; }
        [Key]
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

        [Key]
        public string Name { get; set; }

        [NotMapped]
        public Color Color
        {
            get
            {
                return ColorValue.ToColor();
            }
            set
            {
                ColorValue = value.ToInt();
            }
        }

        public int ColorValue { get; set; }

        public StickyNoteColor()
        {
            Name = "Yellow";
            Color = Color.FromArgb(255, 175, 156, 72);
        }

        public StickyNoteColor(string name, Color color)
        {
            Name = name;
            Color = color;
        }
    }
}
