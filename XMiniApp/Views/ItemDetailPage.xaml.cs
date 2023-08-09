using System.ComponentModel;
using Xamarin.Forms;
using XMiniApp.ViewModels;

namespace XMiniApp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}