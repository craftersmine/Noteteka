using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Pages;

namespace App.Core
{
    public class ApplicationSettings
    {
        public FilterType CalendarEventsFilter { get; set; } = FilterType.AllEvents;
    }
}
