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
        private static List<Event> _events;
        private static ReadOnlyCollection<Event> _eventList;

        private static List<Event> Events
        {
            get { return _events; }
            set
            {
                _events = value;
                _eventList = new ReadOnlyCollection<Event>(_events);
            }
        }
        public static ReadOnlyCollection<Event> EventList
        {
            get { return _eventList; }
        }

        public static async Task LoadEventsAsync()
        {
            // Temporary Set-up
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Events.xml"));
            Events = (await ContentSerializer.ParseEventsAsync(file)).ToList();
        }
    }
}
