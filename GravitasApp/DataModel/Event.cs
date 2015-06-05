using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataModel
{
    public class Event
    {
            public string name { get; set; }
            public string category { get; set; }
            public string coordinator1Name { get; set; }
            public string coordinator2Name { get; set; }
            public long coordinator1Phone { get; set; }
            public long coordinator2Phone { get; set; }
            public string description { get; set; }
            public int fees { get; set; }
            public string venue { get; set; }
            public int prize1 { get; set; }
            public int prize2 { get; set; }
            public int prize3 { get; set; }
            public string duration { get; set; }
            public string miscDetails { get; set; }
    }
}
