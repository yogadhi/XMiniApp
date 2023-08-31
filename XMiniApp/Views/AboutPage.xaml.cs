using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XMiniApp.ViewModels;

namespace XMiniApp.Views
{
    public partial class AboutPage : ContentPage
    {
        AboutViewModel vm;
        public AboutPage()
        {
            try
            {
                InitializeComponent();
                vm = (AboutViewModel)BindingContext;
            }
            catch (Exception ex)
            {
                string exMsg = ex.Message;
            }
        }

        protected override void OnAppearing()
        {
            vm.LoadCommand.Execute(null);
        }
    }
}