﻿using GravitasApp.Helpers;
using GravitasApp.Managers;
using GravitasSDK.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace GravitasApp
{

    public sealed partial class IntroPage : Page, IManageable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DispatcherTimer _timer;
        private Tuple<string, string> _displayQuote;
        private List<Tuple<string, string>> _quotes;
        private Dictionary<string, string> _buttonContents;
        private string _buttonContent;
        private string _currentState;

        public Tuple<string, string> DisplayQuote
        {

            get { return _displayQuote; }
            set
            {
                _displayQuote = value;
                NotifyPropertyChanged();
            }
        }
        public string ButtonContent
        {
            get { return _buttonContent; }
            set
            {
                _buttonContent = value;
                NotifyPropertyChanged();
            }
        }

        public IntroPage()
        {
            this.InitializeComponent();

            _quotes = new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("A designer is an emerging synthesis of artist, inventor, mechanic, objective economist and evolutio­nary strategist.", "BUCKMINSTER FULLER"),
                new Tuple<string,string>("It almost goes without saying that when you are a startup, one of the first things you do is you start setting aside money to defend yourself from patent lawsuits, because any successful company, even moderately successful, is going to get hit by a patent lawsuit from someone who's just trying to look for a payout.", "CHARLES DUHIGG"),
                new Tuple<string,string>("Emile Berliner, a German immigrant working in Washington D.C., patented a successful system of sound recording. This system brought huge revolution in the field of music and recorders which we cherish.", "#FACT"),
            };
            DisplayQuote = _quotes[0];

            _buttonContents = new Dictionary<string, string>()
            {
                {"start","begin"},
                {"progress","downloading content..."},
                {"complete","start browsing"},
                {"failed","download failed. Try again"},
                {"refresh", "updating content..."},
                {"refreshDone", "update successful"},
                {"refreshFail", "update failed"}
            };

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 8);

            SetButtonState("start");
            this.DataContext = this;
        }

        private void Timer_Tick(object sender, object e)
        {
            int nextIndex = (_quotes.IndexOf(DisplayQuote) + 1) % _quotes.Count;
            DisplayQuote = _quotes[nextIndex];
        }

        private void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PageManager.RegisterPage(this);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            _timer.Tick -= Timer_Tick;
            _timer = null;
        }

        public Dictionary<string, object> SaveState()
        {
            return null;
        }

        public async void LoadState(Dictionary<string, object> lastState)
        {
            if (DataManager.ContentReady == true)
            {
                SetButtonState("refresh");
                StatusCode code = await Task.Run(() => DataManager.RefreshEventsAsync());
                if (code == StatusCode.Success || code == StatusCode.NoChange)
                    SetButtonState("refreshDone");
                else
                    SetButtonState("refreshFail");

                await Task.Delay(300);
                PageManager.NavigateTo(typeof(MainPage), null, NavigationType.FreshStart);
            }
        }

        public bool AllowAppExit()
        {
            return true;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == null)
                return;
            else if (_currentState == "start" || _currentState == "failed")
            {
                SetButtonState("progress");
                StatusCode code = await DataManager.RefreshEventsAsync();
                if (code == StatusCode.Success && DataManager.ContentReady == true)
                    SetButtonState("complete");
                else
                {
                    SetButtonState("failed");
                    await MessageDialogs.GetDialog(code).ShowAsync();
                }
            }
            else if (_currentState == "complete")
                PageManager.NavigateTo(typeof(MainPage), null, NavigationType.FreshStart);
        }

        private void SetButtonState(string state)
        {
            switch (state)
            {
                case "start":
                    displayButton.IsEnabled = true;
                    progressRing.IsActive = false;
                    displayButton.Background = new SolidColorBrush() { Color = new Windows.UI.Color() { R = 55, G = 55, B = 55, A = 170 } };
                    break;
                case "progress":
                    displayButton.IsEnabled = false;
                    progressRing.IsActive = true;
                    break;
                case "complete":
                    displayButton.IsEnabled = true;
                    progressRing.IsActive = false;
                    displayButton.Background = new SolidColorBrush() { Color = new Windows.UI.Color() { R = 80, G = 170, B = 80, A = 170 } };
                    break;
                case "failed":
                    displayButton.IsEnabled = true;
                    progressRing.IsActive = false;
                    displayButton.Background = new SolidColorBrush() { Color = new Windows.UI.Color() { R = 170, G = 10, B = 10, A = 170 } };
                    break;
                case "refresh":
                    displayButton.IsEnabled = false;
                    progressRing.IsActive = true;
                    break;
                case "refreshDone":
                    displayButton.IsEnabled = false;
                    progressRing.IsActive = false;
                    displayButton.Background = new SolidColorBrush() { Color = new Windows.UI.Color() { R = 80, G = 170, B = 80, A = 170 } };
                    break;
                case "refreshFail":
                    displayButton.IsEnabled = false;
                    progressRing.IsActive = false;
                    displayButton.Background = new SolidColorBrush() { Color = new Windows.UI.Color() { R = 170, G = 10, B = 10, A = 170 } };
                    break;

            }
            _currentState = state;
            ButtonContent = _buttonContents[state];
        }

    }
}
