using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;


namespace GravitasApp.Helpers
{
    // Instance Members
    public partial class CategoryMetadata
    {
        private readonly string _name;
        private readonly string _info;

        #region Public Properties

        public string Name
        {
            get { return _name; }
        }
        public string Info
        {
            get { return _info; }
        }
        public Uri ImageUri
        {
            get { return new Uri(String.Format("ms-appx:///Assets/Gravitas/{0}.png", Name.Replace(" ", ""))); }
        }
        public SolidColorBrush LabelBrush
        {
            get { return _categoryBrushes[Name]; }
        }

        #endregion

        private CategoryMetadata(string name, string info, Color labelColor)
        {
            _name = name;
            _info = info;

            _categoryBrushes.Add(name, new SolidColorBrush(labelColor));
        }
    }

    // Static Members
    public partial class CategoryMetadata
    {
        private static readonly Dictionary<string, SolidColorBrush> _categoryBrushes;
        private static readonly CategoryMetadataCollection _infoList;
        private static readonly ReadOnlyCollection<CategoryMetadata> _readonlyInfoList;

        static CategoryMetadata()
        {
            _categoryBrushes = new Dictionary<string, SolidColorBrush>();
            _infoList = new CategoryMetadataCollection();
            _readonlyInfoList = new ReadOnlyCollection<CategoryMetadata>(_infoList);
        }

        public static ReadOnlyCollection<CategoryMetadata> InfoList
        {
            get { return _readonlyInfoList; }
        }

        // Ponder: Move data to a Json file (?)
        public static void Initialize()
        {
            _infoList.Add(new CategoryMetadata(
                "BITS AND BYTES",
                "Geek is the new cool! Compete across various domains covering algorithms, networking and more.",
                Colors.SteelBlue));
            _infoList.Add(new CategoryMetadata(
                "BIOXYN",
                "Its the survival of the fittest, time to put those little neurons to use!",
                Colors.Green));
            _infoList.Add(new CategoryMetadata(
                "ROBOMANIA",
                "",
                Colors.Gray));
        }

        public static CategoryMetadata GetMetadata(string categoryName)
        {
            if (_infoList.Contains(categoryName))
                return _infoList[categoryName];
            else
                return null;
        }
    }

    public class CategoryMetadataCollection : KeyedCollection<string, CategoryMetadata>
    {
        protected override string GetKeyForItem(CategoryMetadata item)
        {
            return item.Name;
        }
    }

}
