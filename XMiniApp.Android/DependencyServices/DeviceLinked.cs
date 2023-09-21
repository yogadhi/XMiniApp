using Android.Widget;
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

        public void LongAlert(string message)
        {
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
        }
    }
}