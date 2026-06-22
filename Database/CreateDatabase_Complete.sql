-- =============================================================
-- QLXeMay WPF - DATABASE COMPLETE
-- Version: 2026.06 Complete
-- Mục đích: tạo/migrate database btl đầy đủ cho dự án QLXeMay WPF.
-- Chạy bằng SQL Server Management Studio hoặc sqlcmd.
-- =============================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF DB_ID(N'btl') IS NULL
BEGIN
    CREATE DATABASE btl;
END
GO

USE btl;
GO

-- =============================================================
-- 1. BẢNG DANH MỤC NGHIỆP VỤ
-- =============================================================
IF OBJECT_ID(N'dbo.tblcongviec', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblcongviec (
        macv CHAR(10) NOT NULL,
        tencv NVARCHAR(50) NOT NULL,
        luongthang DECIMAL(18,2) NOT NULL CONSTRAINT DF_tblcongviec_luongthang DEFAULT 0,
        CONSTRAINT PK_tblcongviec PRIMARY KEY CLUSTERED (macv),
        CONSTRAINT CK_tblcongviec_luongthang CHECK (luongthang >= 0)
    );
END
GO

IF OBJECT_ID(N'dbo.tblkhachhang', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblkhachhang (
        makhach CHAR(10) NOT NULL,
        tenkhach NVARCHAR(50) NOT NULL,
        diachi NVARCHAR(100) NOT NULL,
        sdt VARCHAR(15) NOT NULL,
        CONSTRAINT PK_tblkhachhang PRIMARY KEY CLUSTERED (makhach)
    );
END
GO

IF OBJECT_ID(N'dbo.tblnhacungcap', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblnhacungcap (
        mancc CHAR(10) NOT NULL,
        tenncc NVARCHAR(80) NOT NULL,
        diachi NVARCHAR(100) NOT NULL,
        sdt VARCHAR(15) NOT NULL,
        CONSTRAINT PK_tblnhacungcap PRIMARY KEY CLUSTERED (mancc)
    );
END
GO

IF OBJECT_ID(N'dbo.tblmausac', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblmausac (
        mamau CHAR(10) NOT NULL,
        tenmau NVARCHAR(30) NOT NULL,
        CONSTRAINT PK_tblmausac PRIMARY KEY CLUSTERED (mamau)
    );
END
GO

IF OBJECT_ID(N'dbo.tblhangsx', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblhangsx (
        mahangsx CHAR(10) NOT NULL,
        tenhangsx NVARCHAR(50) NOT NULL,
        CONSTRAINT PK_tblhangsx PRIMARY KEY CLUSTERED (mahangsx)
    );
END
GO

IF OBJECT_ID(N'dbo.tbldongco', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tbldongco (
        madongco CHAR(10) NOT NULL,
        tendongco NVARCHAR(50) NOT NULL,
        CONSTRAINT PK_tbldongco PRIMARY KEY CLUSTERED (madongco)
    );
END
GO

IF OBJECT_ID(N'dbo.tblnuocsx', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblnuocsx (
        manuocsx CHAR(10) NOT NULL,
        tennuocsx NVARCHAR(30) NOT NULL,
        CONSTRAINT PK_tblnuocsx PRIMARY KEY CLUSTERED (manuocsx)
    );
END
GO

IF OBJECT_ID(N'dbo.tbltheloai', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tbltheloai (
        maloai CHAR(10) NOT NULL,
        tenloai NVARCHAR(50) NOT NULL,
        CONSTRAINT PK_tbltheloai PRIMARY KEY CLUSTERED (maloai)
    );
END
GO

IF OBJECT_ID(N'dbo.tbltinhtrang', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tbltinhtrang (
        matt CHAR(10) NOT NULL,
        tentt NVARCHAR(30) NOT NULL,
        CONSTRAINT PK_tbltinhtrang PRIMARY KEY CLUSTERED (matt)
    );
END
GO

IF OBJECT_ID(N'dbo.tblphanhxe', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblphanhxe (
        maphanh CHAR(10) NOT NULL,
        tenphanh NVARCHAR(50) NOT NULL,
        CONSTRAINT PK_tblphanhxe PRIMARY KEY CLUSTERED (maphanh)
    );
END
GO

IF OBJECT_ID(N'dbo.tblnhanvien', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblnhanvien (
        manv CHAR(10) NOT NULL,
        tennv NVARCHAR(50) NOT NULL,
        gioitinh NVARCHAR(4) NOT NULL,
        ngaysinh DATE NOT NULL,
        sdt VARCHAR(15) NOT NULL,
        diachi NVARCHAR(100) NOT NULL,
        macv CHAR(10) NOT NULL,
        CONSTRAINT PK_tblnhanvien PRIMARY KEY CLUSTERED (manv),
        CONSTRAINT FK_tblnhanvien_tblcongviec FOREIGN KEY (macv) REFERENCES dbo.tblcongviec(macv),
        CONSTRAINT CK_tblnhanvien_gioitinh CHECK (gioitinh IN (N'Nam', N'Nữ', N'Khác'))
    );
END
GO

-- =============================================================
-- 2. HÀNG HÓA, HÓA ĐƠN NHẬP, HÓA ĐƠN BÁN
-- =============================================================
IF OBJECT_ID(N'dbo.tbldmhang', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tbldmhang (
        mahang CHAR(10) NOT NULL,
        tenhang NVARCHAR(80) NOT NULL,
        maloai CHAR(10) NOT NULL,
        mahangsx CHAR(10) NOT NULL,
        mamau CHAR(10) NOT NULL,
        namsx SMALLINT NOT NULL,
        maphanh CHAR(10) NOT NULL,
        madongco CHAR(10) NOT NULL,
        manuocsx CHAR(10) NOT NULL,
        matt CHAR(10) NOT NULL,
        dungtichbinhxang SMALLINT NOT NULL CONSTRAINT DF_tbldmhang_dungtich DEFAULT 0,
        anh NVARCHAR(200) NULL,
        thoigianbaohanh SMALLINT NOT NULL CONSTRAINT DF_tbldmhang_baohanh DEFAULT 0,
        soluong INT NOT NULL CONSTRAINT DF_tbldmhang_soluong DEFAULT 0,
        dongianhap DECIMAL(18,2) NOT NULL CONSTRAINT DF_tbldmhang_dongianhap DEFAULT 0,
        dongiaban DECIMAL(18,2) NOT NULL CONSTRAINT DF_tbldmhang_dongiaban DEFAULT 0,
        CONSTRAINT PK_tbldmhang PRIMARY KEY CLUSTERED (mahang),
        CONSTRAINT FK_tbldmhang_tbltheloai FOREIGN KEY (maloai) REFERENCES dbo.tbltheloai(maloai),
        CONSTRAINT FK_tbldmhang_tblhangsx FOREIGN KEY (mahangsx) REFERENCES dbo.tblhangsx(mahangsx),
        CONSTRAINT FK_tbldmhang_tblmausac FOREIGN KEY (mamau) REFERENCES dbo.tblmausac(mamau),
        CONSTRAINT FK_tbldmhang_tblphanhxe FOREIGN KEY (maphanh) REFERENCES dbo.tblphanhxe(maphanh),
        CONSTRAINT FK_tbldmhang_tbldongco FOREIGN KEY (madongco) REFERENCES dbo.tbldongco(madongco),
        CONSTRAINT FK_tbldmhang_tblnuocsx FOREIGN KEY (manuocsx) REFERENCES dbo.tblnuocsx(manuocsx),
        CONSTRAINT FK_tbldmhang_tbltinhtrang FOREIGN KEY (matt) REFERENCES dbo.tbltinhtrang(matt),
        CONSTRAINT CK_tbldmhang_namsx CHECK (namsx BETWEEN 1990 AND 2100),
        CONSTRAINT CK_tbldmhang_nonnegative CHECK (dungtichbinhxang >= 0 AND thoigianbaohanh >= 0 AND soluong >= 0 AND dongianhap >= 0 AND dongiaban >= 0)
    );
END
GO

IF OBJECT_ID(N'dbo.tblhoadonnhap', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblhoadonnhap (
        sohdn NVARCHAR(20) NOT NULL,
        manv CHAR(10) NOT NULL,
        ngaynhap DATE NOT NULL,
        mancc CHAR(10) NOT NULL,
        tongtien DECIMAL(18,2) NOT NULL CONSTRAINT DF_tblhoadonnhap_tongtien DEFAULT 0,
        CONSTRAINT PK_tblhoadonnhap PRIMARY KEY CLUSTERED (sohdn),
        CONSTRAINT FK_tblhoadonnhap_tblnhanvien FOREIGN KEY (manv) REFERENCES dbo.tblnhanvien(manv),
        CONSTRAINT FK_tblhoadonnhap_tblnhacungcap FOREIGN KEY (mancc) REFERENCES dbo.tblnhacungcap(mancc),
        CONSTRAINT CK_tblhoadonnhap_tongtien CHECK (tongtien >= 0)
    );
END
GO

IF OBJECT_ID(N'dbo.tblchitiethdn', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblchitiethdn (
        sohdn NVARCHAR(20) NOT NULL,
        mahang CHAR(10) NOT NULL,
        soluong INT NOT NULL,
        dongia DECIMAL(18,2) NOT NULL,
        giamgia DECIMAL(5,2) NOT NULL CONSTRAINT DF_tblchitiethdn_giamgia DEFAULT 0,
        thanhtien DECIMAL(18,2) NOT NULL,
        CONSTRAINT PK_tblchitiethdn PRIMARY KEY CLUSTERED (sohdn, mahang),
        CONSTRAINT FK_tblchitiethdn_tblhoadonnhap FOREIGN KEY (sohdn) REFERENCES dbo.tblhoadonnhap(sohdn),
        CONSTRAINT FK_tblchitiethdn_tbldmhang FOREIGN KEY (mahang) REFERENCES dbo.tbldmhang(mahang),
        CONSTRAINT CK_tblchitiethdn_values CHECK (soluong > 0 AND dongia >= 0 AND giamgia BETWEEN 0 AND 100 AND thanhtien >= 0)
    );
END
GO

IF OBJECT_ID(N'dbo.tbldondathang', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tbldondathang (
        soddh NVARCHAR(20) NOT NULL,
        manv CHAR(10) NOT NULL,
        ngaymua DATE NOT NULL,
        makhach CHAR(10) NOT NULL,
        datcoc DECIMAL(18,2) NOT NULL CONSTRAINT DF_tbldondathang_datcoc DEFAULT 0,
        thue DECIMAL(18,2) NOT NULL CONSTRAINT DF_tbldondathang_thue DEFAULT 0,
        tongtien DECIMAL(18,2) NOT NULL CONSTRAINT DF_tbldondathang_tongtien DEFAULT 0,
        CONSTRAINT PK_tbldondathang PRIMARY KEY CLUSTERED (soddh),
        CONSTRAINT FK_tbldondathang_tblnhanvien FOREIGN KEY (manv) REFERENCES dbo.tblnhanvien(manv),
        CONSTRAINT FK_tbldondathang_tblkhachhang FOREIGN KEY (makhach) REFERENCES dbo.tblkhachhang(makhach),
        CONSTRAINT CK_tbldondathang_values CHECK (datcoc >= 0 AND thue >= 0 AND tongtien >= 0)
    );
END
GO

IF OBJECT_ID(N'dbo.tblchitietddh', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblchitietddh (
        soddh NVARCHAR(20) NOT NULL,
        mahang CHAR(10) NOT NULL,
        soluong INT NOT NULL,
        giamgia DECIMAL(5,2) NOT NULL CONSTRAINT DF_tblchitietddh_giamgia DEFAULT 0,
        thanhtien DECIMAL(18,2) NOT NULL,
        CONSTRAINT PK_tblchitietddh PRIMARY KEY CLUSTERED (soddh, mahang),
        CONSTRAINT FK_tblchitietddh_tbldondathang FOREIGN KEY (soddh) REFERENCES dbo.tbldondathang(soddh),
        CONSTRAINT FK_tblchitietddh_tbldmhang FOREIGN KEY (mahang) REFERENCES dbo.tbldmhang(mahang),
        CONSTRAINT CK_tblchitietddh_values CHECK (soluong > 0 AND giamgia BETWEEN 0 AND 100 AND thanhtien >= 0)
    );
END
GO

-- =============================================================
-- 3. SECURITY: ROLE, PERMISSION, USER, AUDIT LOG
-- =============================================================
IF OBJECT_ID(N'dbo.tblroles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblroles (
        roleid INT IDENTITY(1,1) NOT NULL,
        rolename NVARCHAR(50) NOT NULL,
        displayname NVARCHAR(100) NOT NULL,
        description NVARCHAR(255) NULL,
        isbuiltin BIT NOT NULL CONSTRAINT DF_tblroles_isbuiltin DEFAULT 0,
        CONSTRAINT PK_tblroles PRIMARY KEY CLUSTERED (roleid),
        CONSTRAINT UQ_tblroles_rolename UNIQUE (rolename)
    );
END
GO

IF OBJECT_ID(N'dbo.tblpermissions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblpermissions (
        permissionkey NVARCHAR(80) NOT NULL,
        displayname NVARCHAR(120) NOT NULL,
        description NVARCHAR(255) NULL,
        CONSTRAINT PK_tblpermissions PRIMARY KEY CLUSTERED (permissionkey)
    );
END
GO

IF OBJECT_ID(N'dbo.tblrolepermissions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblrolepermissions (
        roleid INT NOT NULL,
        permissionkey NVARCHAR(80) NOT NULL,
        CONSTRAINT PK_tblrolepermissions PRIMARY KEY CLUSTERED (roleid, permissionkey),
        CONSTRAINT FK_tblrolepermissions_tblroles FOREIGN KEY (roleid) REFERENCES dbo.tblroles(roleid),
        CONSTRAINT FK_tblrolepermissions_tblpermissions FOREIGN KEY (permissionkey) REFERENCES dbo.tblpermissions(permissionkey)
    );
END
GO

IF OBJECT_ID(N'dbo.tblusers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblusers (
        userid INT IDENTITY(1,1) NOT NULL,
        username NVARCHAR(50) NOT NULL,
        displayname NVARCHAR(100) NOT NULL,
        passwordhash NVARCHAR(200) NOT NULL,
        passwordsalt NVARCHAR(200) NOT NULL,
        passworditerations INT NOT NULL,
        roleid INT NOT NULL,
        isactive BIT NOT NULL CONSTRAINT DF_tblusers_isactive DEFAULT 1,
        failedlogincount INT NOT NULL CONSTRAINT DF_tblusers_failedlogincount DEFAULT 0,
        createdat DATETIME NOT NULL CONSTRAINT DF_tblusers_createdat DEFAULT GETDATE(),
        lastloginat DATETIME NULL,
        lockoutendat DATETIME NULL,
        mustchangepassword BIT NOT NULL CONSTRAINT DF_tblusers_mustchangepassword DEFAULT 0,
        passwordchangedat DATETIME NULL,
        CONSTRAINT PK_tblusers PRIMARY KEY CLUSTERED (userid),
        CONSTRAINT UQ_tblusers_username UNIQUE (username),
        CONSTRAINT FK_tblusers_tblroles FOREIGN KEY (roleid) REFERENCES dbo.tblroles(roleid),
        CONSTRAINT CK_tblusers_failedlogincount CHECK (failedlogincount >= 0),
        CONSTRAINT CK_tblusers_passworditerations CHECK (passworditerations >= 10000)
    );
END
GO

-- Migration an toàn cho DB cũ.
IF COL_LENGTH('dbo.tblusers', 'lockoutendat') IS NULL
    ALTER TABLE dbo.tblusers ADD lockoutendat DATETIME NULL;
GO
IF COL_LENGTH('dbo.tblusers', 'mustchangepassword') IS NULL
    ALTER TABLE dbo.tblusers ADD mustchangepassword BIT NOT NULL CONSTRAINT DF_tblusers_mustchangepassword DEFAULT 0 WITH VALUES;
GO
IF COL_LENGTH('dbo.tblusers', 'passwordchangedat') IS NULL
    ALTER TABLE dbo.tblusers ADD passwordchangedat DATETIME NULL;
GO

IF OBJECT_ID(N'dbo.tblauditlog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblauditlog (
        auditid INT IDENTITY(1,1) NOT NULL,
        eventtype NVARCHAR(80) NOT NULL,
        username NVARCHAR(50) NULL,
        userid INT NULL,
        detail NVARCHAR(500) NULL,
        createdat DATETIME NOT NULL CONSTRAINT DF_tblauditlog_createdat DEFAULT GETDATE(),
        CONSTRAINT PK_tblauditlog PRIMARY KEY CLUSTERED (auditid)
    );
END
GO

-- =============================================================
-- 4. INDEX TỐI ƯU CHO TRA CỨU, BÁO CÁO, AUTH
-- =============================================================
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_tblnhanvien_macv' AND object_id = OBJECT_ID(N'dbo.tblnhanvien'))
    CREATE INDEX IX_tblnhanvien_macv ON dbo.tblnhanvien(macv);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_tbldmhang_lookup' AND object_id = OBJECT_ID(N'dbo.tbldmhang'))
    CREATE INDEX IX_tbldmhang_lookup ON dbo.tbldmhang(maloai, mahangsx, mamau, matt) INCLUDE (tenhang, soluong, dongiaban);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_tbldmhang_tenhang' AND object_id = OBJECT_ID(N'dbo.tbldmhang'))
    CREATE INDEX IX_tbldmhang_tenhang ON dbo.tbldmhang(tenhang);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_tblhoadonnhap_ngaynhap' AND object_id = OBJECT_ID(N'dbo.tblhoadonnhap'))
    CREATE INDEX IX_tblhoadonnhap_ngaynhap ON dbo.tblhoadonnhap(ngaynhap DESC) INCLUDE (tongtien, mancc, manv);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_tbldondathang_ngaymua' AND object_id = OBJECT_ID(N'dbo.tbldondathang'))
    CREATE INDEX IX_tbldondathang_ngaymua ON dbo.tbldondathang(ngaymua DESC) INCLUDE (tongtien, makhach, manv);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_tblchitiethdn_mahang' AND object_id = OBJECT_ID(N'dbo.tblchitiethdn'))
    CREATE INDEX IX_tblchitiethdn_mahang ON dbo.tblchitiethdn(mahang);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_tblchitietddh_mahang' AND object_id = OBJECT_ID(N'dbo.tblchitietddh'))
    CREATE INDEX IX_tblchitietddh_mahang ON dbo.tblchitietddh(mahang);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_tblusers_roleid' AND object_id = OBJECT_ID(N'dbo.tblusers'))
    CREATE INDEX IX_tblusers_roleid ON dbo.tblusers(roleid);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_tblusers_active_lockout' AND object_id = OBJECT_ID(N'dbo.tblusers'))
    CREATE INDEX IX_tblusers_active_lockout ON dbo.tblusers(isactive, lockoutendat) INCLUDE (username, failedlogincount);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_tblauditlog_createdat' AND object_id = OBJECT_ID(N'dbo.tblauditlog'))
    CREATE INDEX IX_tblauditlog_createdat ON dbo.tblauditlog(createdat DESC);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_tblauditlog_eventtype' AND object_id = OBJECT_ID(N'dbo.tblauditlog'))
    CREATE INDEX IX_tblauditlog_eventtype ON dbo.tblauditlog(eventtype, createdat DESC);
GO

-- =============================================================
-- 5. SEED MASTER DATA
-- =============================================================
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

MERGE dbo.tblnuocsx AS target
USING (VALUES
    ('NSX01', N'Việt Nam'), ('NSX02', N'Nhật Bản'), ('NSX03', N'Ý'),
    ('NSX04', N'Trung Quốc'), ('NSX05', N'Thái Lan'), ('NSX06', N'Indonesia')
) AS source(manuocsx, tennuocsx)
ON target.manuocsx = source.manuocsx
WHEN MATCHED THEN UPDATE SET tennuocsx = source.tennuocsx
WHEN NOT MATCHED THEN INSERT(manuocsx, tennuocsx) VALUES(source.manuocsx, source.tennuocsx);
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

MERGE dbo.tbltinhtrang AS target
USING (VALUES
    ('TT01', N'Mới'), ('TT02', N'Đã qua sử dụng'), ('TT03', N'Đang bảo hành'), ('TT04', N'Tạm ngưng bán')
) AS source(matt, tentt)
ON target.matt = source.matt
WHEN MATCHED THEN UPDATE SET tentt = source.tentt
WHEN NOT MATCHED THEN INSERT(matt, tentt) VALUES(source.matt, source.tentt);
GO

MERGE dbo.tblphanhxe AS target
USING (VALUES
    ('PH01', N'Phanh đĩa'), ('PH02', N'Phanh tang trống'), ('PH03', N'Phanh ABS'), ('PH04', N'CBS')
) AS source(maphanh, tenphanh)
ON target.maphanh = source.maphanh
WHEN MATCHED THEN UPDATE SET tenphanh = source.tenphanh
WHEN NOT MATCHED THEN INSERT(maphanh, tenphanh) VALUES(source.maphanh, source.tenphanh);
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

MERGE dbo.tblkhachhang AS target
USING (VALUES
    ('KH01', N'Lê Văn Cường', N'Hà Nội', '0901234567'),
    ('KH02', N'Phạm Thị Dung', N'Hải Phòng', '0976543210'),
    ('KH03', N'Hoàng Minh Đức', N'Đà Nẵng', '0911002200'),
    ('KH04', N'Nguyễn Thu Trang', N'Hà Nội', '0933004400'),
    ('KH05', N'Vũ Anh Tuấn', N'Bắc Ninh', '0966007700')
) AS source(makhach, tenkhach, diachi, sdt)
ON target.makhach = source.makhach
WHEN MATCHED THEN UPDATE SET tenkhach = source.tenkhach, diachi = source.diachi, sdt = source.sdt
WHEN NOT MATCHED THEN INSERT(makhach, tenkhach, diachi, sdt) VALUES(source.makhach, source.tenkhach, source.diachi, source.sdt);
GO

MERGE dbo.tbldmhang AS target
USING (VALUES
    ('MH01', N'Honda Vision 2024', 'TL02', 'HSX01', 'MS01', 2024, 'PH04', 'DC05', 'NSX01', 'TT01', 5, N'', 24, 8, 29000000, 34000000),
    ('MH02', N'Yamaha Exciter 155', 'TL01', 'HSX02', 'MS03', 2024, 'PH01', 'DC04', 'NSX01', 'TT01', 4, N'', 24, 5, 44000000, 50500000),
    ('MH03', N'Honda SH Mode', 'TL02', 'HSX01', 'MS02', 2024, 'PH03', 'DC05', 'NSX01', 'TT01', 7, N'', 36, 3, 57000000, 69000000),
    ('MH04', N'Yamaha Grande', 'TL02', 'HSX02', 'MS05', 2024, 'PH04', 'DC04', 'NSX01', 'TT01', 4, N'', 24, 4, 39000000, 46000000),
    ('MH05', N'Suzuki Raider R150', 'TL03', 'HSX03', 'MS04', 2024, 'PH01', 'DC06', 'NSX06', 'TT01', 4, N'', 24, 3, 43000000, 51000000),
    ('MH06', N'VinFast Feliz S', 'TL06', 'HSX04', 'MS02', 2024, 'PH01', 'DC03', 'NSX01', 'TT01', 0, N'', 36, 6, 24000000, 32000000),
    ('MH07', N'Piaggio Liberty 125', 'TL02', 'HSX05', 'MS08', 2024, 'PH03', 'DC02', 'NSX01', 'TT01', 6, N'', 24, 3, 50000000, 61000000),
    ('MH08', N'Honda Winner X', 'TL01', 'HSX01', 'MS06', 2024, 'PH01', 'DC05', 'NSX01', 'TT01', 4, N'', 24, 4, 39000000, 48000000)
) AS source(mahang, tenhang, maloai, mahangsx, mamau, namsx, maphanh, madongco, manuocsx, matt, dungtichbinhxang, anh, thoigianbaohanh, soluong, dongianhap, dongiaban)
ON target.mahang = source.mahang
WHEN MATCHED THEN UPDATE SET tenhang = source.tenhang, maloai = source.maloai, mahangsx = source.mahangsx, mamau = source.mamau, namsx = source.namsx, maphanh = source.maphanh, madongco = source.madongco, manuocsx = source.manuocsx, matt = source.matt, dungtichbinhxang = source.dungtichbinhxang, anh = source.anh, thoigianbaohanh = source.thoigianbaohanh, soluong = source.soluong, dongianhap = source.dongianhap, dongiaban = source.dongiaban
WHEN NOT MATCHED THEN INSERT(mahang, tenhang, maloai, mahangsx, mamau, namsx, maphanh, madongco, manuocsx, matt, dungtichbinhxang, anh, thoigianbaohanh, soluong, dongianhap, dongiaban) VALUES(source.mahang, source.tenhang, source.maloai, source.mahangsx, source.mamau, source.namsx, source.maphanh, source.madongco, source.manuocsx, source.matt, source.dungtichbinhxang, source.anh, source.thoigianbaohanh, source.soluong, source.dongianhap, source.dongiaban);
GO

-- =============================================================
-- 6. SEED HÓA ĐƠN MẪU CHO BÁO CÁO
-- =============================================================
MERGE dbo.tblhoadonnhap AS target
USING (VALUES
    (N'HDN001', 'NV03', CONVERT(date,'2026-06-01'), 'NCC01', 746000000),
    (N'HDN002', 'NV03', CONVERT(date,'2026-06-05'), 'NCC02', 573000000),
    (N'HDN003', 'NV03', CONVERT(date,'2026-06-08'), 'NCC03', 324000000)
) AS source(sohdn, manv, ngaynhap, mancc, tongtien)
ON target.sohdn = source.sohdn
WHEN MATCHED THEN UPDATE SET manv = source.manv, ngaynhap = source.ngaynhap, mancc = source.mancc, tongtien = source.tongtien
WHEN NOT MATCHED THEN INSERT(sohdn, manv, ngaynhap, mancc, tongtien) VALUES(source.sohdn, source.manv, source.ngaynhap, source.mancc, source.tongtien);
GO

MERGE dbo.tblchitiethdn AS target
USING (VALUES
    (N'HDN001', 'MH01', 10, 29000000, 0, 290000000),
    (N'HDN001', 'MH02', 6, 44000000, 0, 264000000),
    (N'HDN001', 'MH06', 8, 24000000, 0, 192000000),
    (N'HDN002', 'MH03', 4, 57000000, 0, 228000000),
    (N'HDN002', 'MH04', 5, 39000000, 0, 195000000),
    (N'HDN002', 'MH07', 3, 50000000, 0, 150000000),
    (N'HDN003', 'MH05', 3, 43000000, 0, 129000000),
    (N'HDN003', 'MH08', 5, 39000000, 0, 195000000)
) AS source(sohdn, mahang, soluong, dongia, giamgia, thanhtien)
ON target.sohdn = source.sohdn AND target.mahang = source.mahang
WHEN MATCHED THEN UPDATE SET soluong = source.soluong, dongia = source.dongia, giamgia = source.giamgia, thanhtien = source.thanhtien
WHEN NOT MATCHED THEN INSERT(sohdn, mahang, soluong, dongia, giamgia, thanhtien) VALUES(source.sohdn, source.mahang, source.soluong, source.dongia, source.giamgia, source.thanhtien);
GO

MERGE dbo.tbldondathang AS target
USING (VALUES
    (N'DDH001', 'NV02', CONVERT(date,'2026-06-10'), 'KH01', 20000000, 0, 118500000),
    (N'DDH002', 'NV02', CONVERT(date,'2026-06-12'), 'KH02', 15000000, 0, 110000000),
    (N'DDH003', 'NV02', CONVERT(date,'2026-06-15'), 'KH03', 30000000, 0, 117000000)
) AS source(soddh, manv, ngaymua, makhach, datcoc, thue, tongtien)
ON target.soddh = source.soddh
WHEN MATCHED THEN UPDATE SET manv = source.manv, ngaymua = source.ngaymua, makhach = source.makhach, datcoc = source.datcoc, thue = source.thue, tongtien = source.tongtien
WHEN NOT MATCHED THEN INSERT(soddh, manv, ngaymua, makhach, datcoc, thue, tongtien) VALUES(source.soddh, source.manv, source.ngaymua, source.makhach, source.datcoc, source.thue, source.tongtien);
GO

MERGE dbo.tblchitietddh AS target
USING (VALUES
    (N'DDH001', 'MH01', 2, 0, 68000000),
    (N'DDH001', 'MH02', 1, 0, 50500000),
    (N'DDH002', 'MH06', 2, 0, 64000000),
    (N'DDH002', 'MH04', 1, 0, 46000000),
    (N'DDH003', 'MH03', 1, 0, 69000000),
    (N'DDH003', 'MH08', 1, 0, 48000000)
) AS source(soddh, mahang, soluong, giamgia, thanhtien)
ON target.soddh = source.soddh AND target.mahang = source.mahang
WHEN MATCHED THEN UPDATE SET soluong = source.soluong, giamgia = source.giamgia, thanhtien = source.thanhtien
WHEN NOT MATCHED THEN INSERT(soddh, mahang, soluong, giamgia, thanhtien) VALUES(source.soddh, source.mahang, source.soluong, source.giamgia, source.thanhtien);
GO

-- Cố định tồn kho mẫu để không phụ thuộc trigger. App hiện đang tự cập nhật tồn kho khi thao tác hóa đơn.
UPDATE dbo.tbldmhang SET soluong = CASE mahang
    WHEN 'MH01' THEN 8 WHEN 'MH02' THEN 5 WHEN 'MH03' THEN 3 WHEN 'MH04' THEN 4
    WHEN 'MH05' THEN 3 WHEN 'MH06' THEN 6 WHEN 'MH07' THEN 3 WHEN 'MH08' THEN 4
    ELSE soluong END
WHERE mahang IN ('MH01','MH02','MH03','MH04','MH05','MH06','MH07','MH08');
GO

-- =============================================================
-- 7. SEED ROLE, PERMISSION, USER MẶC ĐỊNH
-- =============================================================
MERGE dbo.tblpermissions AS target
USING (VALUES
    (N'ManageEmployees', N'Quản lý nhân viên', N'Thêm, sửa, xóa công việc và nhân viên'),
    (N'ManageCatalog', N'Quản lý danh mục chung', N'Quản lý dữ liệu danh mục nền'),
    (N'ManageCustomers', N'Quản lý khách hàng', N'Thêm, sửa, xóa khách hàng'),
    (N'ManageSuppliers', N'Quản lý nhà cung cấp', N'Thêm, sửa, xóa nhà cung cấp'),
    (N'ManageProducts', N'Quản lý hàng hóa', N'Thêm, sửa, xóa xe máy và thuộc tính'),
    (N'SalesInvoice', N'Lập hóa đơn bán', N'Tạo và quản lý đơn đặt hàng/hóa đơn bán'),
    (N'PurchaseInvoice', N'Lập hóa đơn nhập', N'Tạo và quản lý hóa đơn nhập'),
    (N'Search', N'Tìm kiếm dữ liệu', N'Tra cứu hàng hóa, khách hàng, hóa đơn'),
    (N'Reports', N'Xem báo cáo', N'Xem báo cáo bán hàng, nhập hàng và kinh doanh'),
    (N'AiAssistant', N'Sử dụng trợ lý AI', N'Xem gợi ý phân tích dữ liệu bán xe'),
    (N'UserAdmin', N'Quản trị tài khoản', N'Tạo, khóa, mở khóa và reset tài khoản'),
    (N'AuditLog', N'Xem nhật ký hệ thống', N'Xem lịch sử đăng nhập, reset mật khẩu và thao tác quản trị')
) AS source(permissionkey, displayname, description)
ON target.permissionkey = source.permissionkey
WHEN MATCHED THEN UPDATE SET displayname = source.displayname, description = source.description
WHEN NOT MATCHED THEN INSERT(permissionkey, displayname, description) VALUES(source.permissionkey, source.displayname, source.description);
GO

MERGE dbo.tblroles AS target
USING (VALUES
    (N'Administrator', N'Quản trị hệ thống', N'Toàn quyền hệ thống', 1),
    (N'Manager', N'Quản lý cửa hàng', N'Quản lý nghiệp vụ, không quản trị tài khoản', 1),
    (N'Sales', N'Nhân viên bán hàng', N'Bán hàng, khách hàng, tìm kiếm và báo cáo', 1),
    (N'Warehouse', N'Thủ kho', N'Nhập hàng, nhà cung cấp, hàng hóa, tìm kiếm và báo cáo', 1),
    (N'Viewer', N'Chỉ xem báo cáo', N'Chỉ tra cứu và xem báo cáo', 1)
) AS source(rolename, displayname, description, isbuiltin)
ON target.rolename = source.rolename
WHEN MATCHED THEN UPDATE SET displayname = source.displayname, description = source.description, isbuiltin = source.isbuiltin
WHEN NOT MATCHED THEN INSERT(rolename, displayname, description, isbuiltin) VALUES(source.rolename, source.displayname, source.description, source.isbuiltin);
GO

-- Reset mapping quyền built-in rồi seed lại để đồng bộ phiên bản mới.
DELETE rp
FROM dbo.tblrolepermissions rp
INNER JOIN dbo.tblroles r ON rp.roleid = r.roleid
WHERE r.rolename IN (N'Administrator', N'Manager', N'Sales', N'Warehouse', N'Viewer');
GO

INSERT INTO dbo.tblrolepermissions(roleid, permissionkey)
SELECT r.roleid, p.permissionkey
FROM dbo.tblroles r
CROSS JOIN dbo.tblpermissions p
WHERE r.rolename = N'Administrator'
  AND NOT EXISTS (SELECT 1 FROM dbo.tblrolepermissions x WHERE x.roleid = r.roleid AND x.permissionkey = p.permissionkey);
GO

INSERT INTO dbo.tblrolepermissions(roleid, permissionkey)
SELECT r.roleid, v.permissionkey
FROM dbo.tblroles r
CROSS APPLY (VALUES
    (N'ManageCatalog'), (N'ManageCustomers'), (N'ManageSuppliers'), (N'ManageProducts'),
    (N'SalesInvoice'), (N'PurchaseInvoice'), (N'Search'), (N'Reports'), (N'AiAssistant')
) v(permissionkey)
WHERE r.rolename = N'Manager'
  AND NOT EXISTS (SELECT 1 FROM dbo.tblrolepermissions x WHERE x.roleid = r.roleid AND x.permissionkey = v.permissionkey);
GO

INSERT INTO dbo.tblrolepermissions(roleid, permissionkey)
SELECT r.roleid, v.permissionkey
FROM dbo.tblroles r
CROSS APPLY (VALUES
    (N'ManageCustomers'), (N'SalesInvoice'), (N'Search'), (N'Reports'), (N'AiAssistant')
) v(permissionkey)
WHERE r.rolename = N'Sales'
  AND NOT EXISTS (SELECT 1 FROM dbo.tblrolepermissions x WHERE x.roleid = r.roleid AND x.permissionkey = v.permissionkey);
GO

INSERT INTO dbo.tblrolepermissions(roleid, permissionkey)
SELECT r.roleid, v.permissionkey
FROM dbo.tblroles r
CROSS APPLY (VALUES
    (N'ManageSuppliers'), (N'ManageProducts'), (N'PurchaseInvoice'), (N'Search'), (N'Reports')
) v(permissionkey)
WHERE r.rolename = N'Warehouse'
  AND NOT EXISTS (SELECT 1 FROM dbo.tblrolepermissions x WHERE x.roleid = r.roleid AND x.permissionkey = v.permissionkey);
GO

INSERT INTO dbo.tblrolepermissions(roleid, permissionkey)
SELECT r.roleid, v.permissionkey
FROM dbo.tblroles r
CROSS APPLY (VALUES (N'Search'), (N'Reports')) v(permissionkey)
WHERE r.rolename = N'Viewer'
  AND NOT EXISTS (SELECT 1 FROM dbo.tblrolepermissions x WHERE x.roleid = r.roleid AND x.permissionkey = v.permissionkey);
GO

-- Tài khoản mặc định. Hash PBKDF2-SHA256, 160000 iterations, tương thích AuthenticationService.
-- Passwords: admin/Admin@12345, manager/Manager@12345, sales/Sales@12345, warehouse/Warehouse@12345, viewer/Viewer@12345
MERGE dbo.tblusers AS target
USING (VALUES
    (N'admin', N'Quản trị viên', N'uLvvkuEUkmbyfzgOkaBVDzHbiF/TDYw/+cHiI+pHGFc=', N'QnSHdfeP3gNYQIhRYO+DZA==', 160000, N'Administrator', 1),
    (N'manager', N'Quản lý cửa hàng', N'kMM0A3HP6/0zFko2RP+kND9npOBmgM6N+HxxVcQh6lg=', N'i/B9FUJj+MMDrVdREb6L2Q==', 160000, N'Manager', 1),
    (N'sales', N'Nhân viên bán hàng', N'aPJbjXAewMV0r15rqP1yQgmpRyKJftoU0APRyQL+xe4=', N'ayHTZZdm2WkboGaeDFqpKg==', 160000, N'Sales', 1),
    (N'warehouse', N'Nhân viên kho', N'MqVtcuoLIDEOPZybQaftQTdT9Hyt7adfclQ67VLqtcY=', N'+spwbRWHR8xihXP1NGOW7w==', 160000, N'Warehouse', 1),
    (N'viewer', N'Tài khoản xem báo cáo', N'LhW5l2Z/cx5GoF/o0cxEmEXgdWJg5/czyLyzkpQSfyo=', N'IQ5bwn8qLFozRZ22WfzFug==', 160000, N'Viewer', 1)
) AS source(username, displayname, passwordhash, passwordsalt, passworditerations, rolename, isactive)
ON target.username = source.username
WHEN MATCHED THEN UPDATE SET
    displayname = source.displayname,
    roleid = (SELECT roleid FROM dbo.tblroles WHERE rolename = source.rolename),
    isactive = source.isactive
WHEN NOT MATCHED THEN INSERT(username, displayname, passwordhash, passwordsalt, passworditerations, roleid, isactive, failedlogincount, lockoutendat, mustchangepassword, passwordchangedat)
VALUES(source.username, source.displayname, source.passwordhash, source.passwordsalt, source.passworditerations, (SELECT roleid FROM dbo.tblroles WHERE rolename = source.rolename), source.isactive, 0, NULL, 0, GETDATE());
GO

IF NOT EXISTS (SELECT 1 FROM dbo.tblauditlog WHERE eventtype = N'DatabaseSeeded')
BEGIN
    INSERT INTO dbo.tblauditlog(eventtype, username, userid, detail)
    VALUES(N'DatabaseSeeded', N'system', NULL, N'Database Complete seed executed.');
END
GO

-- =============================================================
-- 8. VIEW BÁO CÁO/TRA CỨU TIỆN ÍCH
-- =============================================================
CREATE OR ALTER VIEW dbo.vwDanhMucHangDayDu AS
SELECT
    h.mahang, h.tenhang, h.namsx, h.dungtichbinhxang, h.thoigianbaohanh,
    h.soluong, h.dongianhap, h.dongiaban,
    l.tenloai, sx.tenhangsx, m.tenmau, p.tenphanh, dc.tendongco, nsx.tennuocsx, tt.tentt
FROM dbo.tbldmhang h
LEFT JOIN dbo.tbltheloai l ON h.maloai = l.maloai
LEFT JOIN dbo.tblhangsx sx ON h.mahangsx = sx.mahangsx
LEFT JOIN dbo.tblmausac m ON h.mamau = m.mamau
LEFT JOIN dbo.tblphanhxe p ON h.maphanh = p.maphanh
LEFT JOIN dbo.tbldongco dc ON h.madongco = dc.madongco
LEFT JOIN dbo.tblnuocsx nsx ON h.manuocsx = nsx.manuocsx
LEFT JOIN dbo.tbltinhtrang tt ON h.matt = tt.matt;
GO

CREATE OR ALTER VIEW dbo.vwTonKhoCanhBao AS
SELECT mahang, tenhang, soluong, dongianhap, dongiaban,
       CASE WHEN soluong = 0 THEN N'Hết hàng'
            WHEN soluong <= 3 THEN N'Sắp hết'
            ELSE N'Đủ hàng' END AS trangthaitonkho
FROM dbo.tbldmhang;
GO

CREATE OR ALTER VIEW dbo.vwBaoCaoBanHang AS
SELECT ddh.soddh, ddh.ngaymua, kh.tenkhach, nv.tennv,
       ct.mahang, h.tenhang, ct.soluong, ct.giamgia, ct.thanhtien, ddh.datcoc, ddh.thue, ddh.tongtien
FROM dbo.tbldondathang ddh
INNER JOIN dbo.tblchitietddh ct ON ddh.soddh = ct.soddh
INNER JOIN dbo.tbldmhang h ON ct.mahang = h.mahang
LEFT JOIN dbo.tblkhachhang kh ON ddh.makhach = kh.makhach
LEFT JOIN dbo.tblnhanvien nv ON ddh.manv = nv.manv;
GO

CREATE OR ALTER VIEW dbo.vwBaoCaoNhapHang AS
SELECT hdn.sohdn, hdn.ngaynhap, ncc.tenncc, nv.tennv,
       ct.mahang, h.tenhang, ct.soluong, ct.dongia, ct.giamgia, ct.thanhtien, hdn.tongtien
FROM dbo.tblhoadonnhap hdn
INNER JOIN dbo.tblchitiethdn ct ON hdn.sohdn = ct.sohdn
INNER JOIN dbo.tbldmhang h ON ct.mahang = h.mahang
LEFT JOIN dbo.tblnhacungcap ncc ON hdn.mancc = ncc.mancc
LEFT JOIN dbo.tblnhanvien nv ON hdn.manv = nv.manv;
GO

CREATE OR ALTER VIEW dbo.vwTaiKhoanVaQuyen AS
SELECT u.userid, u.username, u.displayname, r.rolename, r.displayname AS roledisplayname,
       u.isactive, u.failedlogincount, u.lockoutendat, u.mustchangepassword, u.lastloginat,
       p.permissionkey, p.displayname AS permissiondisplayname
FROM dbo.tblusers u
INNER JOIN dbo.tblroles r ON u.roleid = r.roleid
LEFT JOIN dbo.tblrolepermissions rp ON r.roleid = rp.roleid
LEFT JOIN dbo.tblpermissions p ON rp.permissionkey = p.permissionkey;
GO

-- =============================================================
-- 9. HEALTH CHECK PROCEDURE
-- =============================================================
CREATE OR ALTER PROCEDURE dbo.sp_QLXeMay_DatabaseHealthCheck
AS
BEGIN
    SET NOCOUNT ON;
    SELECT N'Tables' AS [GroupName], COUNT(*) AS [Count]
    FROM sys.tables
    WHERE name IN (N'tblcongviec',N'tblnhanvien',N'tblkhachhang',N'tblnhacungcap',N'tblmausac',N'tblhangsx',N'tbldongco',N'tblnuocsx',N'tbltheloai',N'tbltinhtrang',N'tblphanhxe',N'tbldmhang',N'tblhoadonnhap',N'tblchitiethdn',N'tbldondathang',N'tblchitietddh',N'tblroles',N'tblpermissions',N'tblrolepermissions',N'tblusers',N'tblauditlog')
    UNION ALL SELECT N'Products', COUNT(*) FROM dbo.tbldmhang
    UNION ALL SELECT N'Customers', COUNT(*) FROM dbo.tblkhachhang
    UNION ALL SELECT N'PurchaseInvoices', COUNT(*) FROM dbo.tblhoadonnhap
    UNION ALL SELECT N'SalesInvoices', COUNT(*) FROM dbo.tbldondathang
    UNION ALL SELECT N'Users', COUNT(*) FROM dbo.tblusers
    UNION ALL SELECT N'Permissions', COUNT(*) FROM dbo.tblpermissions;
END
GO

EXEC dbo.sp_QLXeMay_DatabaseHealthCheck;
GO

PRINT N'QLXeMay database complete: tạo/migrate database btl thành công.';
GO
