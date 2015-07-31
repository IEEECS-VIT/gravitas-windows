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
        private readonly string _linkFragment;

        #region Public Properties

        public string Name
        {
            get { return _name; }
        }
        public string Info
        {
            get { return _info; }
        }
        public string WebLink
        {
            get { return "http://info.vit.ac.in/gravitas2015/events/list/list.html#" + _linkFragment; }
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

        private CategoryMetadata(string name, string info, Color labelColor, string linkFragment)
        {
            _name = name;
            _info = info;
            _linkFragment = linkFragment;
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

        public static void Initialize()
        {
            _infoList.Add(new CategoryMetadata(
                "PREMIUM",
                "Go big. Participate in these massively popular events and take a shot at beating the crowd!",
                Colors.DarkSalmon, "premium"));
            _infoList.Add(new CategoryMetadata(
                "ROBOMANIA",
                "Robots are always fascinating! Make them run, fly, fight, anything and have the world watch in awe.",
                Colors.LightGray, "robotics"));
            _infoList.Add(new CategoryMetadata(
                "BITS AND BYTES",
                "Geek is the new cool! Compete across various domains covering algorithms, networking and more.",
                Colors.LightBlue, "computer"));
            _infoList.Add(new CategoryMetadata(
                "APPLIED ENGINEERING",
                "Apply everything you've learnt in engineering (no, we're serious) and compete to the teeth.",
                Colors.Goldenrod, "mech"));
            _infoList.Add(new CategoryMetadata(
                "MANAGEMENT",
                "Management isn't for the faint hearted. Showcase your leadership and managing skills in these set of events.",
                Colors.Bisque, "manage"));
            _infoList.Add(new CategoryMetadata(
                "INFORMALS",
                "Put down your books, have some fun and forget engineering!",
                Colors.MediumPurple, "informal"));
            _infoList.Add(new CategoryMetadata(
                "BUILTRIX",
                "Build, plan or construct - this is your platform to show your creativity and skill.",
                Colors.CornflowerBlue, "civil"));
            _infoList.Add(new CategoryMetadata(
                "CIRCUITRIX",
                "Let go of your resistance and charge up with these electrifying events!",
                Colors.MediumSeaGreen, "electric"));
            _infoList.Add(new CategoryMetadata(
                "QUIZ",
                "Set your hands on the buzzer, its quiz time!",
                new Color() { R = 200, G = 50, B = 50, A = 255 }, "quiz"));
            _infoList.Add(new CategoryMetadata(
                "ONLINE",
                "With internet being the world's obsession, take part now in a variety of online events.",
                Colors.Tomato, "online"));
            _infoList.Add(new CategoryMetadata(
                "BIOXYN",
                "Its the survival of the fittest, time to put those little neurons to use!",
                Colors.Green, "bio"));
            _infoList.Add(new CategoryMetadata(
                "SCHOOL",
                "Some mixed bag and classic events to bring back good old days.",
                Colors.LightSlateGray, "school"));
            _infoList.Add(new CategoryMetadata(
                "WORKSHOPS",
                "",
                Colors.DarkTurquoise, "workshops"));
        }

        public static CategoryMetadata GetMetadata(string categoryName)
        {
            if (_infoList.Contains(categoryName.ToUpper()))
                return _infoList[categoryName.ToUpper()];
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
