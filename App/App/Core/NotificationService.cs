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
        public const string OneEventAboutToOccur = "OneCalendarEventAboutToOccur";
        public const string OneEventAboutMissed = "OneCalendarEventMissed";
        public const string MultipleEventsAboutToOccur = "MultipleCalendarEventsAboutToOccur";
        public const string MultipleEventsMissed = "MultipleCalendarEventsMissed";


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

        public void UpdateCalendarEvents()
        {
            CalendarEvent[] events =
                App.DatabaseContext.CalendarEvents.ToArray();
            CalendarEvent[] eventsToRemind = events.Where(e => e.EventDateTime >= DateTime.Now - e.RemindBefore && !e.IsNotificationShown && !e.IsDone).ToArray();
            CalendarEvent[] missedEvents = events.Where(e => e.EventDateTime <= DateTime.Now && !e.IsNotificationShown && !e.IsDone).ToArray();

            if (eventsToRemind.Length > 1)
            {
                ShowCalendarEventsAboutToOccur(eventsToRemind.Length);
                foreach (CalendarEvent evt in eventsToRemind)
                {
                    evt.IsNotificationShown = true;
                }
            }
            else if (eventsToRemind.Length == 1)
            {
                ShowCalendarEventAboutToOccur(eventsToRemind[0]);
            }

            if (missedEvents.Length > 1)
            {
                ShowCalendarEventsMissed(missedEvents.Length);
                foreach (CalendarEvent evt in missedEvents)
                {
                    evt.IsNotificationShown = true;
                }
            }
            else if (missedEvents.Length == 1)
            {
                ShowCalendarEventMissed(missedEvents[0]);
            }

            App.DatabaseContext.SaveChanges();
        }

        public void ShowCalendarEventMissed(CalendarEvent evt)
        {
            AppNotificationButton doneButton = new AppNotificationButton();
            doneButton.Content = "Done";
            doneButton.AddArgument("EventDone", "True");
            doneButton.AddArgument("EventId", evt.Id.ToString());
            AppNotificationButton dismissButton = new AppNotificationButton();
            dismissButton.Content = "Dismiss";
            dismissButton.AddArgument("EventDone", "False");
            dismissButton.AddArgument("EventId", evt.Id.ToString());

            string notificationText = string.Format("{0}{1}{0}{2}", Environment.NewLine,
                evt.Title, evt.Description);
            
            AppNotification notification = new AppNotificationBuilder()
                .SetAudioEvent(AppNotificationSoundEvent.Reminder, AppNotificationAudioLooping.None)
                .AddText("Event missed!").AddText(evt.EventDateTime.ToString("U"))
                .AddText(notificationText).AddArgument("CalendarEventId", evt.Id.ToString()).AddButton(doneButton).AddButton(dismissButton).BuildNotification();
            notification.Priority = AppNotificationPriority.High;
            notification.ExpiresOnReboot = false;
            notification.Tag = OneEventAboutMissed;
            AppNotificationManager.Default.Show(notification);
            evt.IsNotificationShown = true;
            App.DatabaseContext.CalendarEvents.Entry(evt).State = EntityState.Modified;
        }

        public void ShowCalendarEventsMissed(int amount)
        {
            AppNotification notification = new AppNotificationBuilder()
                .AddText(string.Format("There is {0} events missed!", amount)).AddText("You should take a look!")
                .BuildNotification();
            notification.Priority = AppNotificationPriority.High;
            notification.ExpiresOnReboot = false;
            notification.Tag = MultipleEventsMissed;
            AppNotificationManager.Default.Show(notification);
        }

        public void ShowCalendarEventsAboutToOccur(int amount)
        {
            AppNotification notification = new AppNotificationBuilder()
                .AddText(string.Format("There is {0} events upcoming!", amount)).AddText("You should take a look!")
                .BuildNotification();
            notification.Priority = AppNotificationPriority.High;
            notification.ExpiresOnReboot = false;
            notification.Tag = MultipleEventsAboutToOccur;
            AppNotificationManager.Default.Show(notification);
        }

        public void ShowCalendarEventAboutToOccur(CalendarEvent evt)
        {
            AppNotificationButton doneButton = new AppNotificationButton();
            doneButton.Content = "Done";
            doneButton.AddArgument("EventDone", "True");
            doneButton.AddArgument("EventId", evt.Id.ToString());
            AppNotificationButton dismissButton = new AppNotificationButton();
            dismissButton.Content = "Dismiss";
            dismissButton.AddArgument("EventDone", "False");
            dismissButton.AddArgument("EventId", evt.Id.ToString());
            
            string notificationText = string.Format("{0}{1}{0}{2}", Environment.NewLine,
                evt.Title, evt.Description);

            AppNotification notification = new AppNotificationBuilder()
                .SetAudioEvent(AppNotificationSoundEvent.Reminder, AppNotificationAudioLooping.None)
                .AddText(string.Format("Event approaching in around {0}!", evt.RemindBefore.ToString())).AddText(evt.EventDateTime.ToString("U"))
                .AddText(notificationText).AddArgument("CalendarEventId", evt.Id.ToString()).AddButton(doneButton).AddButton(dismissButton).BuildNotification();
            notification.Priority = AppNotificationPriority.High;
            notification.ExpiresOnReboot = false;
            notification.Tag = OneEventAboutToOccur;
            AppNotificationManager.Default.Show(notification);
            evt.IsNotificationShown = true;
            App.DatabaseContext.CalendarEvents.Entry(evt).State = EntityState.Modified;
        }
    }
}
