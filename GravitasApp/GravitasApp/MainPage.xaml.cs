using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GravitasApp.Managers;
using Windows.UI.ViewManagement;
using System.ComponentModel;


namespace GravitasApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IManageable, INotifyPropertyChanged
    {

        private string _sampleText;
        DispatcherTimer timer;

        public string SampleText
        {
            get { return _sampleText; }
            set
            {
                _sampleText = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SampleText"));
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 30);
            timer.Tick += timer_Tick;
            timer.Start();

            this.DataContext = this;
        }

        void timer_Tick(object sender, object e)
        {
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PageManager.RegisterPage(this);
            StatusBar.GetForCurrentView().HideAsync();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            timer.Tick -= timer_Tick;
            timer.Stop();
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
            SampleText = "asdasdasdasdccgcgcgc";
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
