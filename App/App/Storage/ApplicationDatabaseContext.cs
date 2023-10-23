using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core;
using App.Extensions;
using Microsoft.EntityFrameworkCore;

namespace App.Storage
{
    public class ApplicationDatabaseContext : DbContext
    {
        public string ApplicationDatabasePath { get; private set; }

        public DbSet<StickyNote> StickyNotes { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<ToDoTask> ToDoTasks { get; set; }
        public DbSet<NotepadPage> NotepadPages { get; set; }

        public ApplicationDatabaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalendarEvent>().Property(p => p.RepeatOnDays)
                .HasConversion(v => v.ToCommaSeparatedString(), v => v.FromCommaSeparatedString<DayOfWeek>());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string filename = Path.Combine(App.ApplicationDataStoragePath, "DataStorage.db");
            ApplicationDatabasePath = filename;
            optionsBuilder.UseSqlite("Filename=" + filename);
        }
    }
}
