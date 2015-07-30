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

        public List<Event> GetEvents(string eventsJson)
        {
            throw new NotImplementedException();
        }

    }
}
