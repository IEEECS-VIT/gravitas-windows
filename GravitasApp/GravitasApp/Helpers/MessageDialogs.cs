using GravitasSDK.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace GravitasApp.Helpers
{
    public static class MessageDialogs
    {
        public static MessageDialog GetDialog(StatusCode status)
        {
            switch (status)
            {
                case StatusCode.NoInternet:
                    return new MessageDialog("Please check your internet connection and try again.", "No Internet");
                case StatusCode.ServerError:
                    return new MessageDialog("Please try after sometime, our servers are under maintenance or busy.", "Server Error");
                case StatusCode.Busy:
                    return new MessageDialog("Please wait while the last process completes.", "Busy");
                case StatusCode.UnknownError:
                    return new MessageDialog("Something unexpected happened, please try later or send a feedback mail.", "Oops...");
                default: 
                    return new MessageDialog(status.ToString());
            }
        }
    }

}
