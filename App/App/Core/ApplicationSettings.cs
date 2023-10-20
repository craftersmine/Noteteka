using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using App.Pages;

namespace App.Core
{
    public class ApplicationSettings : INotifyPropertyChanged
    {
        private FilterType calendarEventsFilterType = FilterType.AllEvents;
        private bool showDoneEventsInCalendar = false;

        public FilterType CalendarEventsFilter
        {
            get => calendarEventsFilterType;
            set
            {
                calendarEventsFilterType = value;
                OnPropertyChanged();
            }
        }

        public bool ShowDoneEventsInCalendar
        {
            get => showDoneEventsInCalendar;
            set
            {
                showDoneEventsInCalendar = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
