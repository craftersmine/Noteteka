using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using App.Core;
using App.Storage;
using Path = System.IO.Path;
using Timer = System.Timers.Timer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static string ApplicationDataStoragePath { get; private set; }

        public static ApplicationDatabaseContext DatabaseContext { get; private set; }

        public static bool DatabaseRestored { get; private set; }

        public static Version CurrentVersion => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        public static string CurrentVersionString => string.Format("v{1}.{2}.{3}", CurrentVersion.Major,
            CurrentVersion.Minor, CurrentVersion.Revision);

        public static NotificationService NotificationService { get; private set; }

        public static Timer EventTimer { get; private set; }

        public static bool ColdStart { get; set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            ColdStart = true;
            EventTimer = new Timer(TimeSpan.FromSeconds(10).TotalMilliseconds);

            this.InitializeComponent();
        }

        private void Current_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            new MessageDialog("An unhandled exception has occurred! " + e.Exception.Message, "Error!").ShowAsync();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            ApplicationDataStoragePath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "YourNote");

            if (!Directory.Exists(ApplicationDataStoragePath))
                Directory.CreateDirectory(ApplicationDataStoragePath);

            new ApplicationSettingsManager();

            if (Environment.GetCommandLineArgs().Contains("--purge-db"))
            {
                PurgeDatabase();
            }

            Application.Current.UnhandledException += Current_UnhandledException;

            try
            {
                DatabaseContext = new ApplicationDatabaseContext();
            }
            catch (Exception e)
            {
                BackupDatabase();
                PurgeDatabase();
                DatabaseContext = new ApplicationDatabaseContext();
            }

            NotificationService = new NotificationService();

            m_window = new MainWindow();
            EventTimer.Start();

            NotificationService.UpdateCalendarEvents();

            m_window.Activate();
        }

        private Window m_window;

        private void PurgeDatabase()
        {
            string dbFile = Path.Combine(ApplicationDataStoragePath, "DataStorage.db");
                
            File.Delete(dbFile);
            File.Delete(dbFile + "-shm");
            File.Delete(dbFile + "-wal");
        }

        public void BackupDatabase()
        {
            string dbFile = Path.Combine(ApplicationDataStoragePath, "DataStorage.db");

            File.Copy(dbFile, Path.ChangeExtension(dbFile, ".db-backup"));
        }
    }
}
