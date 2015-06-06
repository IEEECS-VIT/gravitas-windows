using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataModel
{
    public class Event
    {
            public string EventName { get; set; }
            public string EventCategory { get; set; }
            public string Coordinator1Name { get; set; }
            public string Coordinator2Name { get; set; }
            public string Coordinator1Phone { get; set; }
            public string Coordinator2Phone { get; set; }
            public string Description { get; set; }
            public int RegistrationFees { get; set; }
            public string EventVenue { get; set; }
            public int[] Prize { get; set; }
            public DateTimeOffset EventStartTime { get; set; }
            public DateTimeOffset EventEndTime { get; set; }
            public string MiscDetails { get; set; }
    }
}
