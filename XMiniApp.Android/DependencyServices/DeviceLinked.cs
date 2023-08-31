using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using XMiniApp.DependencyServices;
using XMiniApp.Droid;

[assembly: Dependency(typeof(DeviceLinkedService))]
namespace XMiniApp.Droid
{
    public class DeviceLinkedService : IDeviceLinkedService
    {
        public string GetDeviceID()
        {
            return Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
        }
    }
}