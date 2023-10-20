using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using App.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

namespace App.Core
{
    public class NotificationService
    {
        public const string OneEventAboutOccurred = "OneCalendarEventOccurred";
        public const string OneEventAboutToOccur = "OneCalendarEventAboutToOccur";
        public const string OneEventAboutMissed = "OneCalendarEventMissed";
        public const string MultipleEventAboutOccurred = "MultipleCalendarEventOccurred";
        public const string MultipleEventsAboutToOccur = "MultipleCalendarEventsAboutToOccur";
        public const string MultipleEventsMissed = "MultipleCalendarEventsMissed";


        public NotificationService()
        {
            App.EventTimer.Elapsed += EventTimer_Elapsed;
            AppNotificationManager.Default.NotificationInvoked += NotificationInvoked;
            AppNotificationManager.Default.Register();
        }

        private async void NotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            if (args.Arguments.ContainsKey("EventDone") && args.Arguments.ContainsKey("EventId"))
            {
                int eventId = int.Parse(args.Arguments["EventId"]);
                bool isDone = bool.Parse(args.Arguments["EventDone"]);

                CalendarEvent evt = App.DatabaseContext.CalendarEvents.First(e => e.Id == eventId);
                if (isDone)
                {
                    evt.IsDone = isDone;
                    evt.OnNotificationShown();
                    evt.OnEventOccurred();
                }
                else
                {
                    evt.OnNotificationShown();
                    evt.IsDismissed = true;
                }
                App.DatabaseContext.CalendarEvents.Entry(evt).State = EntityState.Modified;
                await App.DatabaseContext.SaveChangesAsync();
            }
        }

        private void EventTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateCalendarEvents();
        }

        public async void UpdateCalendarEvents()
        {
            // TODO: figure out how to reimplement calendar events handling
            
            CalendarEvent[] events =
                App.DatabaseContext.CalendarEvents.ToArray();
            CalendarEvent[] currentEvents = events.Where(e => e.EventDateTime == DateTime.Now.TrimSeconds() && !e.IsNotificationShown && !e.IsDone).ToArray();
            CalendarEvent[] eventsToRemind = events.Where(e => e.EventDateTime >= DateTime.Now.TrimSeconds() - e.RemindBefore && !e.IsNotificationShown && !e.IsDone && e.RemindBefore > TimeSpan.Zero).Except(currentEvents).ToArray();
            CalendarEvent[] missedEvents = events.Where(e => e.EventDateTime <= DateTime.Now.TrimSeconds() && (!e.IsNotificationShown || e.IsDismissed) && !e.IsDone).Except(currentEvents).ToArray();

            if (currentEvents.Length > 1)
            {
                ShowCalendarEventsOccurred(currentEvents.Length);
                foreach (CalendarEvent evt in currentEvents)
                {
                    evt.OnEventOccurred();
                }
            }
            else if (currentEvents.Length == 1)
            {
                ShowCalendarEventOccurred(currentEvents[0]);
            }

            if (eventsToRemind.Length > 1)
            {
                ShowCalendarEventsAboutToOccur(eventsToRemind.Length);
                foreach (CalendarEvent evt in eventsToRemind)
                {
                    evt.OnNotificationShown();
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
                    evt.OnNotificationShown();
                }
            }
            else if (missedEvents.Length == 1)
            {
                ShowCalendarEventMissed(missedEvents[0]);
            }

            await App.DatabaseContext.SaveChangesAsync();
        }

        public void ShowCalendarEventOccurred(CalendarEvent evt)
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

            evt.OnNotificationShown();
            evt.OnEventOccurred();

            AppNotification notification = new AppNotificationBuilder()
                .SetAudioEvent(AppNotificationSoundEvent.Reminder, AppNotificationAudioLooping.None)
                .AddText("Event is happening right now!").AddText(evt.EventDateTime.ToString("U"))
                .AddText(notificationText).AddArgument("CalendarEventId", evt.Id.ToString()).AddButton(doneButton).AddButton(dismissButton).BuildNotification();
            notification.Priority = AppNotificationPriority.High;
            notification.ExpiresOnReboot = false;
            notification.Tag = OneEventAboutOccurred;
            AppNotificationManager.Default.Show(notification);
            evt.IsNotificationShown = true;
            App.DatabaseContext.CalendarEvents.Entry(evt).State = EntityState.Modified;
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

            evt.OnEventOccurred();

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

        public void ShowCalendarEventsOccurred(int amount)
        {
            AppNotification notification = new AppNotificationBuilder()
                .AddText(string.Format("There is {0} events happening right now!", amount)).AddText("You should take a look!")
                .BuildNotification();
            notification.Priority = AppNotificationPriority.High;
            notification.ExpiresOnReboot = false;
            notification.Tag = MultipleEventAboutOccurred;
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
