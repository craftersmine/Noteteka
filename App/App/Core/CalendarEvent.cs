using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core
{
    public class CalendarEvent
    {
        public int Id { get; set; }
        public DateTime EventDateTime { get; set; }
        // TODO (craftersmine): fix db integration. implement JSON serialization?
        public IEventRepeat RepeatAction { get; set; }
        public string Description { get; set; }
        public bool IsRepeating { get; set; }
        public bool IsDone { get; set; }

        public CalendarEvent()
        {
            EventDateTime = DateTime.Today;
            Description = "New event";
        }

        public async void OnEventOccurred()
        {
            //CalendarEvent thisEvent = App.DatabaseContext.CalendarEvents.First(evt => evt.Id == this.Id);
            if (IsRepeating)
            {
                EventDateTime = RepeatAction.NextEventOccurrence(this);

                //thisEvent.EventDateTime = EventDateTime;
            }
            else
            {
                IsDone = true;
                //thisEvent.IsDone = true;
            }

            //await App.DatabaseContext.SaveChangesAsync();
        }
    }
}
