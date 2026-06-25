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

IF NOT EXISTS (SELECT 1 FROM dbo.tblauditlog WHERE eventtype = N'ProfessionalSampleDataSeeded')
BEGIN
    INSERT INTO dbo.tblauditlog(eventtype, username, userid, detail)
    VALUES(N'ProfessionalSampleDataSeeded', N'system', NULL, N'Added professional sample data for AI assistant, reports and dashboard.');
END
GO

EXEC dbo.sp_QLXeMay_DatabaseHealthCheck;
GO

PRINT N'QLXeMay professional sample data: đã bổ sung dữ liệu mẫu cho Trợ lý AI.';
GO
