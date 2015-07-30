using GravitasSDK.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Data.Json;


namespace GravitasSDK.Providers
{
    public static class JsonParser
    {
        internal static StatusCode GetStatus(string content)
        {
            JsonObject obj = JsonObject.Parse(content);
            int code = (int)obj.GetNamedNumber("status");
            switch (code)
            {
                case 1: return StatusCode.Success;
                case 2: return StatusCode.NoChange;
                default: return StatusCode.ServerError;
            }
        }

        private void AssignStringList(JsonObject obj, string arrayKey, List<string> targetList)
        {
            if (obj.GetNamedValue(arrayKey).ValueType == JsonValueType.Null)
                return;

            foreach (JsonValue val in obj.GetNamedArray(arrayKey))
                targetList.Add(val.GetString());
        }

        public List<Event> GetEvents(string eventsJson)
        {
            List<Event> events = new List<Event>();
            JsonArray eventsArray = JsonObject.Parse(eventsJson).GetNamedArray("details");

            foreach (JsonValue item in eventsArray)
            {
                Event e = new Event();
                JsonObject eventObject = item.GetObject();

                e.Title = eventObject.GetNamedString("title");
                e.Category = eventObject.GetNamedString("category");
                e.Description = eventObject.GetNamedString("description");

                AssignStringList(eventObject, "chapters", e._AssociatedChapters);
                AssignStringList(eventObject, "emails", e._Emails);
                AssignStringList(eventObject, "fees", e._FeesInfo);
                AssignStringList(eventObject, "team_sizes", e._TeamSizes);

                if (eventObject.GetNamedValue("coordinators").ValueType != null)
                {
                    foreach (JsonValue val in eventObject.GetNamedArray("coordinators"))
                    {
                        JsonObject cObj = val.GetObject();
                        e._Coordinators.Add(new Coordinator(cObj.GetNamedString("name"), cObj.GetNamedString("phone")));
                    }
                }

                if (eventObject.GetNamedValue("prizes").ValueType != null)
                {
                    foreach (JsonValue val in eventObject.GetNamedArray("prizes"))
                    {
                        JsonObject cObj = val.GetObject();
                        e._PrizesInfo.Add(new Tuple<string, ulong>(cObj.GetNamedString("tag"), (ulong)(cObj.GetNamedNumber("prize"))));
                    }
                }

                if (eventObject.GetNamedValue("venue").ValueType != JsonValueType.Null)
                    e.Venue = eventObject.GetNamedString("venue");

                // Timings/Dates are pending

                events.Add(e);
            }

            return events;
        }

    }
}
