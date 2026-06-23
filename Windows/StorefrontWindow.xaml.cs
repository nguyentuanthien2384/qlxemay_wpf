using System.Windows;
using QLXeMay.Infrastructure;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class StorefrontWindow : Window
    {
        public StorefrontWindow()
        {
            InitializeComponent();
            DataContext = new StorefrontViewModel(
                new StoreService(),
                new DialogService(),
                AppSession.CurrentUser,
                SignOut);
        }

        private void SignOut()
        {
            if (Application.Current is App app)
            {
                app.SignOut(this);
            }
        }
    }
}
