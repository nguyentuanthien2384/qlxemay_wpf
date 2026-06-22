-- =====================================================
-- HỆ THỐNG QUẢN LÝ CỬA HÀNG BÁN XE MÁY
-- Script tạo cơ sở dữ liệu
-- =====================================================

-- Tạo database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'btl')
BEGIN
    CREATE DATABASE btl;
END
GO

USE btl;
GO

-- =====================================================
-- BẢNG CÔNG VIỆC
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblcongviec' AND xtype='U')
CREATE TABLE tblcongviec (
    macv CHAR(10) PRIMARY KEY,
    tencv NVARCHAR(50) NOT NULL,
    luongthang DECIMAL(18,2) NOT NULL
);
GO

-- =====================================================
-- BẢNG NHÂN VIÊN
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblnhanvien' AND xtype='U')
CREATE TABLE tblnhanvien (
    manv CHAR(10) PRIMARY KEY,
    tennv NVARCHAR(50) NOT NULL,
    gioitinh NVARCHAR(4) NOT NULL,
    ngaysinh DATE NOT NULL,
    sdt VARCHAR(15) NOT NULL,
    diachi NVARCHAR(50) NOT NULL,
    macv CHAR(10) FOREIGN KEY REFERENCES tblcongviec(macv)
);
GO

-- =====================================================
-- BẢNG KHÁCH HÀNG
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblkhachhang' AND xtype='U')
CREATE TABLE tblkhachhang (
    makhach CHAR(10) PRIMARY KEY,
    tenkhach NVARCHAR(50) NOT NULL,
    diachi NVARCHAR(50) NOT NULL,
    sdt VARCHAR(15) NOT NULL
);
GO

-- =====================================================
-- BẢNG NHÀ CUNG CẤP
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblnhacungcap' AND xtype='U')
CREATE TABLE tblnhacungcap (
    mancc CHAR(10) PRIMARY KEY,
    tenncc NVARCHAR(50) NOT NULL,
    diachi NVARCHAR(50) NOT NULL,
    sdt VARCHAR(15) NOT NULL
);
GO

-- =====================================================
-- BẢNG MÀU SẮC
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblmausac' AND xtype='U')
CREATE TABLE tblmausac (
    mamau CHAR(10) PRIMARY KEY,
    tenmau NVARCHAR(30) NOT NULL
);
GO

-- =====================================================
-- BẢNG HÃNG SẢN XUẤT
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblhangsx' AND xtype='U')
CREATE TABLE tblhangsx (
    mahangsx CHAR(10) PRIMARY KEY,
    tenhangsx NVARCHAR(50) NOT NULL
);
GO

-- =====================================================
-- BẢNG ĐỘNG CƠ
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbldongco' AND xtype='U')
CREATE TABLE tbldongco (
    madongco CHAR(10) PRIMARY KEY,
    tendongco NVARCHAR(50) NOT NULL
);
GO

-- =====================================================
-- BẢNG NƯỚC SẢN XUẤT
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblnuocsx' AND xtype='U')
CREATE TABLE tblnuocsx (
    manuocsx CHAR(10) PRIMARY KEY,
    tennuocsx NVARCHAR(30) NOT NULL
);
GO

-- =====================================================
-- BẢNG THỂ LOẠI
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbltheloai' AND xtype='U')
CREATE TABLE tbltheloai (
    maloai CHAR(10) PRIMARY KEY,
    tenloai NVARCHAR(50) NOT NULL
);
GO

-- =====================================================
-- BẢNG TÌNH TRẠNG
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbltinhtrang' AND xtype='U')
CREATE TABLE tbltinhtrang (
    matt CHAR(10) PRIMARY KEY,
    tentt NVARCHAR(20) NOT NULL
);
GO

-- =====================================================
-- BẢNG PHANH XE
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblphanhxe' AND xtype='U')
CREATE TABLE tblphanhxe (
    maphanh CHAR(10) PRIMARY KEY,
    tenphanh NVARCHAR(50) NOT NULL
);
GO

-- =====================================================
-- BẢNG DANH MỤC HÀNG (HÀNG HÓA)
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbldmhang' AND xtype='U')
CREATE TABLE tbldmhang (
    mahang CHAR(10) PRIMARY KEY,
    tenhang NVARCHAR(50) NOT NULL,
    maloai CHAR(10) FOREIGN KEY REFERENCES tbltheloai(maloai),
    mahangsx CHAR(10) FOREIGN KEY REFERENCES tblhangsx(mahangsx),
    mamau CHAR(10) FOREIGN KEY REFERENCES tblmausac(mamau),
    namsx SMALLINT NOT NULL,
    maphanh CHAR(10) FOREIGN KEY REFERENCES tblphanhxe(maphanh),
    madongco CHAR(10) FOREIGN KEY REFERENCES tbldongco(madongco),
    manuocsx CHAR(10) FOREIGN KEY REFERENCES tblnuocsx(manuocsx),
    matt CHAR(10) FOREIGN KEY REFERENCES tbltinhtrang(matt),
    dungtichbinhxang SMALLINT NOT NULL,
    anh NVARCHAR(200),
    thoigianbaohanh SMALLINT NOT NULL DEFAULT 0,
    soluong INT NOT NULL DEFAULT 0,
    dongianhap DECIMAL(18,2) NOT NULL DEFAULT 0,
    dongiaban DECIMAL(18,2) NOT NULL DEFAULT 0
);
GO

