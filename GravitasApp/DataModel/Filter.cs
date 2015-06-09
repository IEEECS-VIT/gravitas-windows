using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataModel
{
    public interface IFilter<T>
    {
        bool CheckQualification(T content);
        IEnumerable<T> FilterItems(IEnumerable<T> items);
        void GenerateCheckList(IEnumerable<T> items);
        void RunMaintenance(IEnumerable<T> items);
        void ResetAllFlags();
    }

    public class FilterCriterion<TSource, TCriterion> : IFilter<TSource>
        where TCriterion : IEquatable<TCriterion>
    {

        #region Fields and Properties

        private readonly Func<TSource, TCriterion> _filteringPropertySelector;

        private Checklist<TCriterion> _checklist;
        private ReadOnlyCollection<ChecklistItem<TCriterion>> _checklistView;

        public ReadOnlyCollection<ChecklistItem<TCriterion>> Checklist
        {
            get { return _checklistView; }
        }

        #endregion

        #region Constructor

        public FilterCriterion(Func<TSource, TCriterion> filteringPropertySelector)
        {
            _filteringPropertySelector = filteringPropertySelector;
            
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

        public void GenerateCheckList(IEnumerable<TSource> items)
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
                                                    .Distinct<TCriterion>();

            Checklist<TCriterion> checkList = new Checklist<TCriterion>();
            foreach (TCriterion prop in distinctProps)
                checkList.Add(new ChecklistItem<TCriterion>(prop));

            checkList.OrderBy<ChecklistItem<TCriterion>, TCriterion>((checkListItem) => checkListItem.Content);
            return checkList;
        }

        private void PopulateChecklistFresh(IEnumerable<ChecklistItem<TCriterion>> tempChecklist)
        {
            _checklist.Clear();
            foreach (var item in tempChecklist)
                _checklist.Add(item);
        }

        #endregion

    }

}
