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
using GravitasApp.Managers;
using Windows.UI.ViewManagement;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using GravitasApp.Helpers;


namespace GravitasApp
{

    public sealed partial class MainPage : Page, IManageable
    {

        public IEnumerable<CategoryMetadata> CategoryInfoList
        { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();
            CategoryInfoList = CategoryMetadata.InfoList.Take(12);

            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
            this.DataContext = this;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            PageManager.RegisterPage(this);
            await StatusBar.GetForCurrentView().HideAsync();
        }

        #region IManageable Implementation

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

        #endregion

        private void ViewWorkshopsButton_Click(object sender, RoutedEventArgs e)
        {
            DataManager.SetFilterToCategory("Workshops");
            PageManager.NavigateTo(typeof(EventBrowserPage), null, NavigationType.Default);
        }

        private void CategoryView_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataManager.SetFilterToCategory((e.ClickedItem as CategoryMetadata).Name);
            PageManager.NavigateTo(typeof(EventBrowserPage), null, NavigationType.Default);
        }

        private void ShortlistButton_Click(object sender, RoutedEventArgs e)
        {
            PageManager.NavigateTo(typeof(ShortlistPage), null, NavigationType.Default);
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            PageManager.NavigateTo(typeof(AboutPage), null, NavigationType.Default);
        }

        private void AllEventsButton_Click(object sender, RoutedEventArgs e)
        {
            PageManager.NavigateTo(typeof(EventBrowserPage), null, NavigationType.Default);
        }

    }
}
