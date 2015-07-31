using GravitasApp.Helpers;
using GravitasApp.Managers;
using GravitasSDK.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Calls;
using Windows.ApplicationModel.Email;


namespace GravitasApp
{

    public sealed partial class EventPage : Page, IManageable
    {
        public CategoryMetadata CategoryInfo { get; private set; }
        public Event ContextEvent { get; private set; }
        public Visibility TeamSizePopupButtonVisibility { get; private set; }
        public List<Uri> ChapterImages { get; private set; }

        public EventPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PageManager.RegisterPage(this);
            Event ev = DataManager.TryGetEvent(e.Parameter as string);
            if (ev == null)
                return;
            else
            {
                ContextEvent = ev;
                CategoryInfo = CategoryMetadata.GetMetadata(ev.Category);
                if (ev.TeamSizes.Count > 1)
                    TeamSizePopupButtonVisibility = Windows.UI.Xaml.Visibility.Visible;
                else
                    TeamSizePopupButtonVisibility = Windows.UI.Xaml.Visibility.Collapsed;

                ChapterImages = new List<Uri>();
                foreach (string chapter in ev.AssociatedChapters)
                {
                    if (string.Equals(chapter, "individual", StringComparison.OrdinalIgnoreCase) == true)
                        break;
                    ChapterImages.Add(new Uri(String.Format("ms-appx:///Assets/ChapterLogos/{0}.png", chapter.Replace(" ", ""))));
                }
                shortlistButton.IsChecked = DataManager.IsShortlisted(ContextEvent);
                this.DataContext = this;
            }
        }

        public Dictionary<string, object> SaveState()
        {
            return null;
        }

        public void LoadState(Dictionary<string, object> lastState)
        {
        }

        public bool AllowAppExit()
        {
            return true;
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            teamSizeFlyout.ShowAt(sender as FrameworkElement);
        }

        private void WrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            (sender as WrapGrid).ItemHeight = (sender as WrapGrid).ItemWidth = e.NewSize.Width / 3;
        }

        private async void ReadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(CategoryInfo.WebLink));
        }

        private async void ContactButton_Click(object sender, RoutedEventArgs e)
        {
            if ((ContextEvent.Emails.Count + ContextEvent.Coordinators.Count) == 0)
            {
                await new MessageDialog("There are no contact options for the organizers of this event.", "Sorry").ShowAsync();
                return;
            }

            contactPickerFlyout.ShowAt(this as FrameworkElement);
        }

        private void CoordinatorItem_Click(object sender, ItemClickEventArgs e)
        {
            Coordinator c = e.ClickedItem as Coordinator;
            PhoneCallManager.ShowPhoneCallUI(c.Phone, c.Name);
        }

        private async void EmailItem_Click(object sender, ItemClickEventArgs e)
        {
            string email = e.ClickedItem as string;
            EmailMessage mailMsg = new EmailMessage();
            mailMsg.To.Add(new EmailRecipient(email));
            mailMsg.Subject = "Query - " + ContextEvent.Title;
            await EmailManager.ShowComposeNewEmailAsync(mailMsg);
        }

        private void ShortlistButton_Click(object sender, RoutedEventArgs e)
        {
            DataManager.UpdateShortlist(ContextEvent, (bool)shortlistButton.IsChecked);
        }
    }
}
