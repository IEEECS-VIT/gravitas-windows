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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EventBrowserPage : Page, IManageable
    {
        private uint _filterCount;

        public string FilterCountString
        {
            get
            {
                if (_filterCount == 0)
                    return "no filters applied";
                else
                    return string.Format("{0} filter{1} applied", _filterCount, _filterCount == 1 ? "" : "s");
            }
        }
        public IEnumerable<Event> Events
        {
            get;
            private set;
        }

        public EventBrowserPage()
        {
            this.InitializeComponent();

            foreach (dynamic item in DataManager.Filters)
                _filterCount += item.GetCheckCount() != 0 ? 1u : 0u;
            Events = DataManager.EventList;

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

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            PageManager.NavigateTo(typeof(FilterPage), null, NavigationType.Default);
        }
    }
}
