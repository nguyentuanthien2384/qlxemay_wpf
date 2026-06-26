-- =============================================================
-- QLXeMay WPF - PROFESSIONAL SAMPLE DATA FOR AI ASSISTANT
-- Mục đích: bổ sung dữ liệu mẫu đa dạng để dashboard, báo cáo và Trợ lý AI
-- có đủ dữ liệu phân tích như dự án thực tế.
-- Chạy sau file CreateDatabase_Complete.sql.
-- Script dùng ngày tương đối theo GETDATE() để dữ liệu luôn còn mới.
-- =============================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

USE btl;
GO

-- =============================================================
-- 0. ĐẢM BẢO MASTER DATA TỒN TẠI (CHO DB CŨ THIẾU MÃ)
-- =============================================================
MERGE dbo.tbltheloai AS target
USING (VALUES
    ('TL01', N'Underbone'), ('TL02', N'Scooter'), ('TL03', N'Sportbike'),
    ('TL04', N'Nakedbike'), ('TL05', N'Cruiser'), ('TL06', N'Xe điện')
) AS source(maloai, tenloai)
ON target.maloai = source.maloai
WHEN MATCHED THEN UPDATE SET tenloai = source.tenloai
WHEN NOT MATCHED THEN INSERT(maloai, tenloai) VALUES(source.maloai, source.tenloai);
GO

MERGE dbo.tblmausac AS target
USING (VALUES
    ('MS01', N'Đen'), ('MS02', N'Trắng'), ('MS03', N'Đỏ'), ('MS04', N'Xanh'),
    ('MS05', N'Bạc'), ('MS06', N'Xám'), ('MS07', N'Vàng'), ('MS08', N'Nâu')
) AS source(mamau, tenmau)
ON target.mamau = source.mamau
WHEN MATCHED THEN UPDATE SET tenmau = source.tenmau
WHEN NOT MATCHED THEN INSERT(mamau, tenmau) VALUES(source.mamau, source.tenmau);
GO

MERGE dbo.tblhangsx AS target
USING (VALUES
    ('HSX01', N'Honda'), ('HSX02', N'Yamaha'), ('HSX03', N'Suzuki'),
    ('HSX04', N'VinFast'), ('HSX05', N'Piaggio'), ('HSX06', N'SYM'), ('HSX07', N'Kawasaki')
) AS source(mahangsx, tenhangsx)
ON target.mahangsx = source.mahangsx
WHEN MATCHED THEN UPDATE SET tenhangsx = source.tenhangsx
WHEN NOT MATCHED THEN INSERT(mahangsx, tenhangsx) VALUES(source.mahangsx, source.tenhangsx);
GO

MERGE dbo.tbldongco AS target
USING (VALUES
    ('DC01', N'4 thì'), ('DC02', N'Phun xăng điện tử'), ('DC03', N'Động cơ điện'),
    ('DC04', N'Blue Core'), ('DC05', N'eSP+'), ('DC06', N'DOHC')
) AS source(madongco, tendongco)
ON target.madongco = source.madongco
WHEN MATCHED THEN UPDATE SET tendongco = source.tendongco
WHEN NOT MATCHED THEN INSERT(madongco, tendongco) VALUES(source.madongco, source.tendongco);
GO

MERGE dbo.tblnuocsx AS target
USING (VALUES
    ('NSX01', N'Việt Nam'), ('NSX02', N'Nhật Bản'), ('NSX03', N'Ý'),
    ('NSX04', N'Trung Quốc'), ('NSX05', N'Thái Lan'), ('NSX06', N'Indonesia')
) AS source(manuocsx, tennuocsx)
ON target.manuocsx = source.manuocsx
WHEN MATCHED THEN UPDATE SET tennuocsx = source.tennuocsx
WHEN NOT MATCHED THEN INSERT(manuocsx, tennuocsx) VALUES(source.manuocsx, source.tennuocsx);
GO

MERGE dbo.tblphanhxe AS target
USING (VALUES
    ('PH01', N'Phanh đĩa'), ('PH02', N'Phanh tang trống'), ('PH03', N'Phanh ABS'), ('PH04', N'CBS')
) AS source(maphanh, tenphanh)
ON target.maphanh = source.maphanh
WHEN MATCHED THEN UPDATE SET tenphanh = source.tenphanh
WHEN NOT MATCHED THEN INSERT(maphanh, tenphanh) VALUES(source.maphanh, source.tenphanh);
GO

MERGE dbo.tbltinhtrang AS target
USING (VALUES
    ('TT01', N'Mới'), ('TT02', N'Đã qua sử dụng'), ('TT03', N'Đang bảo hành'), ('TT04', N'Tạm ngưng bán')
) AS source(matt, tentt)
ON target.matt = source.matt
WHEN MATCHED THEN UPDATE SET tentt = source.tentt
WHEN NOT MATCHED THEN INSERT(matt, tentt) VALUES(source.matt, source.tentt);
GO

