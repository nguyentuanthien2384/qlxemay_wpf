using System.Collections.Generic;
using QLXeMay.Class;

namespace QLXeMay.Services
{
    internal sealed class AiAssistantService : IAiAssistantService
    {
        public IReadOnlyList<string> GetSampleQuestions() => AIEngine.LayCauHoiMau();
        public string ExecutiveSummary() => AIEngine.TongQuanDieuHanh();
        public string ForecastRevenue() => AIEngine.DuBaoDoanhThu();
        public string ReorderAdvice() => AIEngine.GoiYNhapHang();
        public string RiskWarning() => AIEngine.CanhBaoRuiRo();
        public string AnalyzeInventory() => AIEngine.PhanTichTonKho();
        public string AnalyzeRevenue() => AIEngine.PhanTichDoanhThu();
        public string AnalyzeCustomers() => AIEngine.PhanTichKhachHang();
        public string AnalyzeEmployees() => AIEngine.PhanTichNhanVien();
        public string AdviseProduct(string question) => AIEngine.TuVanSanPham(question);
        public string SmartSearch(string keyword) => AIEngine.TimKiemThongMinh(keyword);
        public string AnswerQuestion(string question) => AIEngine.TraLoiCauHoi(question);
        public string SampleScenarios() => AIEngine.TaoKichBanMau();
    }
}
