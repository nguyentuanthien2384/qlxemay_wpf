using QLXeMay.Class;

namespace QLXeMay.Services
{
    internal sealed class AiAssistantService : IAiAssistantService
    {
        public string AnalyzeInventory() => AIEngine.PhanTichTonKho();
        public string AnalyzeRevenue() => AIEngine.PhanTichDoanhThu();
        public string AnalyzeCustomers() => AIEngine.PhanTichKhachHang();
        public string AnalyzeEmployees() => AIEngine.PhanTichNhanVien();
        public string AdviseProduct(string question) => AIEngine.TuVanSanPham(question);
        public string SmartSearch(string keyword) => AIEngine.TimKiemThongMinh(keyword);
        public string AnswerQuestion(string question) => AIEngine.TraLoiCauHoi(question);
    }
}
