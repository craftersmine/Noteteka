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
        private static readonly Guid DbVersion = new Guid("caca241e-294a-4c28-87e7-012dafcd8f08");

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
            {
                Database.Migrate();
                Metadata.First().DbVersion = DbVersion;
                SaveChanges();
            }
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
