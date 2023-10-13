using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

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
        High = 1,
        Normal = 0,
        Low = -1
    }
}
