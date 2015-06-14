using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GravitasSDK.DataModel
{
    public interface IFilter<T>
    {
        bool CheckQualification(T content);
        IEnumerable<T> FilterItems(IEnumerable<T> items);
        void GenerateChecklist(IEnumerable<T> items);
        void RunMaintenance(IEnumerable<T> items);
        void ResetAllFlags();
    }

    public class FilterCriterion<TSource, TCriterion> : IFilter<TSource>
        where TCriterion : IEquatable<TCriterion>
    {

        #region Fields and Properties

        private readonly Func<TSource, TCriterion> _filteringPropertySelector;
        private readonly string _label;

        private Checklist<TCriterion> _checklist;
        private ReadOnlyCollection<ChecklistItem<TCriterion>> _checklistView;

        public ReadOnlyCollection<ChecklistItem<TCriterion>> Checklist
        {
            get { return _checklistView; }
        }
        public string Label
        {
            get { return _label; }
        }

        internal Checklist<TCriterion> InternalChecklist
        {
            get { return _checklist; }
            set
            {
                _checklist = value;
                _checklistView = new ReadOnlyCollection<ChecklistItem<TCriterion>>(_checklist);
            }
        }

        #endregion

        #region Constructor

        public FilterCriterion(Func<TSource, TCriterion> filteringPropertySelector, string label = null)
        {
            _filteringPropertySelector = filteringPropertySelector;
            _label = label;

            _checklist = new Checklist<TCriterion>();
            _checklistView = new ReadOnlyCollection<ChecklistItem<TCriterion>>(_checklist);
        }

        #endregion

        #region IFilter<> Interface Implementation

        public bool CheckQualification(TSource item)
        {
            TCriterion keyValue = _filteringPropertySelector(item);
            return _checklist[keyValue].IsChecked;
        }

        public IEnumerable<TSource> FilterItems(IEnumerable<TSource> items)
        {
            List<TSource> selectedItems = new List<TSource>();
            foreach (TSource item in items)
                if (CheckQualification(item))
                    selectedItems.Add(item);
            return selectedItems;
        }

        public void GenerateChecklist(IEnumerable<TSource> items)
        {
            PopulateChecklistFresh(GetChecklist(items));
        }

        public void RunMaintenance(IEnumerable<TSource> items)
        {
            var newChecklist = GetChecklist(items);

            foreach (var item in Checklist)
                if (newChecklist.Contains(item.Content))
                    newChecklist[item.Content].IsChecked = item.IsChecked;

            PopulateChecklistFresh(newChecklist);
        }

        public void ResetAllFlags()
        {
            foreach (var item in _checklist)
                item.IsChecked = false;
        }

        #endregion

        #region Private Helper Methods

        private Checklist<TCriterion> GetChecklist(IEnumerable<TSource> items)
        {
            IEnumerable<TCriterion> distinctProps = items
                                                    .Select<TSource, TCriterion>(_filteringPropertySelector)
                                                    .Distinct<TCriterion>()
                                                    .OrderBy<TCriterion, TCriterion>((item) => item);

            Checklist<TCriterion> checklist = new Checklist<TCriterion>();
            foreach (TCriterion prop in distinctProps)
                checklist.Add(new ChecklistItem<TCriterion>(prop));

            return checklist;
        }

        private void PopulateChecklistFresh(IEnumerable<ChecklistItem<TCriterion>> tempChecklist)
        {
            _checklist.Clear();
            foreach (var item in tempChecklist)
                _checklist.Add(item);
        }

        #endregion
    
    }

    public class FilterCriteria<T> : Collection<IFilter<T>>, IFilter<T>
    {
        #region Constructors

        public FilterCriteria(IList<IFilter<T>> filterList)
            : base(filterList)
        { }

        public FilterCriteria()
            : base()
        { }

        #endregion

        #region IFilter<> Interface Implementation

        public bool CheckQualification(T content)
        {
            foreach (IFilter<T> criterion in this)
                if (criterion.CheckQualification(content) == false)
                    return false;
                else
                    continue;
            return true;
        }

        public IEnumerable<T> FilterItems(IEnumerable<T> items)
        {
            List<T> selectedItems = new List<T>();
            foreach (T item in items)
                if (CheckQualification(item))
                    selectedItems.Add(item);
            return selectedItems;
        }

        public void GenerateChecklist(IEnumerable<T> items)
        {
            foreach (var criterion in this)
                criterion.GenerateChecklist(items);
        }

        public void RunMaintenance(IEnumerable<T> items)
        {
            foreach (var criterion in this)
                criterion.RunMaintenance(items);
        }

        public void ResetAllFlags()
        {
            foreach (var criterion in this)
                criterion.ResetAllFlags();
        }

        #endregion
    }

}
