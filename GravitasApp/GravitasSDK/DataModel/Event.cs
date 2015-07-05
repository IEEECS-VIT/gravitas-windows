using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace GravitasSDK.DataModel
{
    [DataContract]
    public class Event
    {

        #region Fields and Properties

        [DataMember]
        private List<uint> _prizes;
        [DataMember]
        private List<Coordinator> _coordinators;
        [DataMember]
        private List<string> _associatedChapters;

        [DataMember]
        public string Title { get; private set; }
        [DataMember]
        public string Category { get; private set; }
        [DataMember]
        public string Description { get; private set; }
        [DataMember]
        public uint RegistrationFees { get; private set; }
        [DataMember]
        public string Venue { get; private set; }
        public ReadOnlyCollection<uint> Prizes { get; private set; }
        [DataMember]
        public DateTimeOffset StartTime { get; private set; }
        [DataMember]
        public DateTimeOffset EndTime { get; private set; }
        public ReadOnlyCollection<Coordinator> Coordinators { get; private set; }
        public ReadOnlyCollection<string> AssociatedChapters { get; private set; }

        #endregion

        #region Constructor

        public Event()
        {
            _prizes = new List<uint>();
            _coordinators = new List<Coordinator>();
            SetUpViews(new StreamingContext());
        }

        #endregion

        #region Serialization Hooks

        [OnDeserialized]
        private void SetUpViews(StreamingContext sc)
        {
            Prizes = new ReadOnlyCollection<uint>(_prizes);
            Coordinators = new ReadOnlyCollection<Coordinator>(_coordinators);
            AssociatedChapters = new ReadOnlyCollection<string>(_associatedChapters);
        }

        #endregion
    }

    [DataContract]
    public class Coordinator
    {
        [DataMember]
        private readonly string _name;
        [DataMember]
        private readonly string _phone;

        public string Name
        { get { return _name; } }
        public string Phone
        { get { return _phone; } }

        public Coordinator(string name, string phone)
        {
            _name = name;
            _phone = phone;
        }
    }
}
