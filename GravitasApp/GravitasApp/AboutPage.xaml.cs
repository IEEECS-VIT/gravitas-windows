using GravitasApp.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Calls;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace GravitasApp
{

    public sealed partial class AboutPage : Page, IManageable
    {

        private DataTransferManager _dataTransferManager;
        public List<Tuple<string, string, string, string>> Contacts { get; private set; }

        public AboutPage()
        {
            Contacts = new List<Tuple<string, string, string, string>>();
            Contacts.Add(new Tuple<string, string, string, string>("REGISTRATION", "Rajlakshmi", "+919500095982", "registrations.gravitas15@vit.ac.in"));
            Contacts.Add(new Tuple<string, string, string, string>("EVENTS", "Chirayu", "+918489996562", "events.gravitas15@vit.ac.in"));
            Contacts.Add(new Tuple<string, string, string, string>("PUBLICITY", "Utkarsh", "+918344557513", "publicity.gravitas15@vit.ac.in"));
            Contacts.Add(new Tuple<string, string, string, string>("CONVENOR", "Prof. CD Naiju", "+919443330174", "convenor.gravitas15@vit.ac.in"));

            _dataTransferManager = DataTransferManager.GetForCurrentView();
            this.InitializeComponent();
            this.DataContext = this;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            PageManager.RegisterPage(this);
            await StatusBar.GetForCurrentView().ShowAsync();
            _dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            await StatusBar.GetForCurrentView().HideAsync();
            _dataTransferManager.DataRequested -= DataTransferManager_DataRequested;
            _dataTransferManager = null;
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

        private async void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            var contact = (e.OriginalSource as FrameworkElement).DataContext as Tuple<string, string, string, string>;
            EmailMessage msg = new EmailMessage();
            msg.To.Add(new EmailRecipient(contact.Item4));
            await EmailManager.ShowComposeNewEmailAsync(msg);
        }

        private void CallButton_Click(object sender, RoutedEventArgs e)
        {
            var contact = (e.OriginalSource as FrameworkElement).DataContext as Tuple<string, string, string, string>;
            PhoneCallManager.ShowPhoneCallUI(contact.Item3, contact.Item2);
        }

        private async void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + Windows.ApplicationModel.Store.CurrentApp.AppId));
        }

        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = "Official graVITas 2015 App";
            request.Data.SetWebLink(Windows.ApplicationModel.Store.CurrentApp.LinkUri);
        }

        private async void FeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            EmailMessage msg = new EmailMessage();
            msg.To.Add(new EmailRecipient("vinaygupta_dev@outlook.com", "Vinay Gupta"));
            msg.Subject = "Feedback - graVITas '15 Windows App";
            await EmailManager.ShowComposeNewEmailAsync(msg);
        }
    }
}