MERGE dbo.tblcongviec AS target
USING (VALUES
    ('CV01', N'Quản lý', 15000000),
    ('CV02', N'Nhân viên bán hàng', 8000000),
    ('CV03', N'Kế toán', 10000000),
    ('CV04', N'Thủ kho', 7000000),
    ('CV05', N'Kỹ thuật viên', 9000000)
) AS source(macv, tencv, luongthang)
ON target.macv = source.macv
WHEN MATCHED THEN UPDATE SET tencv = source.tencv, luongthang = source.luongthang
WHEN NOT MATCHED THEN INSERT(macv, tencv, luongthang) VALUES(source.macv, source.tencv, source.luongthang);
GO

MERGE dbo.tblnhanvien AS target
USING (VALUES
    ('NV01', N'Nguyễn Văn An', N'Nam', CONVERT(date,'1995-05-15'), '0912345678', N'Hà Nội', 'CV01'),
    ('NV02', N'Trần Thị Bình', N'Nữ', CONVERT(date,'1998-08-20'), '0987654321', N'Hà Nội', 'CV02'),
    ('NV03', N'Lê Minh Khôi', N'Nam', CONVERT(date,'1997-02-10'), '0901122334', N'Bắc Ninh', 'CV04'),
    ('NV04', N'Phạm Thu Hà', N'Nữ', CONVERT(date,'1996-11-03'), '0933445566', N'Hà Nội', 'CV03'),
    ('NV05', N'Đỗ Quốc Huy', N'Nam', CONVERT(date,'1999-07-22'), '0977889900', N'Hải Phòng', 'CV05')
) AS source(manv, tennv, gioitinh, ngaysinh, sdt, diachi, macv)
ON target.manv = source.manv
WHEN MATCHED THEN UPDATE SET tennv = source.tennv, gioitinh = source.gioitinh, ngaysinh = source.ngaysinh, sdt = source.sdt, diachi = source.diachi, macv = source.macv
WHEN NOT MATCHED THEN INSERT(manv, tennv, gioitinh, ngaysinh, sdt, diachi, macv) VALUES(source.manv, source.tennv, source.gioitinh, source.ngaysinh, source.sdt, source.diachi, source.macv);
GO

MERGE dbo.tblnhacungcap AS target
USING (VALUES
    ('NCC01', N'Honda Việt Nam', N'Vĩnh Phúc', '0241234567'),
    ('NCC02', N'Yamaha Motor Việt Nam', N'Hà Nội', '0249876543'),
    ('NCC03', N'Suzuki Việt Nam', N'Đồng Nai', '0281111222'),
    ('NCC04', N'VinFast Trading', N'Hải Phòng', '0225888999'),
    ('NCC05', N'Piaggio Việt Nam', N'Vĩnh Phúc', '0211999888')
) AS source(mancc, tenncc, diachi, sdt)
ON target.mancc = source.mancc
WHEN MATCHED THEN UPDATE SET tenncc = source.tenncc, diachi = source.diachi, sdt = source.sdt
WHEN NOT MATCHED THEN INSERT(mancc, tenncc, diachi, sdt) VALUES(source.mancc, source.tenncc, source.diachi, source.sdt);
GO

-- =============================================================
-- 1. BỔ SUNG KHÁCH HÀNG MẪU
-- =============================================================
MERGE dbo.tblkhachhang AS target
USING (VALUES
    ('KH06', N'Bùi Ngọc Anh', N'Quận 1, TP.HCM', '0906001001'),
    ('KH07', N'Nguyễn Hải Nam', N'Quận Bình Thạnh, TP.HCM', '0906001002'),
    ('KH08', N'Trần Gia Hân', N'Thủ Đức, TP.HCM', '0906001003'),
    ('KH09', N'Lê Quốc Bảo', N'Quận 7, TP.HCM', '0906001004'),
    ('KH10', N'Phạm Minh Châu', N'Gò Vấp, TP.HCM', '0906001005'),
    ('KH11', N'Hoàng Tuấn Kiệt', N'Hà Đông, Hà Nội', '0906001006'),
    ('KH12', N'Đặng Thảo Vy', N'Cầu Giấy, Hà Nội', '0906001007'),
    ('KH13', N'Võ Đức Anh', N'Đống Đa, Hà Nội', '0906001008'),
    ('KH14', N'Mai Phương Linh', N'Hải Châu, Đà Nẵng', '0906001009'),
    ('KH15', N'Đỗ Thanh Phong', N'Ninh Kiều, Cần Thơ', '0906001010'),
    ('KH16', N'Ngô Nhật Minh', N'Biên Hòa, Đồng Nai', '0906001011'),
    ('KH17', N'Trịnh Quỳnh Như', N'Vũng Tàu', '0906001012'),
    ('KH18', N'Cao Gia Bảo', N'Thuận An, Bình Dương', '0906001013'),
    ('KH19', N'Lâm Khánh Vy', N'Hạ Long, Quảng Ninh', '0906001014'),
    ('KH20', N'Phan Hoài Nam', N'Thanh Xuân, Hà Nội', '0906001015')
) AS source(makhach, tenkhach, diachi, sdt)
ON target.makhach = source.makhach
WHEN MATCHED THEN UPDATE SET tenkhach = source.tenkhach, diachi = source.diachi, sdt = source.sdt
WHEN NOT MATCHED THEN INSERT(makhach, tenkhach, diachi, sdt) VALUES(source.makhach, source.tenkhach, source.diachi, source.sdt);
GO

