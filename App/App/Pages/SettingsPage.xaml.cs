using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using App.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApplicationSettingsManager.Instance.LastException is not null)
            {
                TopInfoBar.Title = "Error with loading settings!";
                TopInfoBar.Message = string.Format(
                    "There is an error with loading applications settings. Your settings were reset to defaults and old settings were backed up in application data folder.{0}Error Code: 0x{2}{0}Message: {1}",
                    Environment.NewLine, ApplicationSettingsManager.Instance.LastException.Message,
                    ApplicationSettingsManager.Instance.LastException.HResult.ToString("x8"));
                TopInfoBar.Severity = InfoBarSeverity.Error;
            }

            base.OnNavigatedTo(e);
        }

        private void OnDatabasePurgeConfirmClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Windows.AppLifecycle.AppInstance.Restart("--purge-db");
        }

        private void LicenseHyperlinkClick(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo();

            psi.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Licenses", (sender as HyperlinkButton).Tag.ToString() + "_LICENSE.txt");
            psi.UseShellExecute = true;

            if (File.Exists(psi.FileName))
                Process.Start(psi);
            else
            {
                ContentDialog dlg = new ContentDialog();
                dlg.XamlRoot = this.XamlRoot;
                dlg.Title = "Error opening license file";
                dlg.Content = string.Format("Unable to locate \"{0}\" license file at \r\n\"{1}\"!", (sender as HyperlinkButton).Content.ToString(), psi.FileName);
                dlg.CloseButtonText = "Ok";
                dlg.CloseButtonClick += (dialog, args) => dialog.Hide();
                dlg.ShowAsync();
            }
        }
    }
}
