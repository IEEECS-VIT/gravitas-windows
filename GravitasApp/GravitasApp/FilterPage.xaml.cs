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
using GravitasSDK.DataModel;
using GravitasApp.Managers;
using System.Diagnostics;


namespace GravitasApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FilterPage : Page, IManageable
    {
        public IEnumerable<IFilter<Event>> Filters
        {
            get;
            private set;
        }

        public FilterPage()
        {
            this.InitializeComponent();
            Filters = DataManager.Filters;
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

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            PageManager.NavigateBack();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (IFilter<Event> filter in DataManager.Filters)
                filter.ResetAllFlags();
            PageManager.NavigateBack();
        }

    }
}