-- =============================================================
-- 2. BỔ SUNG SẢN PHẨM MẪU ĐA PHÂN KHÚC
-- =============================================================
MERGE dbo.tbldmhang AS target
USING (VALUES
    ('MH01', N'Honda Vision 2024', 'TL02', 'HSX01', 'MS01', 2024, 'PH04', 'DC05', 'NSX01', 'TT01', 5, N'', 24, 8, 29000000, 34000000),
    ('MH02', N'Yamaha Exciter 155', 'TL01', 'HSX02', 'MS03', 2024, 'PH01', 'DC04', 'NSX01', 'TT01', 4, N'', 24, 5, 44000000, 50500000),
    ('MH03', N'Honda SH Mode', 'TL02', 'HSX01', 'MS02', 2024, 'PH03', 'DC05', 'NSX01', 'TT01', 7, N'', 36, 3, 57000000, 69000000),
    ('MH04', N'Yamaha Grande', 'TL02', 'HSX02', 'MS05', 2024, 'PH04', 'DC04', 'NSX01', 'TT01', 4, N'', 24, 4, 39000000, 46000000),
    ('MH05', N'Suzuki Raider R150', 'TL03', 'HSX03', 'MS04', 2024, 'PH01', 'DC06', 'NSX06', 'TT01', 4, N'', 24, 3, 43000000, 51000000),
    ('MH06', N'VinFast Feliz S', 'TL06', 'HSX04', 'MS02', 2024, 'PH01', 'DC03', 'NSX01', 'TT01', 0, N'', 36, 6, 24000000, 32000000),
    ('MH07', N'Piaggio Liberty 125', 'TL02', 'HSX05', 'MS08', 2024, 'PH03', 'DC02', 'NSX01', 'TT01', 6, N'', 24, 3, 50000000, 61000000),
    ('MH08', N'Honda Winner X', 'TL01', 'HSX01', 'MS06', 2024, 'PH01', 'DC05', 'NSX01', 'TT01', 4, N'', 24, 4, 39000000, 48000000),
    ('MH09', N'Honda Air Blade 160 ABS', 'TL02', 'HSX01', 'MS06', 2024, 'PH03', 'DC05', 'NSX01', 'TT01', 5, N'', 36, 7, 47000000, 56000000),
    ('MH10', N'Honda Lead 125', 'TL02', 'HSX01', 'MS02', 2024, 'PH04', 'DC05', 'NSX01', 'TT01', 6, N'', 36, 6, 34000000, 42000000),
    ('MH11', N'Yamaha Janus 125', 'TL02', 'HSX02', 'MS04', 2024, 'PH04', 'DC04', 'NSX01', 'TT01', 4, N'', 24, 10, 25000000, 31500000),
    ('MH12', N'Yamaha NVX 155', 'TL02', 'HSX02', 'MS01', 2024, 'PH03', 'DC04', 'NSX01', 'TT01', 5, N'', 24, 2, 45000000, 55000000),
    ('MH13', N'VinFast Klara S', 'TL06', 'HSX04', 'MS03', 2024, 'PH01', 'DC03', 'NSX01', 'TT01', 0, N'', 36, 5, 28000000, 39000000),
    ('MH14', N'Piaggio Medley 150 ABS', 'TL02', 'HSX05', 'MS05', 2024, 'PH03', 'DC02', 'NSX01', 'TT01', 7, N'', 36, 2, 69000000, 85000000),
    ('MH15', N'Honda Wave Alpha 110', 'TL01', 'HSX01', 'MS02', 2024, 'PH02', 'DC01', 'NSX01', 'TT01', 4, N'', 24, 12, 17000000, 22000000),
    ('MH16', N'SYM Elegant 50', 'TL01', 'HSX06', 'MS07', 2024, 'PH02', 'DC01', 'NSX04', 'TT01', 3, N'', 24, 9, 13000000, 18000000),
    ('MH17', N'Kawasaki Ninja 400', 'TL03', 'HSX07', 'MS04', 2024, 'PH03', 'DC06', 'NSX05', 'TT01', 14, N'', 36, 1, 145000000, 175000000),
    ('MH18', N'Honda SH 160i ABS', 'TL02', 'HSX01', 'MS01', 2024, 'PH03', 'DC05', 'NSX01', 'TT01', 7, N'', 36, 2, 93000000, 115000000),
    ('MH19', N'Yamaha Sirius FI', 'TL01', 'HSX02', 'MS06', 2024, 'PH02', 'DC04', 'NSX01', 'TT01', 4, N'', 24, 8, 19000000, 24000000),
    ('MH20', N'Suzuki Burgman Street 125', 'TL02', 'HSX03', 'MS08', 2024, 'PH01', 'DC02', 'NSX06', 'TT01', 6, N'', 24, 4, 41000000, 52000000),
    ('MH21', N'Honda PCX 160', 'TL02', 'HSX01', 'MS01', 2024, 'PH03', 'DC05', 'NSX01', 'TT01', 8, N'', 36, 0, 65000000, 79000000),
    ('MH22', N'Yamaha Latte 125', 'TL02', 'HSX02', 'MS08', 2024, 'PH04', 'DC04', 'NSX01', 'TT01', 4, N'', 24, 15, 32000000, 39000000)
) AS source(mahang, tenhang, maloai, mahangsx, mamau, namsx, maphanh, madongco, manuocsx, matt, dungtichbinhxang, anh, thoigianbaohanh, soluong, dongianhap, dongiaban)
ON target.mahang = source.mahang
WHEN MATCHED THEN UPDATE SET tenhang = source.tenhang, maloai = source.maloai, mahangsx = source.mahangsx, mamau = source.mamau, namsx = source.namsx, maphanh = source.maphanh, madongco = source.madongco, manuocsx = source.manuocsx, matt = source.matt, dungtichbinhxang = source.dungtichbinhxang, anh = source.anh, thoigianbaohanh = source.thoigianbaohanh, soluong = source.soluong, dongianhap = source.dongianhap, dongiaban = source.dongiaban
WHEN NOT MATCHED THEN INSERT(mahang, tenhang, maloai, mahangsx, mamau, namsx, maphanh, madongco, manuocsx, matt, dungtichbinhxang, anh, thoigianbaohanh, soluong, dongianhap, dongiaban)
VALUES(source.mahang, source.tenhang, source.maloai, source.mahangsx, source.mamau, source.namsx, source.maphanh, source.madongco, source.manuocsx, source.matt, source.dungtichbinhxang, source.anh, source.thoigianbaohanh, source.soluong, source.dongianhap, source.dongiaban);
GO

