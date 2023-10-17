using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

namespace App.Core
{
    public class NotificationService
    {
        public NotificationService()
        {
            App.EventTimer.Elapsed += EventTimer_Elapsed;
            AppNotificationManager.Default.NotificationInvoked += NotificationInvoked;
            AppNotificationManager.Default.Register();
        }

        private void NotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {

        }

        private void EventTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateCalendarEvents();
        }

        private void UpdateCalendarEvents()
        {
            CalendarEvent[] events =
                App.DatabaseContext.CalendarEvents.ToArray();
            CalendarEvent[] eventsToRemind = events.Where(e => e.EventDateTime >= DateTime.Now - e.RemindBefore && !e.IsNotificationShown).ToArray();

            if (eventsToRemind.Length > 1)
            {
                ShowCalendarEventsAboutToOccur(eventsToRemind.Length);
                foreach (CalendarEvent evt in eventsToRemind)
                {
                    evt.IsNotificationShown = true;
                }
            }

            App.DatabaseContext.SaveChanges();
        }

        public void ShowCalendarEventsAboutToOccur(int amount)
        {
            AppNotification notification = new AppNotificationBuilder()
                .AddText(string.Format("There is {0} events upcoming!", amount)).AddText("You should take a look!")
                .BuildNotification();
            notification.Priority = AppNotificationPriority.High;
            notification.ExpiresOnReboot = false;
            AppNotificationManager.Default.Show(notification);
        }

        public void ShowCalendarEventOccurred(CalendarEvent evt)
        {
            AppNotification notification = new AppNotificationBuilder()
                .SetAudioEvent(AppNotificationSoundEvent.Reminder, AppNotificationAudioLooping.None)
                .AddText(string.Format("Event approaching in around {0}!", evt.RemindBefore.ToString()))
                .AddText(evt.Title).AddText(evt.Description).AddArgument("CalendarEventId", evt.Id.ToString()).BuildNotification();
            notification.Priority = AppNotificationPriority.High;
            notification.ExpiresOnReboot = false;
            AppNotificationManager.Default.Show(notification);
            evt.IsNotificationShown = true;
            App.DatabaseContext.CalendarEvents.Entry(evt).State = EntityState.Modified;
        }
    }
}
