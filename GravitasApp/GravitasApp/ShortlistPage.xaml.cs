using GravitasApp.Helpers;
using GravitasApp.Managers;
using GravitasSDK.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace GravitasApp
{

    public sealed partial class ShortlistPage : Page, IManageable
    {
        public List<Tuple<Event, CategoryMetadata>> ShortlistedEventsInfo { get; private set; }
        public Visibility ListHeaderVisiblity { get; private set; }

        public ShortlistPage()
        {
            this.InitializeComponent();
            List<Event> shortlist = DataManager.GetShortlist();
            ShortlistedEventsInfo = shortlist.Select<Event, Tuple<Event,CategoryMetadata>>(
                (Event e) => new Tuple<Event, CategoryMetadata>(e, CategoryMetadata.GetMetadata(e.Category))).ToList();
            if (ShortlistedEventsInfo.Count != 0)
                ListHeaderVisiblity = Windows.UI.Xaml.Visibility.Visible;
            else
                ListHeaderVisiblity = Windows.UI.Xaml.Visibility.Collapsed;
            this.DataContext = this;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PageManager.RegisterPage(this);
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

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            PageManager.NavigateTo(typeof(EventPage), (e.ClickedItem as Tuple<Event, CategoryMetadata>).Item1.Title, NavigationType.Default);
        }

        private void HomePageButton_Click(object sender, RoutedEventArgs e)
        {
            PageManager.NavigateTo(typeof(MainPage), null, NavigationType.FreshStart);
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            PageManager.NavigateTo(typeof(AboutPage), null, NavigationType.Default);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Event ev in DataManager.GetShortlist())
            {
                DataManager.UpdateShortlist(ev, false);
            }
            ListHeaderVisiblity = Windows.UI.Xaml.Visibility.Collapsed;
            ShortlistedEventsInfo = new List<Tuple<Event, CategoryMetadata>>();
            this.DataContext = null;
            this.DataContext = this;
        }
    }
}
