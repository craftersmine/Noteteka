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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using App.Storage;
using Path = System.IO.Path;

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

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
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

            if (Environment.GetCommandLineArgs().Contains("--purge-db"))
            {
                string dbFile = Path.Combine(ApplicationDataStoragePath, "DataStorage.db");
                
                File.Delete(dbFile);
                File.Delete(dbFile + "-shm");
                File.Delete(dbFile + "-wal");
            }

            Application.Current.UnhandledException += Current_UnhandledException;

            DatabaseContext = new ApplicationDatabaseContext();

            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window m_window;
    }
}
