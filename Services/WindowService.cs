using System.Windows;

namespace QLXeMay.Services
{
    internal sealed class WindowService : IWindowService
    {
        private readonly Window owner;

        public WindowService(Window owner)
        {
            this.owner = owner;
        }

        public void ShowDialog(Window window)
        {
            window.Owner = owner;
            window.ShowDialog();
        }
    }
}