-- =============================================================
-- 3. HÓA ĐƠN NHẬP MẪU THEO NHIỀU THÁNG
-- =============================================================
MERGE dbo.tblhoadonnhap AS target
USING (VALUES
    (N'HDNPRO001', 'NV03', DATEADD(day,-175,CONVERT(date,GETDATE())), 'NCC01', 786000000),
    (N'HDNPRO002', 'NV03', DATEADD(day,-145,CONVERT(date,GETDATE())), 'NCC02', 519000000),
    (N'HDNPRO003', 'NV03', DATEADD(day,-115,CONVERT(date,GETDATE())), 'NCC04', 308000000),
    (N'HDNPRO004', 'NV03', DATEADD(day,-85,CONVERT(date,GETDATE())), 'NCC05', 452000000),
    (N'HDNPRO005', 'NV03', DATEADD(day,-50,CONVERT(date,GETDATE())), 'NCC01', 625000000),
    (N'HDNPRO006', 'NV03', DATEADD(day,-18,CONVERT(date,GETDATE())), 'NCC02', 383000000)
) AS source(sohdn, manv, ngaynhap, mancc, tongtien)
ON target.sohdn = source.sohdn
WHEN MATCHED THEN UPDATE SET manv = source.manv, ngaynhap = source.ngaynhap, mancc = source.mancc, tongtien = source.tongtien
WHEN NOT MATCHED THEN INSERT(sohdn, manv, ngaynhap, mancc, tongtien) VALUES(source.sohdn, source.manv, source.ngaynhap, source.mancc, source.tongtien);
GO

