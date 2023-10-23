using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core
{
    public class NotepadPage
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public byte[] Data { get; set; }
    }
}
