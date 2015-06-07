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

        private readonly Func<TSource, TCriterion> _filteringPropertySelector;

        private Checklist<TCriterion> _checklist;
        private ReadOnlyCollection<ChecklistItem<TCriterion>> _checklistView;

        public ReadOnlyCollection<ChecklistItem<TCriterion>> Checklist
        {
            get { return _checklistView; }
        }

        public FilterCriterion(Func<TSource, TCriterion> filteringPropertySelector)
        {
            _checklist = new Checklist<TCriterion>();
            _checklistView = new ReadOnlyCollection<ChecklistItem<TCriterion>>(_checklist);

            _filteringPropertySelector = filteringPropertySelector;
        }

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
            var uniquePropertiesList = GetDistinctValues(items);

            _checklist.Clear();
            foreach (TCriterion value in uniquePropertiesList)
                _checklist.Add(new ChecklistItem<TCriterion>(value));

            _checklist.OrderBy<ChecklistItem<TCriterion>, TCriterion>((checkListItem) => checkListItem.Content);
        }

        public void RunMaintenance(IEnumerable<TSource> items)
        {
            throw new NotImplementedException();
        }

        public void ResetAllFlags()
        {
            foreach (var item in _checklist)
                item.IsChecked = false;
        }

        #endregion

        #region Private Helper Methods

        private IEnumerable<TCriterion> GetDistinctValues(IEnumerable<TSource> items)
        {
            return items
                    .Select<TSource, TCriterion>(_filteringPropertySelector)
                    .Distinct<TCriterion>();
        }

        #endregion

    }

}