MERGE dbo.tblchitiethdn AS target
USING (VALUES
    (N'HDNPRO001', 'MH09', 5, 47000000, 0, 235000000),
    (N'HDNPRO001', 'MH10', 6, 34000000, 0, 204000000),
    (N'HDNPRO001', 'MH15', 12, 17000000, 0, 204000000),
    (N'HDNPRO001', 'MH19', 7, 19000000, 0, 133000000),
    (N'HDNPRO002', 'MH11', 9, 25000000, 0, 225000000),
    (N'HDNPRO002', 'MH12', 4, 45000000, 0, 180000000),
    (N'HDNPRO002', 'MH22', 4, 32000000, 0, 128000000),
    (N'HDNPRO003', 'MH06', 5, 24000000, 0, 120000000),
    (N'HDNPRO003', 'MH13', 4, 28000000, 0, 112000000),
    (N'HDNPRO003', 'MH21', 1, 65000000, 0, 65000000),
    (N'HDNPRO004', 'MH14', 3, 69000000, 0, 207000000),
    (N'HDNPRO004', 'MH20', 4, 41000000, 0, 164000000),
    (N'HDNPRO004', 'MH07', 2, 50000000, 0, 100000000),
    (N'HDNPRO005', 'MH18', 3, 93000000, 0, 279000000),
    (N'HDNPRO005', 'MH03', 2, 57000000, 0, 114000000),
    (N'HDNPRO005', 'MH01', 8, 29000000, 0, 232000000),
    (N'HDNPRO006', 'MH17', 1, 145000000, 0, 145000000),
    (N'HDNPRO006', 'MH11', 4, 25000000, 0, 100000000),
    (N'HDNPRO006', 'MH10', 4, 34000000, 0, 136000000)
) AS source(sohdn, mahang, soluong, dongia, giamgia, thanhtien)
ON target.sohdn = source.sohdn AND target.mahang = source.mahang
WHEN MATCHED THEN UPDATE SET soluong = source.soluong, dongia = source.dongia, giamgia = source.giamgia, thanhtien = source.thanhtien
WHEN NOT MATCHED THEN INSERT(sohdn, mahang, soluong, dongia, giamgia, thanhtien) VALUES(source.sohdn, source.mahang, source.soluong, source.dongia, source.giamgia, source.thanhtien);
GO

-- =============================================================
-- 4. ĐƠN BÁN MẪU ĐỂ AI DỰ BÁO, TÌM TOP SẢN PHẨM, TOP KHÁCH HÀNG
-- =============================================================
MERGE dbo.tbldondathang AS target
USING (VALUES
    (N'DDHPRO001', 'NV02', DATEADD(day,-150,CONVERT(date,GETDATE())), 'KH06', 10000000, 0, 56000000),
    (N'DDHPRO002', 'NV02', DATEADD(day,-135,CONVERT(date,GETDATE())), 'KH07', 10000000, 0, 56000000),
    (N'DDHPRO003', 'NV05', DATEADD(day,-121,CONVERT(date,GETDATE())), 'KH08', 25000000, 0, 157000000),
    (N'DDHPRO004', 'NV02', DATEADD(day,-110,CONVERT(date,GETDATE())), 'KH09', 8000000, 0, 63000000),
    (N'DDHPRO005', 'NV02', DATEADD(day,-96,CONVERT(date,GETDATE())), 'KH10', 10000000, 0, 71000000),
    (N'DDHPRO006', 'NV05', DATEADD(day,-82,CONVERT(date,GETDATE())), 'KH11', 12000000, 0, 55000000),
    (N'DDHPRO007', 'NV02', DATEADD(day,-68,CONVERT(date,GETDATE())), 'KH12', 12000000, 0, 76000000),
    (N'DDHPRO008', 'NV05', DATEADD(day,-55,CONVERT(date,GETDATE())), 'KH13', 10000000, 0, 68000000),
    (N'DDHPRO009', 'NV02', DATEADD(day,-47,CONVERT(date,GETDATE())), 'KH14', 15000000, 0, 69000000),
    (N'DDHPRO010', 'NV04', DATEADD(day,-38,CONVERT(date,GETDATE())), 'KH15', 10000000, 0, 56000000),
    (N'DDHPRO011', 'NV02', DATEADD(day,-29,CONVERT(date,GETDATE())), 'KH16', 20000000, 0, 115000000),
    (N'DDHPRO012', 'NV02', DATEADD(day,-25,CONVERT(date,GETDATE())), 'KH06', 12000000, 0, 65500000),
    (N'DDHPRO013', 'NV05', DATEADD(day,-20,CONVERT(date,GETDATE())), 'KH17', 15000000, 0, 85000000),
    (N'DDHPRO014', 'NV02', DATEADD(day,-15,CONVERT(date,GETDATE())), 'KH18', 10000000, 0, 64000000),
    (N'DDHPRO015', 'NV02', DATEADD(day,-10,CONVERT(date,GETDATE())), 'KH19', 15000000, 0, 79000000),
    (N'DDHPRO016', 'NV05', DATEADD(day,-7,CONVERT(date,GETDATE())), 'KH20', 30000000, 0, 175000000),
    (N'DDHPRO017', 'NV02', DATEADD(day,-4,CONVERT(date,GETDATE())), 'KH12', 8000000, 0, 46000000),
    (N'DDHPRO018', 'NV04', DATEADD(day,-2,CONVERT(date,GETDATE())), 'KH13', 12000000, 0, 81000000)
) AS source(soddh, manv, ngaymua, makhach, datcoc, thue, tongtien)
ON target.soddh = source.soddh
WHEN MATCHED THEN UPDATE SET manv = source.manv, ngaymua = source.ngaymua, makhach = source.makhach, datcoc = source.datcoc, thue = source.thue, tongtien = source.tongtien
WHEN NOT MATCHED THEN INSERT(soddh, manv, ngaymua, makhach, datcoc, thue, tongtien) VALUES(source.soddh, source.manv, source.ngaymua, source.makhach, source.datcoc, source.thue, source.tongtien);
GO

