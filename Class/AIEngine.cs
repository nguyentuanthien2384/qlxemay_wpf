using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace QLXeMay.Class
{
    /// <summary>
    /// AI Engine offline - Phân tích dữ liệu cửa hàng xe máy thông minh
    /// Không cần API key, không cần internet
    /// </summary>
    public static class AIEngine
    {
        // ===== PHÂN TÍCH TỒN KHO =====
        public static string PhanTichTonKho()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("📊 BÁO CÁO PHÂN TÍCH TỒN KHO");
            sb.AppendLine("════════════════════════════════");

            DataTable dtTong = Function.GetDataToTable("SELECT COUNT(*) as SoSP, ISNULL(SUM(soluong),0) as TongTon, ISNULL(AVG(dongiaban),0) as GiaTB, ISNULL(SUM(soluong * dongiaban),0) as GiaTriTon FROM tbldmhang");
            if (dtTong.Rows.Count > 0 && Convert.ToInt32(dtTong.Rows[0]["SoSP"]) > 0)
            {
                sb.AppendLine($"\n📦 Tổng quan:");
                sb.AppendLine($"   • Số loại sản phẩm: {dtTong.Rows[0]["SoSP"]}");
                sb.AppendLine($"   • Tổng tồn kho: {dtTong.Rows[0]["TongTon"]} chiếc");
                sb.AppendLine($"   • Giá bán trung bình: {Convert.ToDouble(dtTong.Rows[0]["GiaTB"]):N0} VNĐ");
                sb.AppendLine($"   • Giá trị hàng tồn: {Convert.ToDouble(dtTong.Rows[0]["GiaTriTon"]):N0} VNĐ");
            }

            // SP tồn kho cao → cần giảm giá
            DataTable dtCao = Function.GetDataToTable("SELECT TOP 5 tenhang, soluong, dongiaban FROM tbldmhang WHERE soluong > 0 ORDER BY soluong DESC");
            if (dtCao.Rows.Count > 0)
            {
                sb.AppendLine($"\n⚠️ Sản phẩm tồn kho CAO (nên khuyến mãi/giảm giá):");
                foreach (DataRow r in dtCao.Rows)
                    sb.AppendLine($"   🔴 {r["tenhang"]}: {r["soluong"]} chiếc (giá {Convert.ToDouble(r["dongiaban"]):N0}đ)");
            }

            // SP tồn kho thấp → cần nhập thêm
            DataTable dtThap = Function.GetDataToTable("SELECT TOP 5 tenhang, soluong, dongiaban FROM tbldmhang WHERE soluong > 0 ORDER BY soluong ASC");
            if (dtThap.Rows.Count > 0)
            {
                sb.AppendLine($"\n⚡ Sản phẩm sắp hết hàng (nên NHẬP THÊM):");
                foreach (DataRow r in dtThap.Rows)
                    sb.AppendLine($"   🟡 {r["tenhang"]}: còn {r["soluong"]} chiếc");
            }

            // SP hết hàng
            DataTable dtHet = Function.GetDataToTable("SELECT tenhang FROM tbldmhang WHERE soluong = 0");
            if (dtHet.Rows.Count > 0)
            {
                sb.AppendLine($"\n🚫 Sản phẩm ĐÃ HẾT HÀNG ({dtHet.Rows.Count} SP):");
                foreach (DataRow r in dtHet.Rows)
                    sb.AppendLine($"   ❌ {r["tenhang"]}");
            }

            // Gợi ý
            sb.AppendLine("\n💡 GỢI Ý:");
            if (dtCao.Rows.Count > 0)
            {
                double soLuongCao = Convert.ToDouble(dtCao.Rows[0]["soluong"]);
                if (soLuongCao > 50)
                    sb.AppendLine("   → Giảm giá 10-20% cho SP tồn kho >50 chiếc để đẩy hàng");
            }
            if (dtHet.Rows.Count > 0)
                sb.AppendLine($"   → Nhập gấp {dtHet.Rows.Count} sản phẩm đã hết hàng");
            if (dtThap.Rows.Count > 0)
                sb.AppendLine("   → Đặt hàng bổ sung cho SP còn dưới 10 chiếc");

            return sb.ToString();
        }

        // ===== PHÂN TÍCH DOANH THU =====
        public static string PhanTichDoanhThu()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("💰 BÁO CÁO PHÂN TÍCH DOANH THU");
            sb.AppendLine("════════════════════════════════");

            // Tổng doanh thu
            DataTable dtTong = Function.GetDataToTable("SELECT COUNT(*) as SoHD, ISNULL(SUM(tongtien),0) as TongDT FROM tbldondathang");
            DataTable dtNhap = Function.GetDataToTable("SELECT COUNT(*) as SoHDN, ISNULL(SUM(tongtien),0) as TongNhap FROM tblhoadonnhap");

            double tongBan = 0, tongNhap = 0;
            int soHDBan = 0, soHDNhap = 0;
            if (dtTong.Rows.Count > 0) { tongBan = Convert.ToDouble(dtTong.Rows[0]["TongDT"]); soHDBan = Convert.ToInt32(dtTong.Rows[0]["SoHD"]); }
            if (dtNhap.Rows.Count > 0) { tongNhap = Convert.ToDouble(dtNhap.Rows[0]["TongNhap"]); soHDNhap = Convert.ToInt32(dtNhap.Rows[0]["SoHDN"]); }
            double loiNhuan = tongBan - tongNhap;

            sb.AppendLine($"\n📈 Tổng quan:");
            sb.AppendLine($"   • Tổng doanh thu bán: {tongBan:N0} VNĐ ({soHDBan} đơn)");
            sb.AppendLine($"   • Tổng chi phí nhập: {tongNhap:N0} VNĐ ({soHDNhap} đơn)");
            sb.AppendLine($"   • Lợi nhuận ước tính: {loiNhuan:N0} VNĐ");
            if (tongBan > 0)
                sb.AppendLine($"   • Biên lợi nhuận: {(loiNhuan / tongBan * 100):F1}%");
            if (soHDBan > 0)
                sb.AppendLine($"   • Giá trị trung bình/đơn: {(tongBan / soHDBan):N0} VNĐ");

            // Top SP bán chạy
            DataTable dtTop = Function.GetDataToTable("SELECT TOP 5 h.tenhang, SUM(ct.soluong) as slban, SUM(ct.thanhtien) as dthu FROM tblchitietddh ct INNER JOIN tbldmhang h ON ct.mahang=h.mahang GROUP BY h.tenhang ORDER BY SUM(ct.soluong) DESC");
            if (dtTop.Rows.Count > 0)
            {
                sb.AppendLine($"\n🏆 Top 5 sản phẩm BÁN CHẠY nhất:");
                int i = 1;
                foreach (DataRow r in dtTop.Rows)
                {
                    string medal = i <= 3 ? new[] { "🥇", "🥈", "🥉" }[i - 1] : "  ";
                    sb.AppendLine($"   {medal} #{i} {r["tenhang"]}: {r["slban"]} chiếc ({Convert.ToDouble(r["dthu"]):N0}đ)");
                    i++;
                }
            }

            // SP chưa bán được
            DataTable dtChuaBan = Function.GetDataToTable("SELECT tenhang, soluong, dongiaban FROM tbldmhang WHERE mahang NOT IN (SELECT DISTINCT mahang FROM tblchitietddh) AND soluong > 0");
            if (dtChuaBan.Rows.Count > 0)
            {
                sb.AppendLine($"\n😟 Sản phẩm CHƯA BÁN ĐƯỢC ({dtChuaBan.Rows.Count} SP):");
                foreach (DataRow r in dtChuaBan.Rows)
                    sb.AppendLine($"   ⚪ {r["tenhang"]}: tồn {r["soluong"]} chiếc");
            }

            // Gợi ý
            sb.AppendLine("\n💡 GỢI Ý KINH DOANH:");
            if (loiNhuan < 0) sb.AppendLine("   🔴 CẢNH BÁO: Đang LỖ! Cần tăng giá hoặc giảm chi phí nhập.");
            else if (tongBan > 0 && loiNhuan / tongBan < 0.1) sb.AppendLine("   🟡 Biên lợi nhuận thấp (<10%). Nên tìm NCC giá tốt hơn.");
            else sb.AppendLine("   🟢 Lợi nhuận ổn định. Tiếp tục duy trì.");
            if (dtChuaBan.Rows.Count > 3) sb.AppendLine($"   → Khuyến mãi {dtChuaBan.Rows.Count} SP chưa bán để thu hồi vốn.");
            if (dtTop.Rows.Count > 0) sb.AppendLine($"   → Tập trung nhập thêm SP bán chạy nhất: {dtTop.Rows[0]["tenhang"]}");

            return sb.ToString();
        }

        // ===== TƯ VẤN SẢN PHẨM =====
        public static string TuVanSanPham(string yeuCau)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("🎯 TƯ VẤN SẢN PHẨM");
            sb.AppendLine("════════════════════════════════");

            string yc = yeuCau.ToLower();
            string sqlWhere = " WHERE soluong > 0";

            // Phân tích từ khóa
            if (yc.Contains("rẻ") || yc.Contains("giá thấp") || yc.Contains("tiết kiệm"))
                sqlWhere += " ORDER BY dongiaban ASC";
            else if (yc.Contains("đắt") || yc.Contains("cao cấp") || yc.Contains("sang"))
                sqlWhere += " ORDER BY dongiaban DESC";
            else if (yc.Contains("bán chạy") || yc.Contains("phổ biến") || yc.Contains("hot"))
                sqlWhere = " WHERE mahang IN (SELECT TOP 10 mahang FROM tblchitietddh GROUP BY mahang ORDER BY SUM(soluong) DESC)";
            else if (yc.Contains("mới") || yc.Contains("nhập gần đây"))
                sqlWhere += " AND mahang IN (SELECT DISTINCT mahang FROM tblchitiethdn ct INNER JOIN tblhoadonnhap h ON ct.sohdn=h.sohdn WHERE h.ngaynhap >= DATEADD(month,-1,GETDATE()))";
            else
                sqlWhere += " ORDER BY soluong DESC";

            string sql = "SELECT TOP 5 h.tenhang, h.soluong, h.dongiaban, l.tenloai, hsx.tenhangsx " +
                         "FROM tbldmhang h LEFT JOIN tbltheloai l ON h.maloai=l.maloai " +
                         "LEFT JOIN tblhangsx hsx ON h.mahangsx=hsx.mahangsx " + sqlWhere;
            DataTable dt = Function.GetDataToTable(sql);

            if (dt.Rows.Count > 0)
            {
                sb.AppendLine($"\n🏍️ Gợi ý {dt.Rows.Count} sản phẩm phù hợp:\n");
                int i = 1;
                foreach (DataRow r in dt.Rows)
                {
                    sb.AppendLine($"   {i}. {r["tenhang"]}");
                    sb.AppendLine($"      Loại: {r["tenloai"]} | Hãng: {r["tenhangsx"]}");
                    sb.AppendLine($"      Giá: {Convert.ToDouble(r["dongiaban"]):N0} VNĐ | Còn: {r["soluong"]} chiếc");
                    sb.AppendLine();
                    i++;
                }
            }
            else
                sb.AppendLine("\n❌ Không tìm thấy sản phẩm phù hợp.");

            return sb.ToString();
        }

        // ===== TÌM KIẾM THÔNG MINH =====
        public static string TimKiemThongMinh(string tuKhoa)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"🔍 KẾT QUẢ TÌM KIẾM: \"{tuKhoa}\"");
            sb.AppendLine("════════════════════════════════");

            string kw = tuKhoa.Trim();
            string sql = @"SELECT h.mahang, h.tenhang, l.tenloai, hsx.tenhangsx, m.tenmau, h.soluong, h.dongiaban, h.dongianhap
                FROM tbldmhang h 
                LEFT JOIN tbltheloai l ON h.maloai=l.maloai 
                LEFT JOIN tblhangsx hsx ON h.mahangsx=hsx.mahangsx 
                LEFT JOIN tblmausac m ON h.mamau=m.mamau
                WHERE h.tenhang LIKE @keyword OR l.tenloai LIKE @keyword
                   OR hsx.tenhangsx LIKE @keyword OR m.tenmau LIKE @keyword
                   OR h.mahang LIKE @keyword
                ORDER BY h.soluong DESC";

            DataTable dt = Function.GetDataToTable(sql, Function.Param("@keyword", "%" + kw + "%"));
            if (dt.Rows.Count > 0)
            {
                sb.AppendLine($"\n✅ Tìm thấy {dt.Rows.Count} sản phẩm:\n");
                foreach (DataRow r in dt.Rows)
                {
                    string trangThai = Convert.ToInt32(r["soluong"]) > 0 ? "🟢 Còn hàng" : "🔴 Hết hàng";
                    sb.AppendLine($"   [{r["mahang"]}] {r["tenhang"]}");
                    sb.AppendLine($"   {r["tenloai"]} | {r["tenhangsx"]} | Màu: {r["tenmau"]}");
                    sb.AppendLine($"   Giá: {Convert.ToDouble(r["dongiaban"]):N0}đ | Tồn: {r["soluong"]} | {trangThai}");
                    double loiNhuan = Convert.ToDouble(r["dongiaban"]) - Convert.ToDouble(r["dongianhap"]);
                    sb.AppendLine($"   Lợi nhuận/SP: {loiNhuan:N0}đ ({(loiNhuan / Math.Max(Convert.ToDouble(r["dongiaban"]), 1) * 100):F0}%)");
                    sb.AppendLine();
                }
            }
            else
            {
                sb.AppendLine($"\n❌ Không tìm thấy sản phẩm nào với từ khóa \"{tuKhoa}\".");
                sb.AppendLine("\n💡 Thử tìm với: tên xe, hãng (Honda, Yamaha...), loại, hoặc màu sắc.");
            }
            return sb.ToString();
        }

        // ===== PHÂN TÍCH KHÁCH HÀNG =====
        public static string PhanTichKhachHang()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("👥 PHÂN TÍCH KHÁCH HÀNG");
            sb.AppendLine("════════════════════════════════");

            string soKH = Function.GetFieldValues("SELECT COUNT(*) FROM tblkhachhang");
            sb.AppendLine($"\n📊 Tổng khách hàng: {soKH}");

            // Top khách hàng mua nhiều nhất
            DataTable dtTopKH = Function.GetDataToTable("SELECT TOP 5 kh.tenkhach, kh.sdt, COUNT(*) as SoDon, SUM(d.tongtien) as TongChi FROM tbldondathang d INNER JOIN tblkhachhang kh ON d.makhach=kh.makhach GROUP BY kh.tenkhach, kh.sdt ORDER BY SUM(d.tongtien) DESC");
            if (dtTopKH.Rows.Count > 0)
            {
                sb.AppendLine($"\n🏆 Top khách hàng VIP (mua nhiều nhất):");
                int i = 1;
                foreach (DataRow r in dtTopKH.Rows)
                {
                    sb.AppendLine($"   #{i} {r["tenkhach"]} (SĐT: {r["sdt"]})");
                    sb.AppendLine($"      {r["SoDon"]} đơn | Tổng chi: {Convert.ToDouble(r["TongChi"]):N0}đ");
                    i++;
                }
            }

            // Khách hàng chưa mua
            DataTable dtChuaMua = Function.GetDataToTable("SELECT tenkhach, sdt FROM tblkhachhang WHERE makhach NOT IN (SELECT DISTINCT makhach FROM tbldondathang)");
            if (dtChuaMua.Rows.Count > 0)
            {
                sb.AppendLine($"\n😐 Khách hàng CHƯA MUA lần nào ({dtChuaMua.Rows.Count} người):");
                foreach (DataRow r in dtChuaMua.Rows)
                    sb.AppendLine($"   ⚪ {r["tenkhach"]} - SĐT: {r["sdt"]}");
                sb.AppendLine($"\n💡 GỢI Ý: Gọi điện/SMS chăm sóc {dtChuaMua.Rows.Count} khách hàng này.");
            }

            return sb.ToString();
        }

        // ===== PHÂN TÍCH NHÂN VIÊN =====
        public static string PhanTichNhanVien()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("👔 PHÂN TÍCH HIỆU SUẤT NHÂN VIÊN");
            sb.AppendLine("════════════════════════════════");

            DataTable dt = Function.GetDataToTable(@"SELECT nv.manv, nv.tennv, 
                ISNULL(ban.SoDon,0) as SoDonBan, ISNULL(ban.TongBan,0) as DoanhThu,
                ISNULL(nhap.SoHDN,0) as SoDonNhap
                FROM tblnhanvien nv
                LEFT JOIN (SELECT manv, COUNT(*) as SoDon, SUM(tongtien) as TongBan FROM tbldondathang GROUP BY manv) ban ON nv.manv=ban.manv
                LEFT JOIN (SELECT manv, COUNT(*) as SoHDN FROM tblhoadonnhap GROUP BY manv) nhap ON nv.manv=nhap.manv
                ORDER BY ISNULL(ban.TongBan,0) DESC");

            if (dt.Rows.Count > 0)
            {
                sb.AppendLine($"\n📊 Hiệu suất {dt.Rows.Count} nhân viên:\n");
                foreach (DataRow r in dt.Rows)
                {
                    double dt_val = Convert.ToDouble(r["DoanhThu"]);
                    string rank = dt_val > 0 ? "⭐" : "⚪";
                    sb.AppendLine($"   {rank} {r["tennv"]} ({r["manv"]})");
                    sb.AppendLine($"      Bán: {r["SoDonBan"]} đơn ({dt_val:N0}đ) | Nhập: {r["SoDonNhap"]} đơn");
                }

                // Tìm NV doanh thu cao nhất
                if (Convert.ToDouble(dt.Rows[0]["DoanhThu"]) > 0)
                    sb.AppendLine($"\n🏆 Nhân viên xuất sắc nhất: {dt.Rows[0]["tennv"]} ({Convert.ToDouble(dt.Rows[0]["DoanhThu"]):N0}đ doanh thu)");
            }

            return sb.ToString();
        }

        // ===== TRẢ LỜI CÂU HỎI TỰ NHIÊN =====
        public static string TraLoiCauHoi(string cauHoi)
        {
            string q = cauHoi.ToLower().Trim();

            // Tồn kho
            if (q.Contains("tồn kho") || q.Contains("kho") || q.Contains("hàng tồn") || q.Contains("còn bao nhiêu"))
                return PhanTichTonKho();

            // Doanh thu
            if (q.Contains("doanh thu") || q.Contains("lợi nhuận") || q.Contains("bán được") || q.Contains("kinh doanh") || q.Contains("thu nhập"))
                return PhanTichDoanhThu();

            // Khách hàng
            if (q.Contains("khách") || q.Contains("vip") || q.Contains("người mua"))
                return PhanTichKhachHang();

            // Nhân viên
            if (q.Contains("nhân viên") || q.Contains("hiệu suất") || q.Contains("nv"))
                return PhanTichNhanVien();

            // Tư vấn
            if (q.Contains("tư vấn") || q.Contains("gợi ý") || q.Contains("nên mua") || q.Contains("đề xuất"))
                return TuVanSanPham(cauHoi);

            // Bán chạy
            if (q.Contains("bán chạy") || q.Contains("hot") || q.Contains("phổ biến") || q.Contains("top"))
                return TuVanSanPham("bán chạy");

            // Giá rẻ
            if (q.Contains("rẻ") || q.Contains("giá thấp") || q.Contains("tiết kiệm") || q.Contains("sinh viên"))
                return TuVanSanPham("rẻ tiết kiệm");

            // Cao cấp
            if (q.Contains("đắt") || q.Contains("cao cấp") || q.Contains("sang") || q.Contains("premium"))
                return TuVanSanPham("cao cấp sang");

            // Hết hàng
            if (q.Contains("hết hàng") || q.Contains("hết") || q.Contains("nhập thêm"))
            {
                DataTable dt = Function.GetDataToTable("SELECT tenhang FROM tbldmhang WHERE soluong = 0");
                if (dt.Rows.Count > 0)
                {
                    StringBuilder sb = new StringBuilder($"🚫 Có {dt.Rows.Count} sản phẩm hết hàng:\n");
                    foreach (DataRow r in dt.Rows) sb.AppendLine($"   ❌ {r["tenhang"]}");
                    sb.AppendLine($"\n💡 Nên nhập bổ sung ngay {dt.Rows.Count} sản phẩm này!");
                    return sb.ToString();
                }
                return "✅ Hiện tại không có sản phẩm nào hết hàng!";
            }

            // Mặc định: tìm kiếm sản phẩm
            string kq = TimKiemThongMinh(cauHoi);
            if (kq.Contains("Không tìm thấy"))
            {
                return "🤖 Tôi chưa hiểu rõ câu hỏi. Bạn có thể hỏi:\n\n" +
                       "📊 \"phân tích tồn kho\" - Báo cáo tồn kho chi tiết\n" +
                       "💰 \"doanh thu\" - Phân tích doanh thu, lợi nhuận\n" +
                       "👥 \"khách hàng\" - Phân tích khách hàng VIP\n" +
                       "👔 \"nhân viên\" - Hiệu suất từng nhân viên\n" +
                       "🎯 \"tư vấn xe rẻ\" - Gợi ý sản phẩm\n" +
                       "🔥 \"bán chạy\" - Top SP bán chạy nhất\n" +
                       "🔍 Hoặc gõ tên/hãng xe để tìm kiếm";
            }
            return kq;
        }
    }
}
