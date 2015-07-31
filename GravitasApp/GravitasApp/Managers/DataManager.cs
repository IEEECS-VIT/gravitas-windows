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

        private const string EVENTS_FILE_NAME = "events.json";
        private const string FILTERS_FILE_NAME = "filters.txt";
        private const string SHORTLIST_FILE_NAME = "shortlist.txt";
        private const string DATA_VERSION_KEY = "dataVersion";

        private static List<Event> _eventList;
        private static HashSet<Event> _shortlistedEvents;
        private static FilterCriteria<Event> _filterList;
        private static ReadOnlyCollection<Event> _events;
        private static ReadOnlyCollection<IFilter<Event>> _filters;
        private static bool _isBusy;
        private static bool _contentReady;

        private static List<Event> EventList
        {
            get { return _eventList; }
            set
            {
                _eventList = value;

                if (_eventList != null && _eventList.Count != 0)
                {
                    _contentReady = true;
                    _events = new ReadOnlyCollection<Event>(_eventList);
                }
                else
                    _events = null;
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
        public static bool ContentReady
        {
            get { return _contentReady; }
        }

        public static string EventsDataVersion
        {
            get
            {
                try
                {
                    string ver = (string)App._settings.Values[DATA_VERSION_KEY];
                    if (ver == null)
                        return "none";
                    else
                        return ver;
                }
                catch
                {
                    return "none";
                }
            }
            private set
            {
                App._settings.Values[DATA_VERSION_KEY] = value;
            }
        }
        public static bool IsBusy
        {
            get { return _isBusy; }
        }

        #endregion

        #region Constructor

        static DataManager()
        {
            FilterList = new FilterCriteria<Event>();
            FilterList.Add(new FilterCriterion<Event, string>((e) => e.Category, "CATEGORY"));
            FilterList.Add(new FilterCriterion<Event, string>((e) =>
            {
                if (e.TeamSizes.Count == 0)
                    return "NA";
                else
                {
                    int x;
                    bool res = int.TryParse(e.TeamSizes[0], out x);
                    if (res == true)
                        return x == 1 ? "Individual" : "Group";
                    else
                        return "Variable options";
                }
            }, "TEAM SIZE"));
            FilterList.Add(new FilterCriterion<Event, string>((e) =>
            {
                if (e.AssociatedChapters.Count == 1)
                    return e.AssociatedChapters[0];
                else
                    return "Multiple chapters";
            }, "ORGANISED BY"));

            CategoryMetadata.Initialize();
        }

        #endregion

        #region Private Helpers

        private static async Task<StatusCode> MonitoredTask(Func<Task<StatusCode>> function)
        {
            if (_isBusy == true)
                return StatusCode.Busy;
            _isBusy = true;
            StatusCode code = await function();
            _isBusy = false;
            return code;
        }

        private static async Task<StatusCode> TryLoadEventsFromCache()
        {
            try
            {
                StorageFile file = await App._folder.GetFileAsync(EVENTS_FILE_NAME);
                string content = await FileIO.ReadTextAsync(file);
                EventList = JsonParser.TryGetEvents(content).Item2;
                if (EventList == null)
                    return StatusCode.UnknownError;

                return StatusCode.Success;
            }
            catch
            { return StatusCode.NoData; }
        }

        #endregion

        #region Public Methods (API)

        public static void SetFilterToCategory(string category)
        {
            FilterCriteria.ResetAllFlags();
            foreach (var item in (FilterList[0] as FilterCriterion<Event, string>).Checklist)
            {
                if (string.Equals(item.Content, category, StringComparison.OrdinalIgnoreCase))
                    item.IsChecked = true;
            }
        }

        public static Event TryGetEvent(string eventTitle)
        {
            Event ev = _eventList.Find((e) => e.Title == eventTitle);
            return ev;
        }

        public static bool IsShortlisted(Event e)
        {
            return _shortlistedEvents.Contains(e);
        }

        /// <summary>
        /// Adds or removes an item from the shortlist.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="operation">
        /// True to add, false to remove.
        /// </param>
        /// <returns></returns>
        public static bool UpdateShortlist(Event e, bool operation)
        {
            if (operation == true)
                return _shortlistedEvents.Add(e);
            else
                return _shortlistedEvents.Remove(e);
        }

        public static List<Event> GetShortlist()
        {
            return _shortlistedEvents.ToList();
        }

        public static async Task<StatusCode> TryLoadDataAsync()
        {
            return await MonitoredTask(async () =>
            {
                StatusCode status = await TryLoadEventsFromCache();
                if (status != StatusCode.Success)
                    return StatusCode.NoData;

                try
                {
                    StorageFile file = await App._folder.GetFileAsync(FILTERS_FILE_NAME);
                    bool res = await ContentManager.TryRestoreChecklistsAsync(file, FilterList);
                    if (res == false)
                        FilterList.RunMaintenance(EventList);
                }
                catch
                {
                    FilterList.GenerateChecklist(EventList);
                }

                try
                {
                    StorageFile file = await App._folder.GetFileAsync(SHORTLIST_FILE_NAME);
                    IList<string> eventNames = await FileIO.ReadLinesAsync(file);
                    var selectedEvents = EventList.TakeWhile((e) => eventNames.Contains(e.Title));
                    _shortlistedEvents = new HashSet<Event>();
                    foreach (Event ev in selectedEvents)
                        _shortlistedEvents.Add(ev);
                }
                catch
                {
                    _shortlistedEvents = new HashSet<Event>();
                }

                return StatusCode.Success;
            });
        }

        public static async Task<StatusCode> RefreshEventsAsync()
        {
            return await MonitoredTask(async () =>
            {
                try
                {
                    Response<string> response = await NetworkService.TryGetEventsJsonAsync(EventsDataVersion);
                    if (response.Code == StatusCode.Success)
                    {
                        var details = JsonParser.TryGetEvents(response.Content);

                        // Locally set data variables
                        EventList = details.Item2;
                        FilterList.RunMaintenance(EventList);

                        // Replace existing filters
                        try
                        {
                            StorageFile file = await App._folder.CreateFileAsync(FILTERS_FILE_NAME, CreationCollisionOption.ReplaceExisting);
                            await ContentManager.TrySaveChecklistsAsync(file, FilterList);
                        }
                        catch
                        { }
                        // Cache new data and update version setting
                        try
                        {
                            StorageFile file = await App._folder.CreateFileAsync(EVENTS_FILE_NAME, CreationCollisionOption.ReplaceExisting);
                            await FileIO.WriteTextAsync(file, response.Content);
                            EventsDataVersion = details.Item1;
                        }
                        catch { }

                        // Update shortlisted events
                        HashSet<Event> newShortlist = new HashSet<Event>();
                        foreach (Event e in _shortlistedEvents)
                            if (EventList.Contains(e))
                                newShortlist.Add(e);
                        _shortlistedEvents = newShortlist;

                        return StatusCode.Success;
                    }
                    else
                        return response.Code;
                }
                catch
                {
                    EventList = new List<Event>();
                    FilterList.GenerateChecklist(EventList);
                    _shortlistedEvents = new HashSet<Event>();
                    return StatusCode.UnknownError;
                }
            });
        }

        public static async Task<StatusCode> TrySaveDataAsync()
        {
            return await MonitoredTask(async () =>
                {
                    if (ContentReady == false)
                        return StatusCode.NoData;

                    try
                    {
                        StorageFile file = await App._folder.CreateFileAsync(FILTERS_FILE_NAME, CreationCollisionOption.ReplaceExisting);
                        await ContentManager.TrySaveChecklistsAsync(file, FilterList);
                        StorageFile file2 = await App._folder.CreateFileAsync(SHORTLIST_FILE_NAME, CreationCollisionOption.ReplaceExisting);
                        await FileIO.WriteLinesAsync(file2, _shortlistedEvents.Select((e) => e.Title).ToList());
                        return StatusCode.Success;
                    }
                    catch
                    {
                        return StatusCode.UnknownError;
                    }
                });
        }

        #endregion
    }
}