MERGE dbo.tblchitietddh AS target
USING (VALUES
    (N'DDHPRO001', 'MH01', 1, 0, 34000000),
    (N'DDHPRO001', 'MH15', 1, 0, 22000000),
    (N'DDHPRO002', 'MH09', 1, 0, 56000000),
    (N'DDHPRO003', 'MH18', 1, 0, 115000000),
    (N'DDHPRO003', 'MH10', 1, 0, 42000000),
    (N'DDHPRO004', 'MH11', 2, 0, 63000000),
    (N'DDHPRO005', 'MH06', 1, 0, 32000000),
    (N'DDHPRO005', 'MH13', 1, 0, 39000000),
    (N'DDHPRO006', 'MH12', 1, 0, 55000000),
    (N'DDHPRO007', 'MH01', 1, 0, 34000000),
    (N'DDHPRO007', 'MH10', 1, 0, 42000000),
    (N'DDHPRO008', 'MH15', 2, 0, 44000000),
    (N'DDHPRO008', 'MH19', 1, 0, 24000000),
    (N'DDHPRO009', 'MH03', 1, 0, 69000000),
    (N'DDHPRO010', 'MH09', 1, 0, 56000000),
    (N'DDHPRO011', 'MH18', 1, 0, 115000000),
    (N'DDHPRO012', 'MH01', 1, 0, 34000000),
    (N'DDHPRO012', 'MH11', 1, 0, 31500000),
    (N'DDHPRO013', 'MH14', 1, 0, 85000000),
    (N'DDHPRO014', 'MH06', 2, 0, 64000000),
    (N'DDHPRO015', 'MH21', 1, 0, 79000000),
    (N'DDHPRO016', 'MH17', 1, 0, 175000000),
    (N'DDHPRO017', 'MH15', 1, 0, 22000000),
    (N'DDHPRO017', 'MH19', 1, 0, 24000000),
    (N'DDHPRO018', 'MH10', 1, 0, 42000000),
    (N'DDHPRO018', 'MH13', 1, 0, 39000000)
) AS source(soddh, mahang, soluong, giamgia, thanhtien)
ON target.soddh = source.soddh AND target.mahang = source.mahang
WHEN MATCHED THEN UPDATE SET soluong = source.soluong, giamgia = source.giamgia, thanhtien = source.thanhtien
WHEN NOT MATCHED THEN INSERT(soddh, mahang, soluong, giamgia, thanhtien) VALUES(source.soddh, source.mahang, source.soluong, source.giamgia, source.thanhtien);
GO

-- Cố định tồn kho mẫu cuối kỳ để tạo đủ tình huống: bán chạy, sắp hết, hết hàng, tồn cao.
UPDATE dbo.tbldmhang SET soluong = CASE mahang
    WHEN 'MH01' THEN 5 WHEN 'MH02' THEN 5 WHEN 'MH03' THEN 2 WHEN 'MH04' THEN 4
    WHEN 'MH05' THEN 3 WHEN 'MH06' THEN 4 WHEN 'MH07' THEN 3 WHEN 'MH08' THEN 4
    WHEN 'MH09' THEN 3 WHEN 'MH10' THEN 2 WHEN 'MH11' THEN 7 WHEN 'MH12' THEN 2
    WHEN 'MH13' THEN 3 WHEN 'MH14' THEN 1 WHEN 'MH15' THEN 8 WHEN 'MH16' THEN 9
    WHEN 'MH17' THEN 0 WHEN 'MH18' THEN 1 WHEN 'MH19' THEN 6 WHEN 'MH20' THEN 4
    WHEN 'MH21' THEN 0 WHEN 'MH22' THEN 15
    ELSE soluong END
WHERE mahang IN ('MH01','MH02','MH03','MH04','MH05','MH06','MH07','MH08','MH09','MH10','MH11','MH12','MH13','MH14','MH15','MH16','MH17','MH18','MH19','MH20','MH21','MH22');
GO

-- =============================================================
-- 5. TÀI KHOẢN DEMO MỞ RỘNG (ĐĂNG NHẬP ĐƯỢC NGAY)
-- =============================================================
IF COL_LENGTH('dbo.tblusers', 'makhach') IS NULL
    ALTER TABLE dbo.tblusers ADD makhach CHAR(10) NULL;
GO