-- =====================================================
-- BẢNG HÓA ĐƠN NHẬP
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblhoadonnhap' AND xtype='U')
CREATE TABLE tblhoadonnhap (
    sohdn NVARCHAR(20) PRIMARY KEY,
    manv CHAR(10) FOREIGN KEY REFERENCES tblnhanvien(manv),
    ngaynhap DATE NOT NULL,
    mancc CHAR(10) FOREIGN KEY REFERENCES tblnhacungcap(mancc),
    tongtien DECIMAL(18,2) NOT NULL DEFAULT 0
);
GO

-- =====================================================
-- BẢNG CHI TIẾT HÓA ĐƠN NHẬP
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblchitiethdn' AND xtype='U')
CREATE TABLE tblchitiethdn (
    sohdn NVARCHAR(20) NOT NULL FOREIGN KEY REFERENCES tblhoadonnhap(sohdn),
    mahang CHAR(10) NOT NULL FOREIGN KEY REFERENCES tbldmhang(mahang),
    soluong INT NOT NULL,
    dongia DECIMAL(18,2) NOT NULL,
    giamgia DECIMAL(5,2) DEFAULT 0,
    thanhtien DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (sohdn, mahang)
);
GO

-- =====================================================
-- BẢNG ĐƠN ĐẶT HÀNG (HÓA ĐƠN BÁN)
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tbldondathang' AND xtype='U')
CREATE TABLE tbldondathang (
    soddh NVARCHAR(20) PRIMARY KEY,
    manv CHAR(10) FOREIGN KEY REFERENCES tblnhanvien(manv),
    ngaymua DATE NOT NULL,
    makhach CHAR(10) FOREIGN KEY REFERENCES tblkhachhang(makhach),
    datcoc DECIMAL(18,2) DEFAULT 0,
    thue DECIMAL(18,2) NOT NULL DEFAULT 0,
    tongtien DECIMAL(18,2) NOT NULL DEFAULT 0
);
GO

-- =====================================================
-- BẢNG CHI TIẾT ĐƠN ĐẶT HÀNG
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblchitietddh' AND xtype='U')
CREATE TABLE tblchitietddh (
    soddh NVARCHAR(20) NOT NULL FOREIGN KEY REFERENCES tbldondathang(soddh),
    mahang CHAR(10) NOT NULL FOREIGN KEY REFERENCES tbldmhang(mahang),
    soluong INT NOT NULL,
    giamgia DECIMAL(5,2) DEFAULT 0,
    thanhtien DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (soddh, mahang)
);
GO

