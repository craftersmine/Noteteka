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
        private static readonly Guid DbVersion = new Guid("b8401604-a9f0-4c5c-98f3-e67d47c0e4dc");

        public string ApplicationDatabasePath { get; private set; }

        public DbSet<StickyNote> StickyNotes { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<ToDoTask> ToDoTasks { get; set; }
        public DbSet<NotepadPage> NotepadPages { get; set; }
        public DbSet<DbMetadata> Metadata { get; set; }

        public ApplicationDatabaseContext()
        {
            string filename = Path.Combine(App.ApplicationDataStoragePath, "DataStorage.db");
            ApplicationDatabasePath = filename;

            if (!File.Exists(filename))
            {
                Database.EnsureCreated();
                Metadata.Add(new DbMetadata() { DbVersion = DbVersion });
                SaveChanges();
            }

            if (Metadata.ToArray()[0].DbVersion != DbVersion)
                Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalendarEvent>().Property(p => p.RepeatOnDays)
                .HasConversion(v => v.ToCommaSeparatedString(), v => v.FromCommaSeparatedString<DayOfWeek>());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=" + ApplicationDatabasePath);
        }
    }
}