MERGE dbo.tblpermissions AS target
USING (VALUES
    (N'ShopOrder', N'Mua hàng trực tuyến', N'Khách hàng tự đăng nhập để đặt mua xe')
) AS source(permissionkey, displayname, description)
ON target.permissionkey = source.permissionkey
WHEN MATCHED THEN UPDATE SET displayname = source.displayname, description = source.description
WHEN NOT MATCHED THEN INSERT(permissionkey, displayname, description) VALUES(source.permissionkey, source.displayname, source.description);
GO

MERGE dbo.tblroles AS target
USING (VALUES
    (N'Customer', N'Khách hàng', N'Tài khoản khách hàng tự đăng nhập và mua sản phẩm', 1)
) AS source(rolename, displayname, description, isbuiltin)
ON target.rolename = source.rolename
WHEN MATCHED THEN UPDATE SET displayname = source.displayname, description = source.description, isbuiltin = source.isbuiltin
WHEN NOT MATCHED THEN INSERT(rolename, displayname, description, isbuiltin) VALUES(source.rolename, source.displayname, source.description, source.isbuiltin);
GO

INSERT INTO dbo.tblrolepermissions(roleid, permissionkey)
SELECT r.roleid, N'ShopOrder'
FROM dbo.tblroles r
WHERE r.rolename = N'Customer'
  AND NOT EXISTS (
      SELECT 1 FROM dbo.tblrolepermissions x
      WHERE x.roleid = r.roleid AND x.permissionkey = N'ShopOrder'
  );
GO

-- Mật khẩu staff demo: Demo@12345
-- Hash PBKDF2-SHA256 (160000): 6xjh+9hNU/WONktI4j4Rz4H5SH+52iduZT2PPc1MTAI=
-- Salt: BXQhZicr8Hr7FmbQNIuq+Q==
MERGE dbo.tblusers AS target
USING (VALUES
    (N'demo_admin', N'Admin Demo', N'6xjh+9hNU/WONktI4j4Rz4H5SH+52iduZT2PPc1MTAI=', N'BXQhZicr8Hr7FmbQNIuq+Q==', 160000, N'Administrator', 1, NULL),
    (N'demo_manager', N'Quản lý Demo', N'6xjh+9hNU/WONktI4j4Rz4H5SH+52iduZT2PPc1MTAI=', N'BXQhZicr8Hr7FmbQNIuq+Q==', 160000, N'Manager', 1, NULL),
    (N'demo_sales_1', N'Bán hàng Demo 1', N'6xjh+9hNU/WONktI4j4Rz4H5SH+52iduZT2PPc1MTAI=', N'BXQhZicr8Hr7FmbQNIuq+Q==', 160000, N'Sales', 1, NULL),
    (N'demo_sales_2', N'Bán hàng Demo 2', N'6xjh+9hNU/WONktI4j4Rz4H5SH+52iduZT2PPc1MTAI=', N'BXQhZicr8Hr7FmbQNIuq+Q==', 160000, N'Sales', 1, NULL),
    (N'demo_warehouse', N'Thủ kho Demo', N'6xjh+9hNU/WONktI4j4Rz4H5SH+52iduZT2PPc1MTAI=', N'BXQhZicr8Hr7FmbQNIuq+Q==', 160000, N'Warehouse', 1, NULL),
    (N'demo_viewer', N'Xem báo cáo Demo', N'6xjh+9hNU/WONktI4j4Rz4H5SH+52iduZT2PPc1MTAI=', N'BXQhZicr8Hr7FmbQNIuq+Q==', 160000, N'Viewer', 1, NULL)
) AS source(username, displayname, passwordhash, passwordsalt, passworditerations, rolename, isactive, makhach)
ON target.username = source.username
WHEN MATCHED THEN UPDATE SET
    displayname = source.displayname,
    passwordhash = source.passwordhash,
    passwordsalt = source.passwordsalt,
    passworditerations = source.passworditerations,
    roleid = (SELECT roleid FROM dbo.tblroles WHERE rolename = source.rolename),
    isactive = source.isactive,
    failedlogincount = 0,
    lockoutendat = NULL,
    mustchangepassword = 0,
    passwordchangedat = GETDATE(),
    makhach = source.makhach
WHEN NOT MATCHED THEN
    INSERT(username, displayname, passwordhash, passwordsalt, passworditerations, roleid, isactive, failedlogincount, lockoutendat, mustchangepassword, passwordchangedat, makhach)
    VALUES(source.username, source.displayname, source.passwordhash, source.passwordsalt, source.passworditerations,
           (SELECT roleid FROM dbo.tblroles WHERE rolename = source.rolename),
           source.isactive, 0, NULL, 0, GETDATE(), source.makhach);
GO

