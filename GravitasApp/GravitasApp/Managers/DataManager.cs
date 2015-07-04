using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using GravitasSDK.Providers;
using GravitasSDK.DataModel;
using System.Collections.ObjectModel;
using GravitasApp.Helpers;


namespace GravitasApp.Managers
{
    public static class DataManager
    {
        #region Fields and Properties

        private static List<Event> _eventList;
        private static FilterCriteria<Event> _filterList;
        private static ReadOnlyCollection<Event> _events;
        private static ReadOnlyCollection<IFilter<Event>> _filters;

        private static List<Event> EventList
        {
            get { return _eventList; }
            set
            {
                _eventList = value;
                _events = new ReadOnlyCollection<Event>(_eventList);
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

        public static ReadOnlyCollection<Event> Events
        {
            get { return _events; }
        }
        public static ReadOnlyCollection<IFilter<Event>> Filters
        {
            get { return _filters; }
        }
        public static IFilter<Event> FilterCriteria
        {
            get { return _filterList as IFilter<Event>; }
        }

        #endregion

        // Temporary Set-up
        public static async Task LoadEventsAsync()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Events.xml"));
            EventList = (await ContentSerializer.ParseEventsAsync(file)).ToList();
            _filterList.GenerateChecklist(_eventList);
        }

        #region Constructor

        // Temporary Set-up
        static DataManager()
        {
            FilterList = new FilterCriteria<Event>();
            FilterList.Add(new FilterCriterion<Event, string>((e) => e.Category, "CATEGORY"));
            FilterList.Add(new FilterCriterion<Event, string>((e) => e.Venue, "VENUE"));

            CategoryMetadata.Initialize();
        }

        #endregion

        public static void SetFilterToCategory(string category)
        {
            FilterCriteria.ResetAllFlags();
            foreach (var item in (FilterList[0] as FilterCriterion<Event, string>).Checklist)
            {
                if (string.Equals(item.Content, category, StringComparison.OrdinalIgnoreCase))
                    item.IsChecked = true;
            }
        }

    }
}
