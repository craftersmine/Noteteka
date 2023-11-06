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
using System.Reflection;
using System.Runtime.InteropServices;
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

        public static string ApplicationCrashReportStoragePath { get; private set; }

        public static ApplicationDatabaseContext DatabaseContext { get; private set; }

        public static bool DatabaseRestored { get; private set; }

        public static Version CurrentVersion => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        public static string CurrentVersionString => System.Reflection.Assembly.GetExecutingAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        public static string AppName => System.Reflection.Assembly.GetExecutingAssembly()
            ?.GetCustomAttribute<AssemblyProductAttribute>().Product;
        public static string AppCopyright => System.Reflection.Assembly.GetExecutingAssembly()
            ?.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
        public static string AppDescription => System.Reflection.Assembly.GetExecutingAssembly()
            ?.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;

        public static NotificationService NotificationService { get; private set; }

        public static Timer EventTimer { get; private set; }

        public static bool ColdStart { get; set; }

        public static MainWindow MainWindow { get; set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Application.Current.UnhandledException += Current_UnhandledException;
            App.Current.UnhandledException += Current_UnhandledException;

            ColdStart = true;
            EventTimer = new Timer(TimeSpan.FromSeconds(10).TotalMilliseconds);

            this.InitializeComponent();
        }

        private const string CrashReportTemplate = 
@"Noteteka has crashed!

This file contains technical information about application crash and doesn't contain personal information!
We recommend you to send this report to developers here:
https://github.com/craftersmine/Noteteka/issues

- OS Version: {0}
- Application Version: {1}
- Application Informational Version: {2}
- System Date: {3}

Crash related info:
{4}
";

        private const string ExceptionTemplate =
            @"Exception: {0}
Message: {1}
HResult: 0x{2}
Stack trace:
{3}
";

        private void Current_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            string exceptionInfo = GetExceptionInfo(e.Exception);
            string crashReport = string.Format(CrashReportTemplate, 
                Environment.OSVersion.ToString(),
                CurrentVersion, CurrentVersionString, DateTime.Now.ToString("dd.MM.yyyy-hh:mm:sszzz"), exceptionInfo);

            string crashReportFileName = string.Format("crash-report_{0}.log", DateTime.Now.ToString("ddMMyyyy-hhmmss"));
            string crashReportFilePath = Path.Combine(ApplicationCrashReportStoragePath, crashReportFileName);

            File.WriteAllText(crashReportFilePath, crashReport);
        }

        private string GetExceptionInfo(Exception? exception)
        {
            if (exception is null)
                return "Exception is not recorded or end of exception chain!";
            string exceptionInfo = string.Format(ExceptionTemplate, exception.GetType().FullName, exception.Message,
                exception.HResult.ToString("x8"), GetStackTrace(exception));
            if (exception.InnerException is not null)
            {
                string innerExceptionInfo = GetExceptionInfo(exception.InnerException);
                exceptionInfo += "-------------------" + Environment.NewLine + innerExceptionInfo;
            }

            return exceptionInfo;
        }

        private string GetStackTrace(Exception ex)
        {
            List<string> lines = new List<string>();
            if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                foreach (var line in ex.StackTrace.Split(new string[] { Environment.NewLine, "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    lines.Add($"{line}");
            else lines.Add("No Exception Stacktrace recorded!");
            return string.Join(Environment.NewLine, lines.ToArray());
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            ApplicationDataStoragePath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Noteteka");
            ApplicationCrashReportStoragePath = Path.Combine(ApplicationDataStoragePath, "crash-reports");

            Directory.CreateDirectory(ApplicationDataStoragePath);
            Directory.CreateDirectory(ApplicationCrashReportStoragePath);

            if (Environment.GetCommandLineArgs().Contains("--purge-db"))
            {
                PurgeDatabase();
            }

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
