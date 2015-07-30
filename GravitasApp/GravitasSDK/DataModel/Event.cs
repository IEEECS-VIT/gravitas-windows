using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace GravitasSDK.DataModel
{
    public class Event
    {

        #region Private and Internal Properties

        internal List<Tuple<string, ulong>> _PrizesInfo { get; private set; }
        internal List<string> _FeesInfo { get; private set; }
        internal List<string> _TeamSizes { get; private set; }

        internal List<string> _AssociatedChapters { get; private set; }
        internal List<Coordinator> _Coordinators { get; private set; }
        internal List<string> _Emails { get; private set; }

        #endregion

        #region Public Properties

        public string Title { get; internal set; }
        public string Category { get; internal set; }
        public string Description { get; internal set; }
        public string Venue { get; internal set; }

        public ReadOnlyCollection<string> TeamSizes { get; private set; }
        public ReadOnlyCollection<Tuple<string, ulong>> PrizesInfo { get; private set; }
        public ReadOnlyCollection<string> FeesInfo { get; private set; }

        public ReadOnlyCollection<string> AssociatedChapters { get; private set; }
        public ReadOnlyCollection<Coordinator> Coordinators { get; private set; }
        public ReadOnlyCollection<string> Emails { get; private set; }

        #endregion

        #region Constructor

        public Event()
        {
            _TeamSizes = new List<string>();
            _PrizesInfo = new List<Tuple<string, ulong>>();
            _FeesInfo = new List<string>();

            _AssociatedChapters = new List<string>();
            _Coordinators = new List<Coordinator>();
            _Emails = new List<string>();

            TeamSizes = new ReadOnlyCollection<string>(_TeamSizes);
            PrizesInfo = new ReadOnlyCollection<Tuple<string, ulong>>(_PrizesInfo);
            FeesInfo = new ReadOnlyCollection<string>(_FeesInfo);

            AssociatedChapters = new ReadOnlyCollection<string>(_AssociatedChapters);
            Coordinators = new ReadOnlyCollection<Coordinator>(_Coordinators);
            Emails = new ReadOnlyCollection<string>(_Emails);
        }

        #endregion
    }

    public class Coordinator
    {
        private readonly string _name;
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
