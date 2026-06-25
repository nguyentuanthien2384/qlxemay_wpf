using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace QLXeMay.Class
{
    /// <summary>
    /// AI Engine offline - phân tích dữ liệu cửa hàng xe máy theo hướng Business Intelligence.
    /// Không cần API key, không cần internet. Kết quả dựa trên dữ liệu thật trong SQL Server.
    /// </summary>
    public static class AIEngine
    {
        private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");

        public static IReadOnlyList<string> LayCauHoiMau()
        {
            return new[]
            {
                "Tổng quan tình hình kinh doanh hôm nay",
                "Dự báo doanh thu 30 ngày tới và mức độ tin cậy",
                "Xe nào nên nhập thêm trong tháng này?",
                "Sản phẩm nào tồn kho cao cần khuyến mãi?",
                "Phân tích rủi ro kinh doanh hiện tại",
                "Gợi ý xe dưới 40 triệu cho sinh viên đi học",
                "Tư vấn xe tay ga cho khách nữ đi trong thành phố",
                "Top xe bán chạy và lý do nên tiếp tục nhập",
                "Khách hàng VIP cần chăm sóc như thế nào?",
                "Nhân viên nào đang có hiệu suất bán tốt nhất?",
                "Tìm khách hàng ở Hà Nội",
                "Tìm xe Honda màu trắng còn hàng",
                "Có xe nào sắp hết hàng không?",
                "Kế hoạch khuyến mãi để giảm tồn kho",
                "So sánh doanh thu tháng này với tháng trước"
            };
        }

        // ===== TỔNG QUAN ĐIỀU HÀNH =====
        public static string TongQuanDieuHanh()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("📌 TỔNG QUAN ĐIỀU HÀNH CỬA HÀNG");
            sb.AppendLine("════════════════════════════════════════════════════");

            DataTable revenue = Function.GetDataToTable(@"
                SELECT
                    ISNULL(SUM(CASE WHEN ngaymua >= DATEADD(day,-30,CONVERT(date,GETDATE())) THEN tongtien ELSE 0 END),0) AS DoanhThu30Ngay,
                    ISNULL(SUM(CASE WHEN ngaymua >= DATEADD(day,-60,CONVERT(date,GETDATE())) AND ngaymua < DATEADD(day,-30,CONVERT(date,GETDATE())) THEN tongtien ELSE 0 END),0) AS DoanhThu30NgayTruoc,
                    SUM(CASE WHEN ngaymua >= DATEADD(day,-30,CONVERT(date,GETDATE())) THEN 1 ELSE 0 END) AS SoDon30Ngay,
                    ISNULL(AVG(CASE WHEN ngaymua >= DATEADD(day,-30,CONVERT(date,GETDATE())) THEN tongtien END),0) AS GiaTriDonTB
                FROM tbldondathang");

            DataTable purchase = Function.GetDataToTable(@"
                SELECT ISNULL(SUM(CASE WHEN ngaynhap >= DATEADD(day,-30,CONVERT(date,GETDATE())) THEN tongtien ELSE 0 END),0) AS ChiPhiNhap30Ngay
                FROM tblhoadonnhap");

            DataTable inventory = Function.GetDataToTable(@"
                SELECT COUNT(*) AS SoMatHang,
                       ISNULL(SUM(soluong),0) AS TongTon,
                       ISNULL(SUM(soluong * dongianhap),0) AS VonTonKho,
                       ISNULL(SUM(soluong * dongiaban),0) AS GiaTriBanDuKien,
                       SUM(CASE WHEN soluong = 0 THEN 1 ELSE 0 END) AS HetHang,
                       SUM(CASE WHEN soluong BETWEEN 1 AND 3 THEN 1 ELSE 0 END) AS SapHet
                FROM tbldmhang");

            DataTable customers = Function.GetDataToTable(@"
                SELECT COUNT(*) AS TongKH,
                       SUM(CASE WHEN EXISTS (SELECT 1 FROM tbldondathang d WHERE d.makhach = kh.makhach) THEN 1 ELSE 0 END) AS DaMua,
                       SUM(CASE WHEN NOT EXISTS (SELECT 1 FROM tbldondathang d WHERE d.makhach = kh.makhach) THEN 1 ELSE 0 END) AS ChuaMua
                FROM tblkhachhang kh");

            double doanhThu30 = FirstDouble(revenue, "DoanhThu30Ngay");
            double doanhThuTruoc = FirstDouble(revenue, "DoanhThu30NgayTruoc");
            double chiPhiNhap30 = FirstDouble(purchase, "ChiPhiNhap30Ngay");
            double loiNhuanUocTinh = doanhThu30 - chiPhiNhap30;
            int soDon30 = FirstInt(revenue, "SoDon30Ngay");
            double aov = FirstDouble(revenue, "GiaTriDonTB");
            double growth = doanhThuTruoc <= 0 ? 0 : (doanhThu30 - doanhThuTruoc) / doanhThuTruoc * 100.0;

            sb.AppendLine("\nI. KPI 30 ngày gần nhất");
            sb.AppendLine($"   • Doanh thu bán hàng: {Money(doanhThu30)}");
            sb.AppendLine($"   • Chi phí nhập hàng: {Money(chiPhiNhap30)}");
            sb.AppendLine($"   • Lợi nhuận ước tính: {Money(loiNhuanUocTinh)}");
            sb.AppendLine($"   • Số đơn bán: {soDon30:N0} đơn | Giá trị trung bình/đơn: {Money(aov)}");
            if (doanhThuTruoc > 0)
                sb.AppendLine($"   • Tăng trưởng so với 30 ngày trước: {growth:F1}% ({DanhGiaTangTruong(growth)})");
            else
                sb.AppendLine("   • Tăng trưởng: chưa đủ dữ liệu kỳ trước để so sánh.");

            sb.AppendLine("\nII. Tồn kho và khách hàng");
            sb.AppendLine($"   • Mặt hàng đang quản lý: {FirstInt(inventory, "SoMatHang"):N0}");
            sb.AppendLine($"   • Tổng xe tồn kho: {FirstInt(inventory, "TongTon"):N0} chiếc");
            sb.AppendLine($"   • Vốn nằm trong kho: {Money(FirstDouble(inventory, "VonTonKho"))}");
            sb.AppendLine($"   • Hết hàng: {FirstInt(inventory, "HetHang"):N0} mã | Sắp hết: {FirstInt(inventory, "SapHet"):N0} mã");
            sb.AppendLine($"   • Khách hàng: {FirstInt(customers, "TongKH"):N0} | Đã mua: {FirstInt(customers, "DaMua"):N0} | Chưa mua: {FirstInt(customers, "ChuaMua"):N0}");

            DataTable topProducts = Function.GetDataToTable(@"
                SELECT TOP 5 h.tenhang, SUM(ct.soluong) AS SoLuongBan, SUM(ct.thanhtien) AS DoanhThu
                FROM tblchitietddh ct
                INNER JOIN tbldondathang d ON ct.soddh = d.soddh
                INNER JOIN tbldmhang h ON ct.mahang = h.mahang
                WHERE d.ngaymua >= DATEADD(day,-90,CONVERT(date,GETDATE()))
                GROUP BY h.tenhang
                ORDER BY SUM(ct.thanhtien) DESC");
            AppendTopProducts(sb, topProducts, "III. Top sản phẩm tạo doanh thu 90 ngày");

            sb.AppendLine("\nIV. Khuyến nghị ưu tiên");
            if (doanhThu30 <= 0)
            {
                sb.AppendLine("   1. Chưa có doanh thu trong 30 ngày gần nhất: cần nhập dữ liệu bán hàng mẫu hoặc kiểm tra quy trình lập đơn.");
            }
            else
            {
                if (growth < -10) sb.AppendLine("   1. Doanh thu đang giảm: cần kiểm tra sản phẩm bán chậm, chương trình khuyến mãi và hiệu suất nhân viên bán hàng.");
                else sb.AppendLine("   1. Duy trì nhóm sản phẩm đang tạo doanh thu tốt và bổ sung tồn kho trước khi thiếu hàng.");
            }

            int hetHang = FirstInt(inventory, "HetHang");
            int sapHet = FirstInt(inventory, "SapHet");
            if (hetHang + sapHet > 0)
                sb.AppendLine($"   2. Có {hetHang + sapHet} mã hàng cần theo dõi nhập bổ sung để tránh mất đơn.");
            else
                sb.AppendLine("   2. Tồn kho hiện chưa có cảnh báo thiếu hàng nghiêm trọng.");

            if (FirstInt(customers, "ChuaMua") > 0)
                sb.AppendLine("   3. Tạo danh sách chăm sóc khách chưa mua để tăng tỷ lệ chuyển đổi.");
            sb.AppendLine("   4. Dùng câu hỏi mẫu: 'Dự báo doanh thu 30 ngày tới' hoặc 'Xe nào nên nhập thêm?' để xem phân tích sâu hơn.");

            return sb.ToString();
        }

        // ===== PHÂN TÍCH TỒN KHO =====
        public static string PhanTichTonKho()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("📦 PHÂN TÍCH TỒN KHO CHUYÊN NGHIỆP");
            sb.AppendLine("════════════════════════════════════════════════════");

            DataTable summary = Function.GetDataToTable(@"
                SELECT COUNT(*) AS SoSP,
                       ISNULL(SUM(soluong),0) AS TongTon,
                       ISNULL(AVG(NULLIF(dongiaban,0)),0) AS GiaTB,
                       ISNULL(SUM(soluong * dongianhap),0) AS VonTon,
                       ISNULL(SUM(soluong * dongiaban),0) AS GiaTriBan,
                       SUM(CASE WHEN soluong = 0 THEN 1 ELSE 0 END) AS HetHang,
                       SUM(CASE WHEN soluong BETWEEN 1 AND 3 THEN 1 ELSE 0 END) AS SapHet,
                       SUM(CASE WHEN soluong >= 10 THEN 1 ELSE 0 END) AS TonCao
                FROM tbldmhang");

            if (FirstInt(summary, "SoSP") == 0)
            {
                return "📦 Chưa có dữ liệu hàng hóa. Hãy thêm xe máy hoặc chạy Database/SampleData_Professional.sql để có dữ liệu mẫu cho AI phân tích.";
            }

            double vonTon = FirstDouble(summary, "VonTon");
            double giaTriBan = FirstDouble(summary, "GiaTriBan");
            double laiGopDuKien = giaTriBan - vonTon;
            double margin = giaTriBan <= 0 ? 0 : laiGopDuKien / giaTriBan * 100.0;

            sb.AppendLine("\nI. Tổng quan tồn kho");
            sb.AppendLine($"   • Số loại xe: {FirstInt(summary, "SoSP"):N0}");
            sb.AppendLine($"   • Tổng số xe trong kho: {FirstInt(summary, "TongTon"):N0} chiếc");
            sb.AppendLine($"   • Giá bán trung bình: {Money(FirstDouble(summary, "GiaTB"))}");
            sb.AppendLine($"   • Vốn tồn kho: {Money(vonTon)}");
            sb.AppendLine($"   • Giá trị bán dự kiến: {Money(giaTriBan)}");
            sb.AppendLine($"   • Lãi gộp dự kiến nếu bán hết: {Money(laiGopDuKien)} ({margin:F1}%)");

            DataTable lowStock = Function.GetDataToTable(@"
                SELECT TOP 10 h.mahang, h.tenhang, h.soluong, h.dongiaban, sx.tenhangsx,
                       ISNULL(SUM(CASE WHEN d.ngaymua >= DATEADD(day,-90,CONVERT(date,GETDATE())) THEN ct.soluong ELSE 0 END),0) AS Ban90Ngay
                FROM tbldmhang h
                LEFT JOIN tblhangsx sx ON h.mahangsx = sx.mahangsx
                LEFT JOIN tblchitietddh ct ON h.mahang = ct.mahang
                LEFT JOIN tbldondathang d ON ct.soddh = d.soddh
                WHERE h.soluong <= 3
                GROUP BY h.mahang, h.tenhang, h.soluong, h.dongiaban, sx.tenhangsx
                ORDER BY h.soluong ASC, Ban90Ngay DESC");
            AppendInventoryList(sb, lowStock, "\nII. Hàng hết/sắp hết cần xử lý", true);

            DataTable overStock = Function.GetDataToTable(@"
                SELECT TOP 10 h.mahang, h.tenhang, h.soluong, h.dongiaban, sx.tenhangsx,
                       ISNULL(SUM(CASE WHEN d.ngaymua >= DATEADD(day,-90,CONVERT(date,GETDATE())) THEN ct.soluong ELSE 0 END),0) AS Ban90Ngay
                FROM tbldmhang h
                LEFT JOIN tblhangsx sx ON h.mahangsx = sx.mahangsx
                LEFT JOIN tblchitietddh ct ON h.mahang = ct.mahang
                LEFT JOIN tbldondathang d ON ct.soddh = d.soddh
                WHERE h.soluong > 0
                GROUP BY h.mahang, h.tenhang, h.soluong, h.dongiaban, sx.tenhangsx
                ORDER BY CASE WHEN ISNULL(SUM(CASE WHEN d.ngaymua >= DATEADD(day,-90,CONVERT(date,GETDATE())) THEN ct.soluong ELSE 0 END),0)=0 THEN 1 ELSE 0 END DESC,
                         h.soluong DESC");
            AppendInventoryList(sb, overStock, "\nIII. Hàng cần theo dõi tồn kho cao/bán chậm", false);

            sb.AppendLine("\nIV. Hành động đề xuất");
            if (FirstInt(summary, "HetHang") > 0) sb.AppendLine($"   • Nhập ngay {FirstInt(summary, "HetHang")} mã đã hết hàng, ưu tiên mã có lịch sử bán trong 90 ngày.");
            if (FirstInt(summary, "SapHet") > 0) sb.AppendLine($"   • Lập đơn nhập bổ sung cho {FirstInt(summary, "SapHet")} mã còn 1-3 chiếc.");
            if (FirstInt(summary, "TonCao") > 0) sb.AppendLine($"   • Kiểm tra {FirstInt(summary, "TonCao")} mã tồn cao; nếu bán 90 ngày = 0 thì áp dụng combo, giảm giá hoặc đẩy kênh online.");
            sb.AppendLine("   • Mức tồn an toàn gợi ý: đủ bán 21-30 ngày, không để sản phẩm bán chạy xuống dưới 2 chiếc.");

            return sb.ToString();
        }

        // ===== PHÂN TÍCH DOANH THU =====
        public static string PhanTichDoanhThu()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("💰 PHÂN TÍCH DOANH THU VÀ LỢI NHUẬN");
            sb.AppendLine("════════════════════════════════════════════════════");

            DataTable total = Function.GetDataToTable(@"
                SELECT COUNT(*) AS SoHD,
                       ISNULL(SUM(tongtien),0) AS TongDT,
                       ISNULL(AVG(NULLIF(tongtien,0)),0) AS DonTB,
                       MIN(ngaymua) AS NgayDau,
                       MAX(ngaymua) AS NgayCuoi
                FROM tbldondathang");
            DataTable purchase = Function.GetDataToTable(@"
                SELECT COUNT(*) AS SoHDN, ISNULL(SUM(tongtien),0) AS TongNhap
                FROM tblhoadonnhap");

            double tongBan = FirstDouble(total, "TongDT");
            double tongNhap = FirstDouble(purchase, "TongNhap");
            double loiNhuan = tongBan - tongNhap;
            int soHDBan = FirstInt(total, "SoHD");
            int soHDNhap = FirstInt(purchase, "SoHDN");

            sb.AppendLine("\nI. Tổng quan toàn kỳ");
            sb.AppendLine($"   • Tổng doanh thu bán: {Money(tongBan)} ({soHDBan:N0} đơn)");
            sb.AppendLine($"   • Tổng chi phí nhập: {Money(tongNhap)} ({soHDNhap:N0} hóa đơn nhập)");
            sb.AppendLine($"   • Lợi nhuận ước tính: {Money(loiNhuan)}");
            if (tongBan > 0) sb.AppendLine($"   • Biên lợi nhuận ước tính: {loiNhuan / tongBan * 100.0:F1}%");
            if (soHDBan > 0) sb.AppendLine($"   • Giá trị trung bình/đơn: {Money(FirstDouble(total, "DonTB"))}");

            DataTable monthly = Function.GetDataToTable(@"
                SELECT TOP 6 CONVERT(char(7), ngaymua, 120) AS Thang,
                       COUNT(*) AS SoDon,
                       SUM(tongtien) AS DoanhThu
                FROM tbldondathang
                GROUP BY CONVERT(char(7), ngaymua, 120)
                ORDER BY Thang DESC");
            AppendMonthlyRevenue(sb, monthly);

            DataTable topProducts = Function.GetDataToTable(@"
                SELECT TOP 5 h.tenhang, SUM(ct.soluong) AS SoLuongBan, SUM(ct.thanhtien) AS DoanhThu,
                       SUM(ct.thanhtien - ct.soluong * h.dongianhap) AS LaiGop
                FROM tblchitietddh ct
                INNER JOIN tbldmhang h ON ct.mahang = h.mahang
                GROUP BY h.tenhang
                ORDER BY SUM(ct.thanhtien) DESC");
            AppendTopProducts(sb, topProducts, "\nIII. Top xe đóng góp doanh thu");

            DataTable slowProducts = Function.GetDataToTable(@"
                SELECT TOP 8 h.tenhang, h.soluong, h.dongiaban, h.soluong * h.dongiaban AS GiaTriTon
                FROM tbldmhang h
                WHERE h.soluong > 0 AND NOT EXISTS (SELECT 1 FROM tblchitietddh ct WHERE ct.mahang = h.mahang)
                ORDER BY h.soluong * h.dongiaban DESC");
            if (slowProducts.Rows.Count > 0)
            {
                sb.AppendLine("\nIV. Sản phẩm đang tồn nhưng chưa phát sinh bán");
                foreach (DataRow row in slowProducts.Rows)
                {
                    sb.AppendLine($"   • {Text(row, "tenhang")}: tồn {Int(row, "soluong"):N0} chiếc, giá trị bán dự kiến {Money(Double(row, "GiaTriTon"))}");
                }
            }

            sb.AppendLine("\nV. Nhận định và đề xuất");
            if (tongBan <= 0)
            {
                sb.AppendLine("   • Chưa có doanh thu. Hãy nhập dữ liệu bán hàng mẫu hoặc lập đơn bán để báo cáo có ý nghĩa.");
            }
            else if (loiNhuan < 0)
            {
                sb.AppendLine("   • Cảnh báo lỗ ước tính: cần kiểm soát nhập hàng, giá bán và tồn kho chậm luân chuyển.");
            }
            else if (loiNhuan / tongBan < 0.10)
            {
                sb.AppendLine("   • Biên lợi nhuận dưới 10%: nên rà soát chính sách giá, giảm giá và nguồn nhập.");
            }
            else
            {
                sb.AppendLine("   • Biên lợi nhuận đang ở mức chấp nhận được. Nên tập trung đẩy nhóm xe có vòng quay tốt.");
            }
            if (topProducts.Rows.Count > 0) sb.AppendLine($"   • Ưu tiên truyền thông và nhập thêm: {Text(topProducts.Rows[0], "tenhang")}.");
            if (slowProducts.Rows.Count > 0) sb.AppendLine("   • Với sản phẩm chưa bán, nên tạo gói khuyến mãi hoặc đặt ở vị trí nổi bật trên trang bán hàng.");

            return sb.ToString();
        }

        // ===== DỰ BÁO DOANH THU =====
        public static string DuBaoDoanhThu()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("📈 DỰ BÁO DOANH THU 30 NGÀY TỚI");
            sb.AppendLine("════════════════════════════════════════════════════");

            DataTable window = Function.GetDataToTable(@"
                SELECT COUNT(*) AS SoDon,
                       ISNULL(SUM(tongtien),0) AS TongDoanhThu,
                       ISNULL(AVG(NULLIF(tongtien,0)),0) AS DonTrungBinh,
                       MIN(ngaymua) AS NgayDau,
                       MAX(ngaymua) AS NgayCuoi,
                       DATEDIFF(day, MIN(ngaymua), MAX(ngaymua)) + 1 AS SoNgayDuLieu
                FROM tbldondathang
                WHERE ngaymua >= DATEADD(day,-180,CONVERT(date,GETDATE()))");

            int soDon = FirstInt(window, "SoDon");
            if (soDon == 0)
            {
                sb.AppendLine("\nChưa có dữ liệu bán hàng trong 180 ngày gần nhất nên chưa thể dự báo đáng tin cậy.");
                sb.AppendLine("Gợi ý: chạy Database/SampleData_Professional.sql hoặc nhập ít nhất 10-20 đơn bán mẫu để AI phân tích xu hướng.");
                return sb.ToString();
            }

            double total = FirstDouble(window, "TongDoanhThu");
            double aov = FirstDouble(window, "DonTrungBinh");
            int days = Math.Max(1, FirstInt(window, "SoNgayDuLieu"));
            double avgDaily = total / days;

            DataTable lastPeriods = Function.GetDataToTable(@"
                SELECT
                    ISNULL(SUM(CASE WHEN ngaymua >= DATEADD(day,-30,CONVERT(date,GETDATE())) THEN tongtien ELSE 0 END),0) AS Last30,
                    ISNULL(SUM(CASE WHEN ngaymua >= DATEADD(day,-60,CONVERT(date,GETDATE())) AND ngaymua < DATEADD(day,-30,CONVERT(date,GETDATE())) THEN tongtien ELSE 0 END),0) AS Prev30,
                    SUM(CASE WHEN ngaymua >= DATEADD(day,-30,CONVERT(date,GETDATE())) THEN 1 ELSE 0 END) AS OrdersLast30,
                    SUM(CASE WHEN ngaymua >= DATEADD(day,-60,CONVERT(date,GETDATE())) AND ngaymua < DATEADD(day,-30,CONVERT(date,GETDATE())) THEN 1 ELSE 0 END) AS OrdersPrev30
                FROM tbldondathang");
            double last30 = FirstDouble(lastPeriods, "Last30");
            double prev30 = FirstDouble(lastPeriods, "Prev30");
            double trend = prev30 <= 0 ? 0 : (last30 - prev30) / prev30;
            double trendFactor = Clamp(1 + trend * 0.35, 0.75, 1.35);
            double forecastBase = avgDaily * 30;
            double forecast = forecastBase * trendFactor;
            double low = forecast * 0.85;
            double high = forecast * 1.15;

            string confidence;
            if (soDon >= 30 && days >= 90) confidence = "Cao";
            else if (soDon >= 10 && days >= 45) confidence = "Trung bình";
            else confidence = "Thấp";

            sb.AppendLine("\nI. Dữ liệu đầu vào");
            sb.AppendLine($"   • Số đơn trong 180 ngày: {soDon:N0}");
            sb.AppendLine($"   • Doanh thu ghi nhận: {Money(total)}");
            sb.AppendLine($"   • Doanh thu trung bình/ngày: {Money(avgDaily)}");
            sb.AppendLine($"   • Giá trị trung bình/đơn: {Money(aov)}");
            sb.AppendLine($"   • Xu hướng 30 ngày gần nhất: {(prev30 > 0 ? (trend * 100.0).ToString("F1", ViCulture) + "%" : "chưa đủ dữ liệu so sánh")}");

            sb.AppendLine("\nII. Kết quả dự báo");
            sb.AppendLine($"   • Doanh thu dự báo 30 ngày tới: {Money(forecast)}");
            sb.AppendLine($"   • Khoảng dự báo hợp lý: {Money(low)} - {Money(high)}");
            sb.AppendLine($"   • Độ tin cậy: {confidence}");
            sb.AppendLine($"   • Số đơn cần đạt nếu giữ AOV hiện tại: {Math.Ceiling(forecast / Math.Max(aov, 1)):N0} đơn");

            sb.AppendLine("\nIII. Diễn giải chuyên nghiệp");
            if (trend > 0.15) sb.AppendLine("   • Nhu cầu đang tăng, cần kiểm tra tồn kho nhóm xe bán chạy để tránh mất doanh thu.");
            else if (trend < -0.15) sb.AppendLine("   • Nhu cầu đang giảm, nên kích hoạt khuyến mãi hoặc chăm sóc lại khách hàng tiềm năng.");
            else sb.AppendLine("   • Xu hướng khá ổn định, có thể dùng mức bán trung bình làm mục tiêu vận hành tháng tới.");
            sb.AppendLine("   • Đây là dự báo thống kê đơn giản dựa trên dữ liệu nội bộ, không thay thế phân tích thị trường bên ngoài.");

            return sb.ToString();
        }

        // ===== GỢI Ý NHẬP HÀNG =====
        public static string GoiYNhapHang()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("🚚 GỢI Ý NHẬP HÀNG THEO TỐC ĐỘ BÁN");
            sb.AppendLine("════════════════════════════════════════════════════");

            List<ReorderRecommendation> recommendations = GetReorderRecommendations();
            if (recommendations.Count == 0)
            {
                sb.AppendLine("\nChưa phát hiện mã hàng cần nhập gấp theo dữ liệu hiện tại.");
                sb.AppendLine("Gợi ý: vẫn nên kiểm tra thủ công các xe chiến lược, xe mới ra mắt hoặc đơn đặt trước của khách.");
                return sb.ToString();
            }

            sb.AppendLine("\nI. Danh sách ưu tiên nhập");
            int index = 1;
            foreach (ReorderRecommendation item in recommendations)
            {
                sb.AppendLine($"   {index}. {item.ProductName} [{item.ProductCode}]");
                sb.AppendLine($"      Tồn hiện tại: {item.Stock:N0} chiếc | Bán 90 ngày: {item.Sold90:N0} chiếc | Tốc độ: {item.DailyVelocity:F2} chiếc/ngày");
                sb.AppendLine($"      Mức tồn an toàn: {item.ReorderPoint:N0} chiếc | Đề xuất nhập: {item.SuggestedQty:N0} chiếc");
                sb.AppendLine($"      Biên lợi nhuận/SP: {Money(item.MarginPerUnit)} ({item.MarginRate:F1}%) | Lý do: {item.Reason}");
                index++;
            }

            double neededCapital = 0;
            foreach (ReorderRecommendation item in recommendations)
            {
                neededCapital += item.SuggestedQty * item.Cost;
            }

            sb.AppendLine("\nII. Ngân sách dự kiến");
            sb.AppendLine($"   • Vốn nhập đề xuất: {Money(neededCapital)}");
            sb.AppendLine("   • Ưu tiên nhập trước các mã có tốc độ bán cao, lợi nhuận tốt và tồn kho dưới mức an toàn.");
            sb.AppendLine("   • Nếu ngân sách hạn chế, nhập 50-70% số lượng đề xuất cho nhóm ưu tiên 1-3.");

            return sb.ToString();
        }

        // ===== CẢNH BÁO RỦI RO =====
        public static string CanhBaoRuiRo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("⚠️ PHÂN TÍCH RỦI RO KINH DOANH");
            sb.AppendLine("════════════════════════════════════════════════════");

            List<string> high = new List<string>();
            List<string> medium = new List<string>();
            List<string> low = new List<string>();

            DataTable rev = Function.GetDataToTable(@"
                SELECT ISNULL(SUM(CASE WHEN ngaymua >= DATEADD(day,-30,CONVERT(date,GETDATE())) THEN tongtien ELSE 0 END),0) AS Last30,
                       ISNULL(SUM(CASE WHEN ngaymua >= DATEADD(day,-60,CONVERT(date,GETDATE())) AND ngaymua < DATEADD(day,-30,CONVERT(date,GETDATE())) THEN tongtien ELSE 0 END),0) AS Prev30
                FROM tbldondathang");
            double last30 = FirstDouble(rev, "Last30");
            double prev30 = FirstDouble(rev, "Prev30");
            if (prev30 > 0)
            {
                double growth = (last30 - prev30) / prev30 * 100.0;
                if (growth < -20) high.Add($"Doanh thu 30 ngày giảm {Math.Abs(growth):F1}% so với kỳ trước.");
                else if (growth < -5) medium.Add($"Doanh thu có dấu hiệu giảm {Math.Abs(growth):F1}% so với kỳ trước.");
                else low.Add($"Doanh thu không giảm mạnh; tăng trưởng kỳ gần nhất {growth:F1}%.");
            }

            DataTable stock = Function.GetDataToTable(@"
                SELECT SUM(CASE WHEN soluong = 0 THEN 1 ELSE 0 END) AS HetHang,
                       SUM(CASE WHEN soluong BETWEEN 1 AND 3 THEN 1 ELSE 0 END) AS SapHet,
                       ISNULL(SUM(CASE WHEN soluong > 0 AND NOT EXISTS (SELECT 1 FROM tblchitietddh ct WHERE ct.mahang = tbldmhang.mahang) THEN soluong * dongianhap ELSE 0 END),0) AS VonCham
                FROM tbldmhang");
            int het = FirstInt(stock, "HetHang");
            int sapHet = FirstInt(stock, "SapHet");
            double vonCham = FirstDouble(stock, "VonCham");
            if (het > 0) high.Add($"Có {het} mã hết hàng, rủi ro mất đơn nếu khách hỏi mua.");
            if (sapHet > 0) medium.Add($"Có {sapHet} mã sắp hết hàng, cần theo dõi nhập bổ sung.");
            if (vonCham > 0) medium.Add($"Vốn nằm ở sản phẩm chưa phát sinh bán: {Money(vonCham)}.");

            DataTable margin = Function.GetDataToTable(@"
                SELECT TOP 5 tenhang, dongianhap, dongiaban,
                       CASE WHEN dongiaban <= 0 THEN 0 ELSE (dongiaban - dongianhap) / dongiaban * 100 END AS BienLoiNhuan
                FROM tbldmhang
                WHERE dongiaban > 0
                ORDER BY BienLoiNhuan ASC");
            foreach (DataRow row in margin.Rows)
            {
                double m = Double(row, "BienLoiNhuan");
                if (m < 8) medium.Add($"Biên lợi nhuận thấp ở {Text(row, "tenhang")}: {m:F1}%.");
            }

            DataTable inactiveCustomers = Function.GetDataToTable(@"
                SELECT COUNT(*) AS SoKhach
                FROM tblkhachhang kh
                WHERE NOT EXISTS (SELECT 1 FROM tbldondathang d WHERE d.makhach = kh.makhach)");
            int inactive = FirstInt(inactiveCustomers, "SoKhach");
            if (inactive >= 5) medium.Add($"Có {inactive} khách hàng chưa mua lần nào; cần chăm sóc chuyển đổi.");
            else if (inactive > 0) low.Add($"Có {inactive} khách hàng chưa mua, có thể đưa vào danh sách chăm sóc.");

            AppendRiskGroup(sb, "I. Rủi ro cao", high, "Chưa phát hiện rủi ro cao.");
            AppendRiskGroup(sb, "\nII. Rủi ro trung bình", medium, "Chưa phát hiện rủi ro trung bình đáng chú ý.");
            AppendRiskGroup(sb, "\nIII. Tín hiệu ổn định", low, "Chưa có tín hiệu ổn định nổi bật.");

            sb.AppendLine("\nIV. Kế hoạch kiểm soát");
            sb.AppendLine("   1. Kiểm tra tồn kho bán chạy mỗi ngày, ưu tiên mã dưới 3 chiếc.");
            sb.AppendLine("   2. Rà soát sản phẩm chưa bán trong 30-60 ngày để chạy khuyến mãi hoặc trưng bày lại.");
            sb.AppendLine("   3. Theo dõi biên lợi nhuận khi giảm giá để tránh bán nhiều nhưng không có lãi.");
            sb.AppendLine("   4. Giao KPI chăm sóc khách chưa mua cho nhân viên bán hàng.");

            return sb.ToString();
        }

        // ===== TƯ VẤN SẢN PHẨM =====
        public static string TuVanSanPham(string yeuCau)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("🎯 TƯ VẤN SẢN PHẨM THEO NHU CẦU KHÁCH HÀNG");
            sb.AppendLine("════════════════════════════════════════════════════");

            string normalized = Normalize(yeuCau);
            DataTable products = Function.GetDataToTable(@"
                SELECT h.mahang, h.tenhang, h.soluong, h.dongianhap, h.dongiaban, h.namsx,
                       l.tenloai, sx.tenhangsx, m.tenmau, p.tenphanh, dc.tendongco, nsx.tennuocsx,
                       ISNULL(SUM(CASE WHEN d.ngaymua >= DATEADD(day,-90,CONVERT(date,GETDATE())) THEN ct.soluong ELSE 0 END),0) AS Ban90Ngay
                FROM tbldmhang h
                LEFT JOIN tbltheloai l ON h.maloai = l.maloai
                LEFT JOIN tblhangsx sx ON h.mahangsx = sx.mahangsx
                LEFT JOIN tblmausac m ON h.mamau = m.mamau
                LEFT JOIN tblphanhxe p ON h.maphanh = p.maphanh
                LEFT JOIN tbldongco dc ON h.madongco = dc.madongco
                LEFT JOIN tblnuocsx nsx ON h.manuocsx = nsx.manuocsx
                LEFT JOIN tblchitietddh ct ON h.mahang = ct.mahang
                LEFT JOIN tbldondathang d ON ct.soddh = d.soddh
                WHERE h.soluong > 0
                GROUP BY h.mahang, h.tenhang, h.soluong, h.dongianhap, h.dongiaban, h.namsx,
                         l.tenloai, sx.tenhangsx, m.tenmau, p.tenphanh, dc.tendongco, nsx.tennuocsx");

            if (products.Rows.Count == 0)
            {
                sb.AppendLine("\nKhông có sản phẩm còn hàng để tư vấn. Vui lòng nhập hàng hoặc chạy dữ liệu mẫu chuyên nghiệp.");
                return sb.ToString();
            }

            List<ProductScore> scores = new List<ProductScore>();
            double budget = ExtractBudget(yeuCau);
            foreach (DataRow row in products.Rows)
            {
                ProductScore score = ScoreProduct(row, normalized, budget);
                scores.Add(score);
            }

            scores.Sort((a, b) => b.Score.CompareTo(a.Score));

            sb.AppendLine($"\nNhu cầu đã hiểu: {yeuCau}");
            if (budget > 0) sb.AppendLine($"Ngân sách nhận diện: khoảng/dưới {Money(budget)}");
            sb.AppendLine("\nI. Gợi ý phù hợp nhất");

            int take = Math.Min(5, scores.Count);
            for (int i = 0; i < take; i++)
            {
                ProductScore item = scores[i];
                DataRow row = item.Row;
                double margin = Double(row, "dongiaban") - Double(row, "dongianhap");
                double marginRate = Double(row, "dongiaban") <= 0 ? 0 : margin / Double(row, "dongiaban") * 100.0;
                sb.AppendLine($"   {i + 1}. {Text(row, "tenhang")} [{Text(row, "mahang")}]");
                sb.AppendLine($"      Giá bán: {Money(Double(row, "dongiaban"))} | Còn: {Int(row, "soluong"):N0} chiếc | Bán 90 ngày: {Int(row, "Ban90Ngay"):N0}");
                sb.AppendLine($"      Phân loại: {Text(row, "tenloai")} | Hãng: {Text(row, "tenhangsx")} | Màu: {Text(row, "tenmau")} | Phanh: {Text(row, "tenphanh")}");
                sb.AppendLine($"      Lý do đề xuất: {item.Reason}");
                sb.AppendLine($"      Gợi ý bán hàng: nhấn mạnh bảo hành, tồn kho còn {Int(row, "soluong"):N0} chiếc và lợi nhuận gộp khoảng {Money(margin)} ({marginRate:F1}%).");
            }

            sb.AppendLine("\nII. Cách tư vấn khách hàng");
            if (ContainsAny(normalized, "sinh vien", "re", "tiet kiem"))
                sb.AppendLine("   • Tập trung vào chi phí sở hữu, tiết kiệm xăng/điện, dễ bảo dưỡng và trả góp.");
            else if (ContainsAny(normalized, "cao cap", "sang", "sh", "abs"))
                sb.AppendLine("   • Tập trung vào thiết kế, thương hiệu, an toàn, trải nghiệm lái và bảo hành.");
            else if (ContainsAny(normalized, "nu", "thanh pho", "di lam"))
                sb.AppendLine("   • Tập trung vào cốp xe, chiều cao yên, trọng lượng, kiểu dáng và sự tiện dụng khi đi phố.");
            else
                sb.AppendLine("   • Hỏi thêm ngân sách, mục đích sử dụng, quãng đường di chuyển và sở thích thương hiệu để chốt mẫu phù hợp.");

            return sb.ToString();
        }

        // ===== TÌM KIẾM THÔNG MINH =====
        public static string TimKiemThongMinh(string tuKhoa)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"🔍 TÌM KIẾM THÔNG MINH: \"{tuKhoa}\"");
            sb.AppendLine("════════════════════════════════════════════════════");

            string kw = tuKhoa.Trim();
            bool found = false;

            DataTable products = Function.GetDataToTable(@"
                SELECT TOP 10 h.mahang, h.tenhang, l.tenloai, sx.tenhangsx, m.tenmau, h.soluong, h.dongiaban, h.dongianhap
                FROM tbldmhang h
                LEFT JOIN tbltheloai l ON h.maloai = l.maloai
                LEFT JOIN tblhangsx sx ON h.mahangsx = sx.mahangsx
                LEFT JOIN tblmausac m ON h.mamau = m.mamau
                WHERE h.tenhang LIKE @keyword OR l.tenloai LIKE @keyword OR sx.tenhangsx LIKE @keyword
                   OR m.tenmau LIKE @keyword OR h.mahang LIKE @keyword
                ORDER BY h.soluong DESC, h.dongiaban DESC", Function.Param("@keyword", "%" + kw + "%"));
            if (products.Rows.Count > 0)
            {
                found = true;
                sb.AppendLine("\nI. Sản phẩm phù hợp");
                foreach (DataRow row in products.Rows)
                {
                    double profit = Double(row, "dongiaban") - Double(row, "dongianhap");
                    sb.AppendLine($"   • [{Text(row, "mahang")}] {Text(row, "tenhang")} - {Text(row, "tenhangsx")} - {Text(row, "tenmau")}");
                    sb.AppendLine($"     Giá: {Money(Double(row, "dongiaban"))} | Tồn: {Int(row, "soluong"):N0} | Lợi nhuận/SP: {Money(profit)}");
                }
            }

            DataTable customers = Function.GetDataToTable(@"
                SELECT TOP 10 kh.makhach, kh.tenkhach, kh.sdt, kh.diachi,
                       ISNULL(COUNT(d.soddh),0) AS SoDon,
                       ISNULL(SUM(d.tongtien),0) AS TongChi
                FROM tblkhachhang kh
                LEFT JOIN tbldondathang d ON kh.makhach = d.makhach
                WHERE kh.makhach LIKE @keyword OR kh.tenkhach LIKE @keyword OR kh.sdt LIKE @keyword OR kh.diachi LIKE @keyword
                GROUP BY kh.makhach, kh.tenkhach, kh.sdt, kh.diachi
                ORDER BY TongChi DESC", Function.Param("@keyword", "%" + kw + "%"));
            if (customers.Rows.Count > 0)
            {
                found = true;
                sb.AppendLine("\nII. Khách hàng phù hợp");
                foreach (DataRow row in customers.Rows)
                    sb.AppendLine($"   • [{Text(row, "makhach")}] {Text(row, "tenkhach")} | SĐT: {Text(row, "sdt")} | {Text(row, "diachi")} | {Int(row, "SoDon"):N0} đơn - {Money(Double(row, "TongChi"))}");
            }

            DataTable employees = Function.GetDataToTable(@"
                SELECT TOP 10 nv.manv, nv.tennv, nv.sdt, cv.tencv,
                       ISNULL(COUNT(d.soddh),0) AS SoDon,
                       ISNULL(SUM(d.tongtien),0) AS DoanhThu
                FROM tblnhanvien nv
                LEFT JOIN tblcongviec cv ON nv.macv = cv.macv
                LEFT JOIN tbldondathang d ON nv.manv = d.manv
                WHERE nv.manv LIKE @keyword OR nv.tennv LIKE @keyword OR nv.sdt LIKE @keyword OR cv.tencv LIKE @keyword
                GROUP BY nv.manv, nv.tennv, nv.sdt, cv.tencv
                ORDER BY DoanhThu DESC", Function.Param("@keyword", "%" + kw + "%"));
            if (employees.Rows.Count > 0)
            {
                found = true;
                sb.AppendLine("\nIII. Nhân viên phù hợp");
                foreach (DataRow row in employees.Rows)
                    sb.AppendLine($"   • [{Text(row, "manv")}] {Text(row, "tennv")} | {Text(row, "tencv")} | SĐT: {Text(row, "sdt")} | Doanh thu phụ trách: {Money(Double(row, "DoanhThu"))}");
            }

            DataTable orders = Function.GetDataToTable(@"
                SELECT TOP 10 d.soddh, d.ngaymua, kh.tenkhach, nv.tennv, d.tongtien
                FROM tbldondathang d
                LEFT JOIN tblkhachhang kh ON d.makhach = kh.makhach
                LEFT JOIN tblnhanvien nv ON d.manv = nv.manv
                WHERE d.soddh LIKE @keyword OR kh.tenkhach LIKE @keyword OR nv.tennv LIKE @keyword
                ORDER BY d.ngaymua DESC", Function.Param("@keyword", "%" + kw + "%"));
            if (orders.Rows.Count > 0)
            {
                found = true;
                sb.AppendLine("\nIV. Đơn bán phù hợp");
                foreach (DataRow row in orders.Rows)
                    sb.AppendLine($"   • {Text(row, "soddh")} | {Date(row, "ngaymua")} | KH: {Text(row, "tenkhach")} | NV: {Text(row, "tennv")} | {Money(Double(row, "tongtien"))}");
            }

            if (!found)
            {
                sb.AppendLine("\nKhông tìm thấy dữ liệu phù hợp.");
                sb.AppendLine("Gợi ý tìm kiếm: tên xe, hãng xe, màu xe, mã xe, tên khách, số điện thoại, mã đơn, tên nhân viên.");
            }

            return sb.ToString();
        }

        // ===== PHÂN TÍCH KHÁCH HÀNG =====
        public static string PhanTichKhachHang()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("👥 PHÂN TÍCH KHÁCH HÀNG VÀ CHĂM SÓC");
            sb.AppendLine("════════════════════════════════════════════════════");

            DataTable summary = Function.GetDataToTable(@"
                SELECT COUNT(*) AS TongKhach,
                       SUM(CASE WHEN EXISTS (SELECT 1 FROM tbldondathang d WHERE d.makhach = kh.makhach) THEN 1 ELSE 0 END) AS CoMua,
                       SUM(CASE WHEN NOT EXISTS (SELECT 1 FROM tbldondathang d WHERE d.makhach = kh.makhach) THEN 1 ELSE 0 END) AS ChuaMua
                FROM tblkhachhang kh");
            sb.AppendLine("\nI. Tổng quan tệp khách hàng");
            sb.AppendLine($"   • Tổng khách hàng: {FirstInt(summary, "TongKhach"):N0}");
            sb.AppendLine($"   • Đã mua: {FirstInt(summary, "CoMua"):N0}");
            sb.AppendLine($"   • Chưa mua: {FirstInt(summary, "ChuaMua"):N0}");

            DataTable vip = Function.GetDataToTable(@"
                SELECT TOP 5 kh.tenkhach, kh.sdt, kh.diachi, COUNT(d.soddh) AS SoDon, SUM(d.tongtien) AS TongChi, MAX(d.ngaymua) AS LanMuaCuoi
                FROM tbldondathang d
                INNER JOIN tblkhachhang kh ON d.makhach = kh.makhach
                GROUP BY kh.tenkhach, kh.sdt, kh.diachi
                ORDER BY SUM(d.tongtien) DESC");
            if (vip.Rows.Count > 0)
            {
                sb.AppendLine("\nII. Khách hàng VIP / giá trị cao");
                int i = 1;
                foreach (DataRow row in vip.Rows)
                {
                    sb.AppendLine($"   {i}. {Text(row, "tenkhach")} | {Text(row, "sdt")} | {Text(row, "diachi")}");
                    sb.AppendLine($"      {Int(row, "SoDon"):N0} đơn | Tổng chi: {Money(Double(row, "TongChi"))} | Lần mua cuối: {Date(row, "LanMuaCuoi")}");
                    i++;
                }
            }

            DataTable noBuy = Function.GetDataToTable(@"
                SELECT TOP 10 kh.tenkhach, kh.sdt, kh.diachi
                FROM tblkhachhang kh
                WHERE NOT EXISTS (SELECT 1 FROM tbldondathang d WHERE d.makhach = kh.makhach)
                ORDER BY kh.tenkhach");
            if (noBuy.Rows.Count > 0)
            {
                sb.AppendLine("\nIII. Khách hàng cần chuyển đổi");
                foreach (DataRow row in noBuy.Rows)
                    sb.AppendLine($"   • {Text(row, "tenkhach")} | {Text(row, "sdt")} | {Text(row, "diachi")}");
            }

            sb.AppendLine("\nIV. Kế hoạch chăm sóc đề xuất");
            sb.AppendLine("   • Nhóm VIP: gọi cảm ơn, ưu tiên bảo dưỡng/khuyến mãi phụ kiện, mời giới thiệu khách mới.");
            sb.AppendLine("   • Nhóm đã mua 1 lần: nhắc lịch bảo dưỡng, giới thiệu phụ kiện, bảo hiểm, gia hạn dịch vụ.");
            sb.AppendLine("   • Nhóm chưa mua: gửi 3 mẫu xe phù hợp ngân sách, hẹn lái thử, ưu đãi cọc trong tuần.");
            sb.AppendLine("   • Lưu lịch sử chăm sóc để lần sau AI đánh giá tỷ lệ chuyển đổi tốt hơn.");

            return sb.ToString();
        }

        // ===== PHÂN TÍCH NHÂN VIÊN =====
        public static string PhanTichNhanVien()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("👔 PHÂN TÍCH HIỆU SUẤT NHÂN VIÊN");
            sb.AppendLine("════════════════════════════════════════════════════");

            DataTable dt = Function.GetDataToTable(@"
                SELECT nv.manv, nv.tennv, cv.tencv,
                       ISNULL(ban.SoDonBan,0) AS SoDonBan,
                       ISNULL(ban.DoanhThu,0) AS DoanhThu,
                       ISNULL(ban.SoXeBan,0) AS SoXeBan,
                       ISNULL(nhap.SoDonNhap,0) AS SoDonNhap,
                       ISNULL(nhap.TongNhap,0) AS TongNhap
                FROM tblnhanvien nv
                LEFT JOIN tblcongviec cv ON nv.macv = cv.macv
                LEFT JOIN (
                    SELECT d.manv, COUNT(DISTINCT d.soddh) AS SoDonBan, SUM(d.tongtien) AS DoanhThu, SUM(ct.soluong) AS SoXeBan
                    FROM tbldondathang d
                    LEFT JOIN tblchitietddh ct ON d.soddh = ct.soddh
                    GROUP BY d.manv
                ) ban ON nv.manv = ban.manv
                LEFT JOIN (
                    SELECT manv, COUNT(*) AS SoDonNhap, SUM(tongtien) AS TongNhap
                    FROM tblhoadonnhap
                    GROUP BY manv
                ) nhap ON nv.manv = nhap.manv
                ORDER BY ISNULL(ban.DoanhThu,0) DESC, ISNULL(nhap.TongNhap,0) DESC");

            if (dt.Rows.Count == 0)
            {
                sb.AppendLine("\nChưa có dữ liệu nhân viên.");
                return sb.ToString();
            }

            sb.AppendLine("\nI. Bảng hiệu suất");
            int rank = 1;
            foreach (DataRow row in dt.Rows)
            {
                double revenue = Double(row, "DoanhThu");
                int orders = Int(row, "SoDonBan");
                double aov = orders <= 0 ? 0 : revenue / orders;
                string badge = rank == 1 && revenue > 0 ? "🏆" : revenue > 0 ? "⭐" : "⚪";
                sb.AppendLine($"   {badge} {rank}. {Text(row, "tennv")} [{Text(row, "manv")}] - {Text(row, "tencv")}");
                sb.AppendLine($"      Bán: {orders:N0} đơn | {Int(row, "SoXeBan"):N0} xe | Doanh thu: {Money(revenue)} | TB/đơn: {Money(aov)}");
                sb.AppendLine($"      Nhập hàng: {Int(row, "SoDonNhap"):N0} phiếu | Giá trị nhập phụ trách: {Money(Double(row, "TongNhap"))}");
                rank++;
            }

            sb.AppendLine("\nII. Gợi ý quản trị");
            if (Double(dt.Rows[0], "DoanhThu") > 0)
                sb.AppendLine($"   • Nhân viên nổi bật: {Text(dt.Rows[0], "tennv")} với doanh thu {Money(Double(dt.Rows[0], "DoanhThu"))}.");
            sb.AppendLine("   • Nên giao KPI theo số đơn, doanh thu và tỷ lệ chăm sóc lại khách hàng thay vì chỉ theo doanh thu.");
            sb.AppendLine("   • Nhân viên chưa có đơn cần được phân khách tiềm năng hoặc hỗ trợ kỹ năng tư vấn sản phẩm.");

            return sb.ToString();
        }

        // ===== KỊCH BẢN MẪU =====
        public static string TaoKichBanMau()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("🧪 BỘ CÂU HỎI MẪU CHO TRỢ LÝ AI");
            sb.AppendLine("════════════════════════════════════════════════════");
            sb.AppendLine("\nBạn có thể bấm nhanh trong giao diện hoặc copy các câu sau:");
            int i = 1;
            foreach (string question in LayCauHoiMau())
            {
                sb.AppendLine($"   {i}. {question}");
                i++;
            }
            sb.AppendLine("\nGợi ý nhập dữ liệu mẫu:");
            sb.AppendLine("   • Chạy file Database/SampleData_Professional.sql để có thêm nhiều xe, khách hàng, đơn bán và hóa đơn nhập theo nhiều tháng.");
            sb.AppendLine("   • Sau khi có dữ liệu mẫu, các chức năng dự báo, cảnh báo rủi ro, gợi ý nhập hàng sẽ có kết quả chuyên nghiệp hơn.");
            return sb.ToString();
        }

        // ===== TRẢ LỜI CÂU HỎI TỰ NHIÊN =====
        public static string TraLoiCauHoi(string cauHoi)
        {
            if (string.IsNullOrWhiteSpace(cauHoi)) return TaoKichBanMau();

            string q = Normalize(cauHoi);

            if (ContainsAny(q, "vi du", "mau", "cau hoi mau", "demo", "huong dan", "goi y cau hoi"))
                return TaoKichBanMau();

            if (ContainsAny(q, "tong quan", "hom nay", "dieu hanh", "dashboard", "tinh hinh"))
                return TongQuanDieuHanh();

            if (ContainsAny(q, "du bao", "forecast", "30 ngay toi", "thang toi", "du doan"))
                return DuBaoDoanhThu();

            if (ContainsAny(q, "nhap them", "nen nhap", "dat hang", "bo sung", "muc ton an toan"))
                return GoiYNhapHang();

            if (ContainsAny(q, "rui ro", "canh bao", "bat thuong", "nguy co", "kiem soat"))
                return CanhBaoRuiRo();

            if (ContainsAny(q, "ton kho", "kho", "hang ton", "sap het", "het hang", "ton cao", "ban cham"))
                return PhanTichTonKho();

            if (ContainsAny(q, "doanh thu", "loi nhuan", "ban duoc", "kinh doanh", "thu nhap", "bien loi nhuan", "so sanh doanh thu"))
                return PhanTichDoanhThu();

            if (ContainsAny(q, "khach", "vip", "nguoi mua", "cham soc", "chuyen doi"))
                return PhanTichKhachHang();

            if (ContainsAny(q, "nhan vien", "hieu suat", "kpi", "nv"))
                return PhanTichNhanVien();

            if (ContainsAny(q, "tu van", "goi y", "nen mua", "de xuat", "xe re", "cao cap", "sinh vien", "nu", "tay ga", "xe dien", "duoi", "trieu"))
                return TuVanSanPham(cauHoi);

            if (ContainsAny(q, "ban chay", "hot", "pho bien", "top"))
                return TuVanSanPham("bán chạy");

            string searchResult = TimKiemThongMinh(cauHoi);
            if (searchResult.Contains("Không tìm thấy"))
            {
                return "🤖 Tôi chưa hiểu rõ câu hỏi. Bạn có thể hỏi theo các mẫu sau:\n\n" +
                       "   • Tổng quan tình hình kinh doanh hôm nay\n" +
                       "   • Dự báo doanh thu 30 ngày tới\n" +
                       "   • Xe nào nên nhập thêm trong tháng này?\n" +
                       "   • Phân tích rủi ro kinh doanh hiện tại\n" +
                       "   • Gợi ý xe dưới 40 triệu cho sinh viên\n" +
                       "   • Top khách hàng VIP cần chăm sóc\n" +
                       "   • Nhân viên nào đang bán tốt nhất?\n";
            }

            return searchResult;
        }

        private static void AppendTopProducts(StringBuilder sb, DataTable data, string title)
        {
            if (data.Rows.Count == 0)
            {
                sb.AppendLine(title);
                sb.AppendLine("   • Chưa có dữ liệu sản phẩm bán ra.");
                return;
            }

            sb.AppendLine(title);
            int i = 1;
            foreach (DataRow row in data.Rows)
            {
                string medal = i == 1 ? "🥇" : i == 2 ? "🥈" : i == 3 ? "🥉" : "•";
                sb.AppendLine($"   {medal} {Text(row, "tenhang")}: {Int(row, "SoLuongBan"):N0} xe | {Money(Double(row, "DoanhThu"))}");
                if (data.Columns.Contains("LaiGop")) sb.AppendLine($"      Lãi gộp ước tính: {Money(Double(row, "LaiGop"))}");
                i++;
            }
        }

        private static void AppendMonthlyRevenue(StringBuilder sb, DataTable monthly)
        {
            sb.AppendLine("\nII. Doanh thu theo tháng gần nhất");
            if (monthly.Rows.Count == 0)
            {
                sb.AppendLine("   • Chưa có dữ liệu doanh thu theo tháng.");
                return;
            }

            foreach (DataRow row in monthly.Rows)
            {
                sb.AppendLine($"   • {Text(row, "Thang")}: {Money(Double(row, "DoanhThu"))} | {Int(row, "SoDon"):N0} đơn");
            }
        }

        private static void AppendInventoryList(StringBuilder sb, DataTable data, string title, bool shortage)
        {
            sb.AppendLine(title);
            if (data.Rows.Count == 0)
            {
                sb.AppendLine(shortage ? "   • Không có mã hàng hết hoặc sắp hết." : "   • Chưa có mã tồn cao đáng chú ý.");
                return;
            }

            foreach (DataRow row in data.Rows)
            {
                string status = Int(row, "soluong") == 0 ? "Hết hàng" : Int(row, "soluong") <= 3 ? "Sắp hết" : Int(row, "Ban90Ngay") == 0 ? "Bán chậm" : "Theo dõi";
                sb.AppendLine($"   • [{Text(row, "mahang")}] {Text(row, "tenhang")} - {Text(row, "tenhangsx")}: tồn {Int(row, "soluong"):N0}, bán 90 ngày {Int(row, "Ban90Ngay"):N0}, giá {Money(Double(row, "dongiaban"))} ({status})");
            }
        }

        private static void AppendRiskGroup(StringBuilder sb, string title, List<string> risks, string emptyMessage)
        {
            sb.AppendLine(title);
            if (risks.Count == 0)
            {
                sb.AppendLine("   • " + emptyMessage);
                return;
            }

            foreach (string risk in risks)
                sb.AppendLine("   • " + risk);
        }

        private static List<ReorderRecommendation> GetReorderRecommendations()
        {
            DataTable data = Function.GetDataToTable(@"
                SELECT h.mahang, h.tenhang, h.soluong, h.dongianhap, h.dongiaban,
                       ISNULL(SUM(CASE WHEN d.ngaymua >= DATEADD(day,-90,CONVERT(date,GETDATE())) THEN ct.soluong ELSE 0 END),0) AS Ban90Ngay,
                       ISNULL(SUM(ct.soluong),0) AS BanTong,
                       MAX(d.ngaymua) AS LanBanCuoi
                FROM tbldmhang h
                LEFT JOIN tblchitietddh ct ON h.mahang = ct.mahang
                LEFT JOIN tbldondathang d ON ct.soddh = d.soddh
                GROUP BY h.mahang, h.tenhang, h.soluong, h.dongianhap, h.dongiaban");

            List<ReorderRecommendation> result = new List<ReorderRecommendation>();
            foreach (DataRow row in data.Rows)
            {
                int sold90 = Int(row, "Ban90Ngay");
                int stock = Int(row, "soluong");
                double dailyVelocity = sold90 / 90.0;
                int reorderPoint = Math.Max(2, (int)Math.Ceiling(dailyVelocity * 21));
                int suggested = 0;
                string reason = string.Empty;

                if (sold90 > 0 && stock <= reorderPoint)
                {
                    suggested = Math.Max(1, (int)Math.Ceiling(dailyVelocity * 45) - stock);
                    reason = "bán có tốc độ và tồn kho dưới mức an toàn";
                }
                else if (stock == 0)
                {
                    suggested = Math.Max(2, sold90 > 0 ? (int)Math.Ceiling(dailyVelocity * 30) : 2);
                    reason = sold90 > 0 ? "đã hết hàng nhưng có lịch sử bán" : "đã hết hàng, nên nhập kiểm tra nhu cầu";
                }
                else if (stock <= 2 && sold90 == 0)
                {
                    suggested = 2;
                    reason = "tồn rất thấp, nhập nhỏ để tránh mất cơ hội bán";
                }

                if (suggested <= 0) continue;

                double selling = Double(row, "dongiaban");
                double cost = Double(row, "dongianhap");
                double margin = selling - cost;
                double marginRate = selling <= 0 ? 0 : margin / selling * 100.0;

                result.Add(new ReorderRecommendation
                {
                    ProductCode = Text(row, "mahang"),
                    ProductName = Text(row, "tenhang"),
                    Stock = stock,
                    Sold90 = sold90,
                    DailyVelocity = dailyVelocity,
                    ReorderPoint = reorderPoint,
                    SuggestedQty = suggested,
                    Cost = cost,
                    MarginPerUnit = margin,
                    MarginRate = marginRate,
                    Reason = reason,
                    Score = sold90 * 10 + Math.Max(0, reorderPoint - stock) * 5 + marginRate
                });
            }

            result.Sort((a, b) => b.Score.CompareTo(a.Score));
            if (result.Count > 8) result = result.GetRange(0, 8);
            return result;
        }

        private static ProductScore ScoreProduct(DataRow row, string normalizedQuestion, double budget)
        {
            double score = 0;
            List<string> reasons = new List<string>();
            string name = Normalize(Text(row, "tenhang"));
            string category = Normalize(Text(row, "tenloai"));
            string brand = Normalize(Text(row, "tenhangsx"));
            string color = Normalize(Text(row, "tenmau"));
            string brake = Normalize(Text(row, "tenphanh"));
            string engine = Normalize(Text(row, "tendongco"));
            double price = Double(row, "dongiaban");
            int stock = Int(row, "soluong");
            int sold90 = Int(row, "Ban90Ngay");

            score += Math.Min(25, sold90 * 4);
            if (sold90 > 0) reasons.Add("đã có lịch sử bán gần đây");
            if (stock > 0) score += 10;
            if (stock <= 2) score -= 3;

            if (budget > 0)
            {
                if (price <= budget)
                {
                    score += 25;
                    reasons.Add("phù hợp ngân sách");
                }
                else
                {
                    score -= Math.Min(30, (price - budget) / 1000000.0);
                }
            }

            if (ContainsAny(normalizedQuestion, "re", "tiet kiem", "sinh vien", "duoi") && price <= 40000000)
            {
                score += 20;
                reasons.Add("giá dễ tiếp cận");
            }

            if (ContainsAny(normalizedQuestion, "cao cap", "sang", "premium", "abs") && (price >= 55000000 || brake.Contains("abs") || name.Contains("sh")))
            {
                score += 22;
                reasons.Add("định vị cao cấp/an toàn tốt");
            }

            if (ContainsAny(normalizedQuestion, "tay ga", "scooter", "nu", "thanh pho", "di lam") && (category.Contains("scooter") || name.Contains("vision") || name.Contains("grande") || name.Contains("liberty") || name.Contains("sh")))
            {
                score += 22;
                reasons.Add("phù hợp đi phố và tiện dụng");
            }

            if (ContainsAny(normalizedQuestion, "xe so", "underbone", "exciter", "winner") && (category.Contains("underbone") || name.Contains("exciter") || name.Contains("winner")))
            {
                score += 20;
                reasons.Add("phù hợp nhóm xe số/côn tay");
            }

            if (ContainsAny(normalizedQuestion, "dien", "electric") && (category.Contains("dien") || engine.Contains("dien") || brand.Contains("vinfast")))
            {
                score += 24;
                reasons.Add("phù hợp nhu cầu xe điện");
            }

            string[] brands = { "honda", "yamaha", "suzuki", "vinfast", "piaggio", "sym", "kawasaki" };
            foreach (string b in brands)
            {
                if (normalizedQuestion.Contains(b) && brand.Contains(b))
                {
                    score += 18;
                    reasons.Add("đúng thương hiệu khách hỏi");
                }
            }

            string[] colors = { "den", "trang", "do", "xanh", "bac", "xam", "vang", "nau" };
            foreach (string c in colors)
            {
                if (normalizedQuestion.Contains(c) && color.Contains(c))
                {
                    score += 8;
                    reasons.Add("đúng màu khách thích");
                }
            }

            if (ContainsAny(normalizedQuestion, "ban chay", "hot", "pho bien") && sold90 > 0)
            {
                score += sold90 * 6;
                reasons.Add("thuộc nhóm có sức bán tốt");
            }

            if (reasons.Count == 0) reasons.Add("còn hàng và có thể tư vấn thêm theo ngân sách/mục đích sử dụng");
            return new ProductScore { Row = row, Score = score, Reason = string.Join(", ", Unique(reasons)) };
        }

        private static double ExtractBudget(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            Match match = Regex.Match(Normalize(text), @"(\d{1,3})\s*(tr|trieu|m)");
            if (!match.Success) return 0;
            if (!double.TryParse(match.Groups[1].Value, NumberStyles.Number, CultureInfo.InvariantCulture, out double million)) return 0;
            if (million <= 0 || million > 500) return 0;
            return million * 1000000.0;
        }

        private static IEnumerable<string> Unique(IEnumerable<string> values)
        {
            HashSet<string> set = new HashSet<string>();
            foreach (string value in values)
            {
                if (set.Add(value)) yield return value;
            }
        }

        private static bool ContainsAny(string source, params string[] keywords)
        {
            if (string.IsNullOrWhiteSpace(source)) return false;
            foreach (string keyword in keywords)
            {
                if (source.Contains(keyword)) return true;
            }
            return false;
        }

        private static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            string formD = value.ToLowerInvariant().Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            foreach (char c in formD)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c == 'đ' ? 'd' : c);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private static string DanhGiaTangTruong(double growth)
        {
            if (growth >= 20) return "tăng mạnh";
            if (growth >= 5) return "tăng tốt";
            if (growth > -5) return "ổn định";
            if (growth > -20) return "giảm nhẹ";
            return "giảm mạnh";
        }

        private static double Clamp(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private static int FirstInt(DataTable table, string column)
        {
            if (table == null || table.Rows.Count == 0) return 0;
            return Int(table.Rows[0], column);
        }

        private static double FirstDouble(DataTable table, string column)
        {
            if (table == null || table.Rows.Count == 0) return 0;
            return Double(table.Rows[0], column);
        }

        private static int Int(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value) return 0;
            try { return Convert.ToInt32(row[column], ViCulture); }
            catch { return 0; }
        }

        private static double Double(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value) return 0;
            try { return Convert.ToDouble(row[column], ViCulture); }
            catch { return 0; }
        }

        private static string Text(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value) return string.Empty;
            return row[column].ToString().Trim();
        }

        private static string Date(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value) return "-";
            try { return Convert.ToDateTime(row[column], ViCulture).ToString("dd/MM/yyyy", ViCulture); }
            catch { return row[column].ToString(); }
        }

        private static string Money(double value)
        {
            return value.ToString("N0", ViCulture) + " VNĐ";
        }

        private sealed class ProductScore
        {
            public DataRow Row { get; set; }
            public double Score { get; set; }
            public string Reason { get; set; }
        }

        private sealed class ReorderRecommendation
        {
            public string ProductCode { get; set; }
            public string ProductName { get; set; }
            public int Stock { get; set; }
            public int Sold90 { get; set; }
            public double DailyVelocity { get; set; }
            public int ReorderPoint { get; set; }
            public int SuggestedQty { get; set; }
            public double Cost { get; set; }
            public double MarginPerUnit { get; set; }
            public double MarginRate { get; set; }
            public string Reason { get; set; }
            public double Score { get; set; }
        }
    }
}
