namespace QLXeMay.Services
{
    internal interface IDialogService
    {
        void ShowInformation(string message, string title = "Thông báo");
        void ShowWarning(string message, string title = "Thông báo");
        void ShowError(string message, string title = "Lỗi");
        bool Confirm(string message, string title = "Xác nhận");
    }
}
