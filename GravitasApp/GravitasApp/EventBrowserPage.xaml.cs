using GravitasApp.Managers;
using GravitasSDK.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public sealed partial class EventBrowserPage : Page, IManageable, INotifyPropertyChanged
    {
        
        private readonly int _filterCount;
        private IEnumerable<Event> _filteredEvents;
        private IEnumerable<Event> _selectedEvents;
        private string _searchBoxText;

        #region Properties

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
        public IEnumerable<Event> SelectedEvents
        {
            get { return _selectedEvents; }
            private set
            {
                _selectedEvents = value;
                NotifyPropertyChanged();
            }
        }
        public string SearchBoxText
        {
            get { return _searchBoxText; }
            set
            {
                _searchBoxText = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        public EventBrowserPage()
        {
            this.InitializeComponent();

            _filterCount = DataManager.FilterCriteria.GetCheckCount();
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PageManager.RegisterPage(this);
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IManageable Implementation

        public Dictionary<string, object> SaveState()
        {
            Dictionary<string, object> stateDic = new Dictionary<string, object>();
            stateDic.Add("searchString", SearchBoxText);
            return stateDic;
        }

        public void LoadState(Dictionary<string, object> lastState)
        {
            if (_filterCount == 0)
                _filteredEvents = DataManager.Events;
            else
                _filteredEvents = DataManager.FilterCriteria.FilterItems(DataManager.Events);
            SelectedEvents = _filteredEvents;

            try
            {
                if (lastState != null)
                    SearchBoxText = (string)lastState["searchString"];
            }
            catch { }
        }

        public bool AllowAppExit()
        {
            return true;
        }

        #endregion

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            PageManager.NavigateTo(typeof(FilterPage), null, NavigationType.Default);
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBoxText = string.Empty;
            SelectedEvents = _filteredEvents;
        }

        private void SearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                this.Focus(FocusState.Programmatic);
                SearchBoxText = (sender as TextBox).Text;
                SearchEvents();
            }
        }

        private void SearchEvents()
        {
            if (SearchBoxText != string.Empty)
            {
                string searchString = SearchBoxText.ToLower();
                IEnumerable<Event> events = _filteredEvents.Where((Event ev, int index) =>
                    {
                        string title = ev.Title.ToLower();
                        if (title.Contains(searchString))
                            return true;
                        else
                            return false;
                    });
                SelectedEvents = events.ToList<Event>();
            }
            else
                SelectedEvents = _filteredEvents;
        }

    }

}
