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

        private static void AssignStringList(JsonObject obj, string arrayKey, List<string> targetList)
        {
            if (obj.GetNamedValue(arrayKey).ValueType == JsonValueType.Null)
                return;

            foreach (JsonValue val in obj.GetNamedArray(arrayKey))
                targetList.Add(val.GetString());
        }

        public static Tuple<string, List<Event>> TryGetEvents(string eventsJson)
        {
            try
            {
                List<Event> events = new List<Event>();
                JsonObject detailsObject = JsonObject.Parse(eventsJson).GetNamedObject("events");
                JsonArray eventsArray = detailsObject.GetNamedArray("details");

                foreach (JsonValue item in eventsArray)
                {
                    Event e = new Event();
                    JsonObject eventObject = item.GetObject();

                    e.Title = eventObject.GetNamedString("title");
                    e.Category = eventObject.GetNamedString("category");
                    if (eventObject.GetNamedValue("description").ValueType != JsonValueType.Null)
                        e.Description = eventObject.GetNamedString("description");
                    else
                        e.Description = null;

                    AssignStringList(eventObject, "chapters", e._AssociatedChapters);
                    AssignStringList(eventObject, "emails", e._Emails);
                    AssignStringList(eventObject, "fees", e._FeesInfo);
                    AssignStringList(eventObject, "team_sizes", e._TeamSizes);
                    AssignStringList(eventObject, "timings", e._Timings);

                    if (eventObject.GetNamedValue("coordinators").ValueType != JsonValueType.Null)
                    {
                        foreach (JsonValue val in eventObject.GetNamedArray("coordinators"))
                        {
                            JsonObject cObj = val.GetObject();
                            e._Coordinators.Add(new Coordinator(cObj.GetNamedString("name"), cObj.GetNamedString("phone")));
                        }
                    }

                    if (eventObject.GetNamedValue("prizes").ValueType != JsonValueType.Null)
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

                string version = detailsObject.GetNamedString("data_version");
                return new Tuple<string, List<Event>>(version, events);
            }
            catch
            { return null; }
        }

    }
}
