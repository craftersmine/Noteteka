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

        private void UpdateCalendarEvents()
        {
            CalendarEventsListBox.ItemsSource = null;
            CalendarEventsListBox.Items.Clear();

            if (App.DatabaseContext.CalendarEvents.Any())
            {
                DateTime nextWeek = DateTime.Now + TimeSpan.FromDays(7);
                App.DatabaseContext.SaveChanges();
                CalendarEventsListBox.Visibility = Visibility.Visible;
                CalendarEventsListBox.ItemsSource = App.DatabaseContext.CalendarEvents;
                //.Where(e => e.EventDateTime >= DateTime.Now).Where(e => e.EventDateTime <= nextWeek)
                //.OrderByDescending(e => e.EventDateTime).ToList();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SwitchEnabled(false);
            UpdateCalendarEvents();
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

            UpdateCalendarEvents();
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
        }

        private async void OnDeleteEventClick(object sender, RoutedEventArgs e)
        {
            RemoveButton.Flyout.Hide();

            CalendarEvent evt = CalendarEventsListBox.SelectedItem as CalendarEvent;

            App.DatabaseContext.CalendarEvents.Remove(evt);
            await App.DatabaseContext.SaveChangesAsync();

            UpdateCalendarEvents();
        }
    }
}