-- =====================================================
-- BẢNG VAI TRÒ, QUYỀN VÀ TÀI KHOẢN ĐĂNG NHẬP
-- Mật khẩu mặc định được seed bằng C# để sinh PBKDF2 hash + salt.
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblroles' AND xtype='U')
CREATE TABLE tblroles (
    roleid INT IDENTITY(1,1) PRIMARY KEY,
    rolename NVARCHAR(50) NOT NULL UNIQUE,
    displayname NVARCHAR(100) NOT NULL,
    description NVARCHAR(255) NULL,
    isbuiltin BIT NOT NULL DEFAULT 0
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblpermissions' AND xtype='U')
CREATE TABLE tblpermissions (
    permissionkey NVARCHAR(80) NOT NULL PRIMARY KEY,
    displayname NVARCHAR(120) NOT NULL,
    description NVARCHAR(255) NULL
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblrolepermissions' AND xtype='U')
CREATE TABLE tblrolepermissions (
    roleid INT NOT NULL FOREIGN KEY REFERENCES tblroles(roleid),
    permissionkey NVARCHAR(80) NOT NULL FOREIGN KEY REFERENCES tblpermissions(permissionkey),
    PRIMARY KEY(roleid, permissionkey)
);
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblusers' AND xtype='U')
CREATE TABLE tblusers (
    userid INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(50) NOT NULL UNIQUE,
    displayname NVARCHAR(100) NOT NULL,
    passwordhash NVARCHAR(200) NOT NULL,
    passwordsalt NVARCHAR(200) NOT NULL,
    passworditerations INT NOT NULL,
    roleid INT NOT NULL FOREIGN KEY REFERENCES tblroles(roleid),
    isactive BIT NOT NULL DEFAULT 1,
    failedlogincount INT NOT NULL DEFAULT 0,
    createdat DATETIME NOT NULL DEFAULT GETDATE(),
    lastloginat DATETIME NULL
);
GO

-- =====================================================
-- DỮ LIỆU MẪU
-- =====================================================

-- Công việc
IF NOT EXISTS (SELECT 1 FROM tblcongviec)
BEGIN
INSERT INTO tblcongviec VALUES ('CV01', N'Quản lý', 15000000);
INSERT INTO tblcongviec VALUES ('CV02', N'Nhân viên bán hàng', 8000000);
INSERT INTO tblcongviec VALUES ('CV03', N'Kế toán', 10000000);
INSERT INTO tblcongviec VALUES ('CV04', N'Thủ kho', 7000000);
END
GO

-- Thể loại
IF NOT EXISTS (SELECT 1 FROM tbltheloai)
BEGIN
INSERT INTO tbltheloai VALUES ('TL01', N'Underbone');
INSERT INTO tbltheloai VALUES ('TL02', N'Scooter');
INSERT INTO tbltheloai VALUES ('TL03', N'Sportbike');
INSERT INTO tbltheloai VALUES ('TL04', N'Nakedbike');
INSERT INTO tbltheloai VALUES ('TL05', N'Cruiser');
END
GO

-- Màu sắc
IF NOT EXISTS (SELECT 1 FROM tblmausac)
BEGIN
INSERT INTO tblmausac VALUES ('MS01', N'Đen');
INSERT INTO tblmausac VALUES ('MS02', N'Trắng');
INSERT INTO tblmausac VALUES ('MS03', N'Đỏ');
INSERT INTO tblmausac VALUES ('MS04', N'Xanh');
INSERT INTO tblmausac VALUES ('MS05', N'Bạc');
END
GO

-- Hãng sản xuất
IF NOT EXISTS (SELECT 1 FROM tblhangsx)
BEGIN
INSERT INTO tblhangsx VALUES ('HSX01', N'Honda');
INSERT INTO tblhangsx VALUES ('HSX02', N'Yamaha');
INSERT INTO tblhangsx VALUES ('HSX03', N'Suzuki');
INSERT INTO tblhangsx VALUES ('HSX04', N'VinFast');
INSERT INTO tblhangsx VALUES ('HSX05', N'Piaggio');
END
GO

-- Nước sản xuất
IF NOT EXISTS (SELECT 1 FROM tblnuocsx)
BEGIN
INSERT INTO tblnuocsx VALUES ('NSX01', N'Việt Nam');
INSERT INTO tblnuocsx VALUES ('NSX02', N'Nhật Bản');
INSERT INTO tblnuocsx VALUES ('NSX03', N'Ý');
INSERT INTO tblnuocsx VALUES ('NSX04', N'Trung Quốc');
END
GO

-- Động cơ
IF NOT EXISTS (SELECT 1 FROM tbldongco)
BEGIN
INSERT INTO tbldongco VALUES ('DC01', N'4 thì');
INSERT INTO tbldongco VALUES ('DC02', N'Phun xăng điện tử');
INSERT INTO tbldongco VALUES ('DC03', N'Động cơ điện');
END
GO

-- Tình trạng
IF NOT EXISTS (SELECT 1 FROM tbltinhtrang)
BEGIN
INSERT INTO tbltinhtrang VALUES ('TT01', N'Mới');
INSERT INTO tbltinhtrang VALUES ('TT02', N'Đã qua sử dụng');
END
GO

-- Phanh xe
IF NOT EXISTS (SELECT 1 FROM tblphanhxe)
BEGIN
INSERT INTO tblphanhxe VALUES ('PH01', N'Phanh đĩa');
INSERT INTO tblphanhxe VALUES ('PH02', N'Phanh tang trống');
INSERT INTO tblphanhxe VALUES ('PH03', N'Phanh ABS');
END
GO

-- Nhà cung cấp
IF NOT EXISTS (SELECT 1 FROM tblnhacungcap)
BEGIN
INSERT INTO tblnhacungcap VALUES ('NCC01', N'Honda Việt Nam', N'Vĩnh Phúc', '0241234567');
INSERT INTO tblnhacungcap VALUES ('NCC02', N'Yamaha Motor VN', N'Hà Nội', '0249876543');
END
GO

-- Nhân viên
IF NOT EXISTS (SELECT 1 FROM tblnhanvien)
BEGIN
INSERT INTO tblnhanvien VALUES ('NV01', N'Nguyễn Văn An', N'Nam', '1995-05-15', '0912345678', N'Hà Nội', 'CV01');
INSERT INTO tblnhanvien VALUES ('NV02', N'Trần Thị Bình', N'Nữ', '1998-08-20', '0987654321', N'Hà Nội', 'CV02');
END
GO

-- Khách hàng
IF NOT EXISTS (SELECT 1 FROM tblkhachhang)
BEGIN
INSERT INTO tblkhachhang VALUES ('KH01', N'Lê Văn Cường', N'Hà Nội', '0901234567');
INSERT INTO tblkhachhang VALUES ('KH02', N'Phạm Thị Dung', N'Hải Phòng', '0976543210');
END
GO

-- Hàng hóa mẫu
IF NOT EXISTS (SELECT 1 FROM tbldmhang)
BEGIN
INSERT INTO tbldmhang VALUES ('MH01', N'Honda Vision', 'TL02', 'HSX01', 'MS01', 2024, 'PH01', 'DC02', 'NSX01', 'TT01', 5, N'', 24, 0, 0, 0);
INSERT INTO tbldmhang VALUES ('MH02', N'Yamaha Exciter', 'TL01', 'HSX02', 'MS03', 2024, 'PH01', 'DC02', 'NSX01', 'TT01', 4, N'', 24, 0, 0, 0);
END
GO

-- =====================================================
-- CHUẨN HÓA LẠI DỮ LIỆU MẪU TIẾNG VIỆT
-- Khối này sửa các database đã từng chạy script với encoding sai.
-- =====================================================
UPDATE tblcongviec SET tencv=N'Quản lý' WHERE macv='CV01';
UPDATE tblcongviec SET tencv=N'Nhân viên bán hàng' WHERE macv='CV02';
UPDATE tblcongviec SET tencv=N'Kế toán' WHERE macv='CV03';
UPDATE tblcongviec SET tencv=N'Thủ kho' WHERE macv='CV04';

UPDATE tblmausac SET tenmau=N'Đen' WHERE mamau='MS01';
UPDATE tblmausac SET tenmau=N'Trắng' WHERE mamau='MS02';
UPDATE tblmausac SET tenmau=N'Đỏ' WHERE mamau='MS03';
UPDATE tblmausac SET tenmau=N'Xanh' WHERE mamau='MS04';
UPDATE tblmausac SET tenmau=N'Bạc' WHERE mamau='MS05';

UPDATE tblnuocsx SET tennuocsx=N'Việt Nam' WHERE manuocsx='NSX01';
UPDATE tblnuocsx SET tennuocsx=N'Nhật Bản' WHERE manuocsx='NSX02';
UPDATE tblnuocsx SET tennuocsx=N'Ý' WHERE manuocsx='NSX03';
UPDATE tblnuocsx SET tennuocsx=N'Trung Quốc' WHERE manuocsx='NSX04';

UPDATE tbldongco SET tendongco=N'4 thì' WHERE madongco='DC01';
UPDATE tbldongco SET tendongco=N'Phun xăng điện tử' WHERE madongco='DC02';
UPDATE tbldongco SET tendongco=N'Động cơ điện' WHERE madongco='DC03';

UPDATE tbltinhtrang SET tentt=N'Mới' WHERE matt='TT01';
UPDATE tbltinhtrang SET tentt=N'Đã qua sử dụng' WHERE matt='TT02';

UPDATE tblphanhxe SET tenphanh=N'Phanh đĩa' WHERE maphanh='PH01';
UPDATE tblphanhxe SET tenphanh=N'Phanh tang trống' WHERE maphanh='PH02';
UPDATE tblphanhxe SET tenphanh=N'Phanh ABS' WHERE maphanh='PH03';

UPDATE tblnhacungcap SET tenncc=N'Honda Việt Nam', diachi=N'Vĩnh Phúc' WHERE mancc='NCC01';
UPDATE tblnhacungcap SET tenncc=N'Yamaha Motor VN', diachi=N'Hà Nội' WHERE mancc='NCC02';

UPDATE tblnhanvien SET tennv=N'Nguyễn Văn An', gioitinh=N'Nam', diachi=N'Hà Nội' WHERE manv='NV01';
UPDATE tblnhanvien SET tennv=N'Trần Thị Bình', gioitinh=N'Nữ', diachi=N'Hà Nội' WHERE manv='NV02';

UPDATE tblkhachhang SET tenkhach=N'Lê Văn Cường', diachi=N'Hà Nội' WHERE makhach='KH01';
UPDATE tblkhachhang SET tenkhach=N'Phạm Thị Dung', diachi=N'Hải Phòng' WHERE makhach='KH02';
GO

PRINT N'Tạo cơ sở dữ liệu thành công!';
GO
