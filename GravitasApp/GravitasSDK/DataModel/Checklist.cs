using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace GravitasSDK.DataModel
{

    [DataContract]
    public class ChecklistItem<T> : IEquatable<ChecklistItem<T>>
        where T : IEquatable<T>
    {
        [DataMember]
        private readonly T _content;
        [DataMember]
        private bool _isChecked;

        public T Content
        { get { return _content; } }
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }

        public ChecklistItem(T content)
        {
            _content = content;
        }

        public bool Equals(ChecklistItem<T> other)
        {
            return this.Content.Equals(other.Content);
        }

        public override bool Equals(object obj)
        {
            ChecklistItem<T> castedObj = obj as ChecklistItem<T>;
            return this.Content.Equals(castedObj.Content);
        }
        public override int GetHashCode()
        {
            return this.Content.GetHashCode();
        }
        public override string ToString()
        {
            return String.Format("{{{0} - {1}}}", Content, IsChecked);
        }
    }

    [CollectionDataContract]
    public class Checklist<T> : KeyedCollection<T, ChecklistItem<T>>
        where T : IEquatable<T>
    {
        protected override T GetKeyForItem(ChecklistItem<T> item)
        {
            return item.Content;
        }

        public Checklist()
            : base()
        { }
    }

}
