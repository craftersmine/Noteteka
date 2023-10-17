using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using App.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalendarPage : Page
    {
        public CalendarPage()
        {
            this.InitializeComponent();
        }

        private void Sync()
        {
            App.DatabaseContext.CalendarEvents.Add(new CalendarEvent()
            {
                Title = "Electrotechnics",
                Description = "307 - practice",
                EventDateTime = new DateTime(2023, 10, 16, 20, 00, 00),
                IsRepeating = true,
                RepeatEvery = TimeSpan.FromDays(14),
                RepeatOnDays = new DayOfWeek[] { DayOfWeek.Monday },
                RemindBefore = TimeSpan.FromMinutes(30)
            });
            App.DatabaseContext.SaveChanges();
        }

        private void UpdateCalendarEvents()
        {
            CalendarEventsListBox.ItemsSource = null;
            CalendarEventsListBox.Items.Clear();

            if (App.DatabaseContext.CalendarEvents.Any())
            {
                DateTime nextWeek = DateTime.Now + TimeSpan.FromDays(7); 
                App.DatabaseContext.SaveChanges();
                CalendarEventsListBox.Visibility = Visibility.Visible;
                CalendarEventsListBox.ItemsSource = App.DatabaseContext.CalendarEvents
                    .Where(e => e.EventDateTime >= DateTime.Now).Where(e => e.EventDateTime <= nextWeek)
                    .OrderByDescending(e => e.EventDateTime).ToList();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateCalendarEvents();
            base.OnNavigatedTo(e);
        }

        private void OnSyncClick(object sender, RoutedEventArgs e)
        {
            Sync();
        }
    }
}
