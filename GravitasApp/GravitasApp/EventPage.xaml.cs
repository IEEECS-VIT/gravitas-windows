using GravitasApp.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace GravitasApp
{

    public sealed partial class EventPage : Page, IManageable, INotifyPropertyChanged
    {

        public class ExpandableList<T> : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private bool _isExpanded;
            private string _listHeader;
            private List<T> _items;

            public bool IsExpanded
            {
                get { return _isExpanded; }
                set
                {
                    _isExpanded = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("IsExpanded"));
                }
            }
            public List<T> Items
            {
                get { return _items; }
                set
                {
                    _items = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Items"));
                }
            }
            public string ListHeader
            {
                get { return _listHeader; }
                set
                {
                    _listHeader = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("ListHeader"));
                }
            }

            public ExpandableList(string listHeader)
            {
                _items = new List<T>();
                _listHeader = listHeader;
            }
        }

        public ExpandableList<Visibility> Views { get; set; }

        public EventPage()
        {
            this.InitializeComponent();

            Views = new ExpandableList<Visibility>("title");
            Views.Items.Add(Visibility.Collapsed);
            Views.Items.Add(Visibility.Collapsed);

            this.DataContext = this;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            PageManager.RegisterPage(this);
            await StatusBar.GetForCurrentView().HideAsync();
        }

        public Dictionary<string, object> SaveState()
        {
            return null;
        }

        public void LoadState(Dictionary<string, object> lastState)
        {
        }

        public bool AllowAppExit()
        {
            return true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((sender as FrameworkElement).DataContext as ExpandableList<Visibility>).IsExpanded = !(((sender as FrameworkElement).DataContext as ExpandableList<Visibility>).IsExpanded);
        }

        private void Button_Click(object sender, TappedRoutedEventArgs e)
        {
            ((sender as FrameworkElement).DataContext as ExpandableList<Visibility>).IsExpanded = !(((sender as FrameworkElement).DataContext as ExpandableList<Visibility>).IsExpanded);
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            teamSizeFlyout.Placement = FlyoutPlacementMode.Bottom;
            teamSizeFlyout.ShowAt(headerGrid);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void WrapGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            (sender as WrapGrid).ItemHeight = (sender as WrapGrid).ItemWidth = e.NewSize.Width / 3;
        }
    }
}
