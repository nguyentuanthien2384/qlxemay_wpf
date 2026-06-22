# QLXeMay WPF - Database Complete

## File chính

- `CreateDatabase_Complete.sql`: tạo/migrate database `btl`, tạo toàn bộ bảng nghiệp vụ, bảng bảo mật, phân quyền, tài khoản mặc định, dữ liệu mẫu, index, view báo cáo và stored procedure health check.

## Cách chạy bằng SQL Server Management Studio

1. Mở SQL Server Management Studio.
2. Kết nối SQL Server hoặc SQL Server Express.
3. Mở file `CreateDatabase_Complete.sql`.
4. Bấm `Execute`.
5. Sau khi chạy xong, kiểm tra dòng cuối hiển thị: `QLXeMay database complete: tạo/migrate database btl thành công.`

## Cách chạy bằng sqlcmd

```powershell
sqlcmd -S .\SQLEXPRESS -E -i ".\CreateDatabase_Complete.sql"
```

Nếu dùng SQL Server instance khác, đổi `.\SQLEXPRESS` thành server name của bạn, ví dụ `localhost` hoặc `DESKTOP-ABC\SQLEXPRESS`.

## Connection string khuyến nghị cho app

```text
Data Source=.\SQLEXPRESS;Initial Catalog=btl;Integrated Security=True;Encrypt=False
```

Có thể set bằng biến môi trường:

```powershell
$env:QLXEMAY_CONNECTION_STRING="Data Source=.\SQLEXPRESS;Initial Catalog=btl;Integrated Security=True;Encrypt=False"
```

## Tài khoản mặc định

| Username | Password | Role |
|---|---|---|
| admin | Admin@12345 | Administrator |
| manager | Manager@12345 | Manager |
| sales | Sales@12345 | Sales |
| warehouse | Warehouse@12345 | Warehouse |
| viewer | Viewer@12345 | Viewer |

## Bảng chính được tạo

- Danh mục: `tblcongviec`, `tblnhanvien`, `tblkhachhang`, `tblnhacungcap`, `tblmausac`, `tblhangsx`, `tbldongco`, `tblnuocsx`, `tbltheloai`, `tbltinhtrang`, `tblphanhxe`.
- Hàng hóa/hóa đơn: `tbldmhang`, `tblhoadonnhap`, `tblchitiethdn`, `tbldondathang`, `tblchitietddh`.
- Bảo mật: `tblroles`, `tblpermissions`, `tblrolepermissions`, `tblusers`, `tblauditlog`.

## View hỗ trợ báo cáo

- `vwDanhMucHangDayDu`
- `vwTonKhoCanhBao`
- `vwBaoCaoBanHang`
- `vwBaoCaoNhapHang`
- `vwTaiKhoanVaQuyen`

## Health check

Sau khi chạy script, có thể kiểm tra nhanh bằng:

```sql
USE btl;
EXEC dbo.sp_QLXeMay_DatabaseHealthCheck;
```

## Lưu ý

Script có thể chạy trên database `btl` mới hoặc database cũ. Với database đang có dữ liệu thật, nên backup trước khi chạy vì script sẽ seed/cập nhật các dòng dữ liệu mẫu có mã như `MH01`, `NV01`, `KH01`, `admin`.
