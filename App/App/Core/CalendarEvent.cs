using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core
{
    public class CalendarEvent
    {
        [Key]
        public int Id { get; set; }
        public DateTime EventDateTime { get; set; }
        public DayOfWeek[] RepeatOnDays { get; set; }
        public TimeSpan RepeatEvery { get; set; }
        public TimeSpan RemindBefore { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsRepeating { get; set; }
        public bool IsDone { get; set; }
        public bool IsNotificationShown { get; set; }

        public CalendarEvent()
        {
            EventDateTime = DateTime.Today;
            Description = "New event";
        }

        public void OnNotificationShown()
        {
            IsNotificationShown = true;
        }

        public void OnEventOccurred()
        {
            if (!IsRepeating)
            {
                IsDone = true;
                return;
            }

            if (RepeatOnDays.Length == 1)
            {
                EventDateTime = Utilities.GetNextWeekday(DateTime.Now.AddDays(1), RepeatOnDays[0]) + RepeatEvery;
            }

            DayOfWeek nextWeekDay = DateTime.Now.DayOfWeek;

            for (int i = 0; i < RepeatOnDays.Length; i++)
            {
                if (RepeatOnDays[i] + 7 > nextWeekDay)
                {
                    nextWeekDay = RepeatOnDays[i];
                    break;
                }
            }

            EventDateTime = Utilities.GetNextWeekday(DateTime.Now.AddDays(1), nextWeekDay) + RepeatEvery;

            App.DatabaseContext.CalendarEvents.Update(this);
            App.DatabaseContext.SaveChanges();
        }
    }
}
