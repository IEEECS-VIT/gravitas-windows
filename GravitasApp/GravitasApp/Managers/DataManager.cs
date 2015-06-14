using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using GravitasSDK.Providers;
using GravitasSDK.DataModel;
using System.Collections.ObjectModel;


namespace GravitasApp.Managers
{
    public static class DataManager
    {
        #region Fields and Properties

        private static List<Event> _events;
        private static FilterCriteria<Event> _filterList;

        private static ReadOnlyCollection<Event> _eventList;
        private static ReadOnlyCollection<IFilter<Event>> _filters;

        private static List<Event> Events
        {
            get { return _events; }
            set
            {
                _events = value;
                _eventList = new ReadOnlyCollection<Event>(_events);
            }
        }
        private static FilterCriteria<Event> FilterList
        {
            get { return _filterList; }
            set
            {
                _filterList = value;
                _filters = new ReadOnlyCollection<IFilter<Event>>(_filterList);
            }
        }

        public static ReadOnlyCollection<Event> EventList
        {
            get { return _eventList; }
        }
        public static ReadOnlyCollection<IFilter<Event>> Filters
        {
            get { return _filters; }
        }

        #endregion

        public static async Task LoadEventsAsync()
        {
            // Temporary Set-up
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Events.xml"));
            Events = (await ContentSerializer.ParseEventsAsync(file)).ToList();
            _filterList.GenerateChecklist(_events);
        }

        #region Constructor

        static DataManager()
        {
            FilterList = new FilterCriteria<Event>();
            FilterList.Add(new FilterCriterion<Event, string>((e) => e.Category, "CATEGORY"));
            FilterList.Add(new FilterCriterion<Event, string>((e) => e.StartTime.ToString("dd MMM, HH:mm"), "START TIME"));
        }

        #endregion
    }
}