-- Mật khẩu customer demo: Customer@12345
-- Hash PBKDF2-SHA256 (160000): WoLOSjviMc1MTpy7fvI6fMbH6/KVs0aFULlLItIum4g=
-- Salt: Kw+zYZr7Q6Bu1SHOneWO6Q==
MERGE dbo.tblusers AS target
USING (VALUES
    (N'customer_01', N'Bùi Ngọc Anh', N'WoLOSjviMc1MTpy7fvI6fMbH6/KVs0aFULlLItIum4g=', N'Kw+zYZr7Q6Bu1SHOneWO6Q==', 160000, N'Customer', 1, 'KH06'),
    (N'customer_02', N'Nguyễn Hải Nam', N'WoLOSjviMc1MTpy7fvI6fMbH6/KVs0aFULlLItIum4g=', N'Kw+zYZr7Q6Bu1SHOneWO6Q==', 160000, N'Customer', 1, 'KH07'),
    (N'customer_03', N'Trần Gia Hân', N'WoLOSjviMc1MTpy7fvI6fMbH6/KVs0aFULlLItIum4g=', N'Kw+zYZr7Q6Bu1SHOneWO6Q==', 160000, N'Customer', 1, 'KH08'),
    (N'customer_04', N'Lê Quốc Bảo', N'WoLOSjviMc1MTpy7fvI6fMbH6/KVs0aFULlLItIum4g=', N'Kw+zYZr7Q6Bu1SHOneWO6Q==', 160000, N'Customer', 1, 'KH09'),
    (N'customer_05', N'Phạm Minh Châu', N'WoLOSjviMc1MTpy7fvI6fMbH6/KVs0aFULlLItIum4g=', N'Kw+zYZr7Q6Bu1SHOneWO6Q==', 160000, N'Customer', 1, 'KH10'),
    (N'customer_06', N'Hoàng Tuấn Kiệt', N'WoLOSjviMc1MTpy7fvI6fMbH6/KVs0aFULlLItIum4g=', N'Kw+zYZr7Q6Bu1SHOneWO6Q==', 160000, N'Customer', 1, 'KH11'),
    (N'customer_07', N'Đặng Thảo Vy', N'WoLOSjviMc1MTpy7fvI6fMbH6/KVs0aFULlLItIum4g=', N'Kw+zYZr7Q6Bu1SHOneWO6Q==', 160000, N'Customer', 1, 'KH12'),
    (N'customer_08', N'Võ Đức Anh', N'WoLOSjviMc1MTpy7fvI6fMbH6/KVs0aFULlLItIum4g=', N'Kw+zYZr7Q6Bu1SHOneWO6Q==', 160000, N'Customer', 1, 'KH13'),
    (N'customer_09', N'Mai Phương Linh', N'WoLOSjviMc1MTpy7fvI6fMbH6/KVs0aFULlLItIum4g=', N'Kw+zYZr7Q6Bu1SHOneWO6Q==', 160000, N'Customer', 1, 'KH14'),
    (N'customer_10', N'Đỗ Thanh Phong', N'WoLOSjviMc1MTpy7fvI6fMbH6/KVs0aFULlLItIum4g=', N'Kw+zYZr7Q6Bu1SHOneWO6Q==', 160000, N'Customer', 1, 'KH15')
) AS source(username, displayname, passwordhash, passwordsalt, passworditerations, rolename, isactive, makhach)
ON target.username = source.username
WHEN MATCHED THEN UPDATE SET
    displayname = source.displayname,
    passwordhash = source.passwordhash,
    passwordsalt = source.passwordsalt,
    passworditerations = source.passworditerations,
    roleid = (SELECT roleid FROM dbo.tblroles WHERE rolename = source.rolename),
    isactive = source.isactive,
    failedlogincount = 0,
    lockoutendat = NULL,
    mustchangepassword = 0,
    passwordchangedat = GETDATE(),
    makhach = source.makhach
WHEN NOT MATCHED THEN
    INSERT(username, displayname, passwordhash, passwordsalt, passworditerations, roleid, isactive, failedlogincount, lockoutendat, mustchangepassword, passwordchangedat, makhach)
    VALUES(source.username, source.displayname, source.passwordhash, source.passwordsalt, source.passworditerations,
           (SELECT roleid FROM dbo.tblroles WHERE rolename = source.rolename),
           source.isactive, 0, NULL, 0, GETDATE(), source.makhach);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.tblauditlog WHERE eventtype = N'ProfessionalSampleDataSeeded')
BEGIN
    INSERT INTO dbo.tblauditlog(eventtype, username, userid, detail)
    VALUES(N'ProfessionalSampleDataSeeded', N'system', NULL, N'Added professional sample data for AI assistant, reports and dashboard.');
END
GO

IF OBJECT_ID(N'dbo.sp_QLXeMay_DatabaseHealthCheck', N'P') IS NOT NULL
    EXEC dbo.sp_QLXeMay_DatabaseHealthCheck;
GO

PRINT N'QLXeMay professional sample data: đã bổ sung dữ liệu mẫu cho Trợ lý AI.';
GO
