using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace App.Core
{
    public class DbMetadata
    {
        [Key]
        public int Id { get; set; }
        public Guid DbVersion { get; set; }
    }
}
