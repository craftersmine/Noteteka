using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core
{
    public class EveryDayEventRepeat : IEventRepeat
    {
        public DateTime NextEventOccurrence(CalendarEvent calendarEvent)
        {
            return calendarEvent.EventDateTime + TimeSpan.FromDays(1);
        }
    }

    public class EveryWeekdayEventRepeat : IEventRepeat
    {
        public DayOfWeek[] Weekdays { get; set; }

        public EveryWeekdayEventRepeat(DayOfWeek[] weekdays)
        {
            Weekdays = weekdays;
        }

        public DateTime NextEventOccurrence(CalendarEvent calendarEvent)
        {
            if (Weekdays.Length == 1)
            {
                return Utilities.GetNextWeekday(DateTime.Now.AddDays(1), Weekdays[0]);
            }

            DayOfWeek nextWeekDay = DateTime.Now.DayOfWeek;

            for (int i = 0; i < Weekdays.Length; i++)
            {
                if (Weekdays[i] + 7 > nextWeekDay)
                {
                    nextWeekDay = Weekdays[i];
                    break;
                }
            }

            return Utilities.GetNextWeekday(DateTime.Now.AddDays(1), nextWeekDay);
        }
    }
}
