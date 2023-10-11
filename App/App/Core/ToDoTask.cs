using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core
{
    public class ToDoTask
    {
        [Key]
        public int Id { get; set; }
        public ToDoTaskPriority Priority { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
    }

    public enum ToDoTaskPriority
    {
        HighPriority = 1,
        NormalPriority = 0,
        LowPriority = -1
    }
}
