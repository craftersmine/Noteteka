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
using App.Dialogs;
using Microsoft.EntityFrameworkCore;

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
            SwitchEnabled(false);
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

        private void UpdateCalendarEvents(FilterType filter, bool showDoneEvents)
        {
            CalendarEventsListBox.ItemsSource = null;
            CalendarEventsListBox.Items.Clear();

            if (App.DatabaseContext.CalendarEvents.Any())
            {
                switch (filter)
                {
                    case FilterType.AllEvents:
                        CalendarEventsListBox.Visibility = Visibility.Visible;
                        CalendarEventsListBox.ItemsSource = App.DatabaseContext.CalendarEvents.Where(e => showDoneEvents ? e.IsDone || !e.IsDone : !e.IsDone);
                        break;
                    case FilterType.Today:
                        DateTime today = DateTime.Today + TimeSpan.FromHours(23) + TimeSpan.FromMinutes(59);
                        CalendarEventsListBox.Visibility = Visibility.Visible;
                        CalendarEventsListBox.ItemsSource = App.DatabaseContext.CalendarEvents
                            .Where(e => e.EventDateTime >= DateTime.Today)
                            .Where(e => e.EventDateTime <= today)
                            .Where(e => showDoneEvents ? e.IsDone || !e.IsDone : !e.IsDone);
                        break;
                    case FilterType.TodayOneWeek:
                        DateTime nextWeek = DateTime.Today + TimeSpan.FromDays(7) + TimeSpan.FromHours(23) + TimeSpan.FromMinutes(59);
                        CalendarEventsListBox.Visibility = Visibility.Visible;
                        CalendarEventsListBox.ItemsSource = App.DatabaseContext.CalendarEvents
                            .Where(e => e.EventDateTime >= DateTime.Today)
                            .Where(e => e.EventDateTime <= nextWeek)
                            .Where(e => showDoneEvents ? e.IsDone || !e.IsDone : !e.IsDone);
                        break;
                    case FilterType.TodayOneMonth:
                        DateTime nextMonth = DateTime.Today + TimeSpan.FromDays(30) + TimeSpan.FromHours(23) + TimeSpan.FromMinutes(59);
                        CalendarEventsListBox.Visibility = Visibility.Visible;
                        CalendarEventsListBox.ItemsSource = App.DatabaseContext.CalendarEvents
                            .Where(e => e.EventDateTime >= DateTime.Today)
                            .Where(e => e.EventDateTime <= nextMonth)
                            .Where(e => showDoneEvents ? e.IsDone || !e.IsDone : !e.IsDone);
                        break;
                }

                //.Where(e => e.EventDateTime >= DateTime.Now).Where(e => e.EventDateTime <= nextWeek)
                //.OrderByDescending(e => e.EventDateTime).ToList();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SwitchEnabled(false);
            foreach (RadioMenuFlyoutItem item in FilterFlyout.Items.Where(i => i.GetType() == typeof(RadioMenuFlyoutItem)))
            {
                item.IsChecked = false;
            }

            ((RadioMenuFlyoutItem)FilterFlyout.Items.First(i => int.Parse(((RadioMenuFlyoutItem)i).Tag.ToString()) == (int)ApplicationSettingsManager.Instance.Settings.CalendarEventsFilter)).IsChecked = true;
            UpdateCalendarEvents(ApplicationSettingsManager.Instance.Settings.CalendarEventsFilter, ApplicationSettingsManager.Instance.Settings.ShowDoneEventsInCalendar);
            base.OnNavigatedTo(e);
        }

        private void OnSyncClick(object sender, RoutedEventArgs e)
        {
            Sync();
        }

        private async void OnAddEventClick(object sender, RoutedEventArgs e)
        {
            ContentDialog dlg = new ContentDialog();
            AddEditCalendarEventDialog dlgContent = new AddEditCalendarEventDialog(null);
            dlg.XamlRoot = this.XamlRoot;
            dlg.Title = "Add new calendar event";
            dlg.Content = dlgContent;
            dlg.CloseButtonText = "Cancel";
            dlg.PrimaryButtonText = "Add";
            dlg.CloseButtonClick += (dialog, args) => dialog.Hide();
            dlg.PrimaryButtonClick += AddEditPrimaryButtonClick;
            dlg.IsPrimaryButtonEnabled = false;
            await dlg.ShowAsync();
        }

        private async void AddEditPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            AddEditCalendarEventDialog dlgContent = (AddEditCalendarEventDialog)sender.Content;

            if (!dlgContent.IsEditing)
            {
                CalendarEvent evt = new CalendarEvent();
                evt.Title = dlgContent.EventTitle;
                evt.Description = dlgContent.EventDescription;
                evt.EventDateTime = dlgContent.EventDateTime;
                evt.RepeatOnDays = dlgContent.RepeatOn;
                evt.RemindBefore = dlgContent.RemindBefore;
                evt.IsRepeating = dlgContent.Repeat;
                evt.RepeatEvery = dlgContent.RepeatEvery;
                App.DatabaseContext.CalendarEvents.Add(evt);
            }
            else
            {
                CalendarEvent evt = App.DatabaseContext.CalendarEvents.First(e => e.Id == dlgContent.CalendarEvent.Id);
                evt.Title = dlgContent.EventTitle;
                evt.Description = dlgContent.EventDescription;
                evt.EventDateTime = dlgContent.EventDateTime;
                evt.RepeatOnDays = dlgContent.RepeatOn;
                evt.RemindBefore = dlgContent.RemindBefore;
                evt.IsRepeating = dlgContent.Repeat;
                evt.RepeatEvery = dlgContent.RepeatEvery;
                App.DatabaseContext.CalendarEvents.Entry(evt).State = EntityState.Modified;
            }
            await App.DatabaseContext.SaveChangesAsync();
            sender.PrimaryButtonClick -= AddEditPrimaryButtonClick;

            UpdateCalendarEvents(ApplicationSettingsManager.Instance.Settings.CalendarEventsFilter, ApplicationSettingsManager.Instance.Settings.ShowDoneEventsInCalendar);
        }

        private async void OnEditEventClick(object sender, RoutedEventArgs e)
        {
            CalendarEvent evt = CalendarEventsListBox.SelectedItem as CalendarEvent;

            ContentDialog dlg = new ContentDialog();
            AddEditCalendarEventDialog dlgContent = new AddEditCalendarEventDialog(evt);
            dlg.XamlRoot = this.XamlRoot;
            dlg.Title = "Edit calendar event";
            dlg.Content = dlgContent;
            dlg.CloseButtonText = "Cancel";
            dlg.PrimaryButtonText = "Confirm";
            dlg.CloseButtonClick += (dialog, args) => dialog.Hide();
            dlg.PrimaryButtonClick += AddEditPrimaryButtonClick;
            dlg.IsPrimaryButtonEnabled = true;
            await dlg.ShowAsync();
        }

        private void OnEventSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                SwitchEnabled(true);
            else
                SwitchEnabled(false);
        }

        private void SwitchEnabled(bool value)
        {
            EditButton.IsEnabled = value;
            RemoveButton.IsEnabled = value;
            DoneButton.IsEnabled = value;
        }

        private async void OnDeleteEventClick(object sender, RoutedEventArgs e)
        {
            RemoveButton.Flyout.Hide();

            CalendarEvent evt = CalendarEventsListBox.SelectedItem as CalendarEvent;

            App.DatabaseContext.CalendarEvents.Remove(evt);
            await App.DatabaseContext.SaveChangesAsync();

            UpdateCalendarEvents(ApplicationSettingsManager.Instance.Settings.CalendarEventsFilter, ApplicationSettingsManager.Instance.Settings.ShowDoneEventsInCalendar);
        }

        private void OnFilterVariantSelected(object sender, RoutedEventArgs e)
        {
            FilterType type = (FilterType)int.Parse(((RadioMenuFlyoutItem)sender).Tag.ToString());
            UpdateCalendarEvents(type, ApplicationSettingsManager.Instance.Settings.ShowDoneEventsInCalendar);
            ApplicationSettingsManager.Instance.Settings.CalendarEventsFilter = type;
        }

        private async void OnDoneClick(object sender, RoutedEventArgs e)
        {
            CalendarEvent evt = CalendarEventsListBox.SelectedItem as CalendarEvent;
            evt.IsDone = true;
            App.DatabaseContext.CalendarEvents.Entry(evt).State = EntityState.Modified;
            await App.DatabaseContext.SaveChangesAsync();
            UpdateCalendarEvents(ApplicationSettingsManager.Instance.Settings.CalendarEventsFilter, ApplicationSettingsManager.Instance.Settings.ShowDoneEventsInCalendar);
        }

        private void OnShowDoneFilterEnabled(object sender, RoutedEventArgs e)
        {
            ApplicationSettingsManager.Instance.Settings.ShowDoneEventsInCalendar =
                ((ToggleMenuFlyoutItem)sender).IsChecked;
            UpdateCalendarEvents(ApplicationSettingsManager.Instance.Settings.CalendarEventsFilter, ApplicationSettingsManager.Instance.Settings.ShowDoneEventsInCalendar);
        }
    }

    public enum FilterType
    {
        AllEvents = 1,
        Today = 2,
        TodayOneWeek = 3,
        TodayOneMonth = 4
    }
}
