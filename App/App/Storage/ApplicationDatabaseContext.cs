using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core;
using Microsoft.EntityFrameworkCore;

namespace App.Storage
{
    public class ApplicationDatabaseContext : DbContext
    {
        public string ApplicationDatabasePath { get; private set; }

        public DbSet<StickyNote> StickyNotes { get; set; }

        public ApplicationDatabaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
