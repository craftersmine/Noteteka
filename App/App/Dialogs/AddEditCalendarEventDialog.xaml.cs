using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using App.Core;
using CommunityToolkit.WinUI.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddEditCalendarEventDialog : Page
    {
        private List<TimeSpan> _repeatEvery = new List<TimeSpan>()
        {
            new TimeSpan(7, 0, 0, 0),
            new TimeSpan(14, 0, 0, 0),
            new TimeSpan(30, 0, 0, 0)
        };

        private bool _isInitialized = false;

        public CalendarEvent? CalendarEvent { get; private set; }

        public bool IsEditing { get; private set; }

        public string EventTitle => TitleTextBox.Text;
        public string EventDescription => DescriptionTextBox.Text;
        public DateTime EventDateTime => OccursOnDatePicker.SelectedDate.Value.Date + OccursAtTimePicker.SelectedTime.Value;
        public TimeSpan RemindBefore => RemindBeforeTimePicker.SelectedTime ?? TimeSpan.Zero;
        public bool Repeat => RepeatToggle.IsOn;
        public TimeSpan RepeatEvery => (TimeSpan)((ComboBoxItem)RepeatEveryComboBox.SelectedItem).Tag;

        public DayOfWeek[] RepeatOn
        {
            get
            {
                List<DayOfWeek> days = new List<DayOfWeek>();
                foreach (SegmentedItem item in RepeatOnDays.SelectedItems)
                {
                    days.Add((DayOfWeek)int.Parse(item.Tag.ToString()));
                }
                return days.ToArray();
            }
        }

        public AddEditCalendarEventDialog(CalendarEvent? evt)
        {
            CalendarEvent = evt;

            this.InitializeComponent();

            OccursAtTimePicker.Time = DateTime.Now.TimeOfDay;
            OccursOnDatePicker.Date = DateTime.Now;

            RepeatEveryComboBox.Items.Add(new ComboBoxItem() { Content = "Every Week", Tag = _repeatEvery[0] });
            RepeatEveryComboBox.Items.Add(new ComboBoxItem() { Content = "Every Second Week", Tag = _repeatEvery[1] });
            RepeatEveryComboBox.Items.Add(new ComboBoxItem() { Content = "Every Month", Tag = _repeatEvery[2] });

            if (CalendarEvent is not null)
            {
                IsEditing = true;
                TitleTextBox.Text = CalendarEvent.Title;
                DescriptionTextBox.Text = CalendarEvent.Description;
                OccursOnDatePicker.SelectedDate = CalendarEvent.EventDateTime;
                OccursAtTimePicker.SelectedTime = CalendarEvent.EventDateTime.TimeOfDay;
                RemindBeforeTimePicker.SelectedTime = CalendarEvent.RemindBefore;
                RepeatToggle.IsOn = CalendarEvent.IsRepeating;
                foreach (DayOfWeek day in CalendarEvent.RepeatOnDays)
                {
                    RepeatOnDays.SelectedItems.Add(RepeatOnDays.Items.First(d =>
                        ((DayOfWeek)int.Parse(((SegmentedItem)d).Tag.ToString()) == day)));
                }

                RepeatEveryComboBox.SelectedItem = RepeatEveryComboBox.Items.First(t =>
                    (TimeSpan)((ComboBoxItem)t).Tag == CalendarEvent.RepeatEvery);
            }
            else
                CalendarEvent = new CalendarEvent();

            _isInitialized = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CheckValidity();
            base.OnNavigatedTo(e);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();

        }

        private void OnRepeatToggled(object sender, RoutedEventArgs e)
        {
            CheckValidity();
        }

        private void CheckValidity()
        {
            if (Parent is not null)
            {
                if (TitleTextBox.Text.Length > 0 && DescriptionTextBox.Text.Length > 0)
                {
                    if (RepeatToggle.IsOn)
                    {
                        if (RepeatOnDays.SelectedItems.Count > 0)
                            ((ContentDialog)Parent).IsPrimaryButtonEnabled = true;
                        else
                            ((ContentDialog)Parent).IsPrimaryButtonEnabled = false;
                    }
                    else
                        ((ContentDialog)Parent).IsPrimaryButtonEnabled = true;
                }
                else
                    ((ContentDialog)Parent).IsPrimaryButtonEnabled = false;
            }
        }

        private void OnRepeatDaysSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckValidity();
        }
    }
}
