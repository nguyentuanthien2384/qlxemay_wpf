# Test plan QLXeMay WPF Pro

## 1. Build và smoke test

1. Chạy `tools\verify-source.ps1` trên Windows 10/11 có .NET SDK 9 và SQL Server Express.
2. Mở app, đăng nhập lần lượt bằng `admin`, `manager`, `sales`, `warehouse`, `viewer`.
3. Kiểm tra form chính bám layout sidebar trong thư mục `giao_dien`.

## 2. Auth/security

- Sai mật khẩu 1-4 lần: app báo lỗi và tăng counter.
- Sai lần thứ 5: user bị khóa tạm 15 phút.
- Admin mở `Quản trị TK`, chọn user, bấm `Mở khóa`.
- Admin reset mật khẩu tạm; user đăng nhập bằng mật khẩu tạm và bị buộc đổi mật khẩu.
- Mật khẩu mới yếu phải bị từ chối theo policy: tối thiểu 10 ký tự, chữ hoa, chữ thường, số, ký tự đặc biệt.
- Tài khoản inactive không được đăng nhập.
- `Nhật ký hệ thống` phải ghi login success/fail, lockout, unlock, reset password, đổi mật khẩu.

## 3. Phân quyền

- Administrator: thấy và mở được toàn bộ chức năng.
- Manager: quản lý nghiệp vụ nhưng không quản trị tài khoản/nhật ký.
- Sales: bán hàng, khách hàng, tìm kiếm, báo cáo, AI.
- Warehouse: nhập hàng, nhà cung cấp, hàng hóa, tìm kiếm, báo cáo.
- Viewer: chỉ tìm kiếm và báo cáo.

## 4. Nghiệp vụ

- Danh mục: thêm, sửa, xóa, làm mới nhân viên, khách hàng, nhà cung cấp, xe máy và các danh mục phụ.
- Nhập hàng: tạo hóa đơn nhập, thêm dòng, tồn kho tăng, xóa dòng rollback tồn kho.
- Bán hàng: tạo hóa đơn bán, kiểm tra tồn kho, thêm dòng, tồn kho giảm, thuế/đặt cọc/tổng tiền đúng.
- Tìm kiếm: tìm theo xe, khách hàng, hóa đơn nhập, đơn đặt hàng.
- Báo cáo: bán hàng, nhập hàng, kết quả kinh doanh, top sản phẩm.
- AI Assistant: các nút phân tích không crash khi dữ liệu trống.

## 5. UI/UX

- Resize cửa sổ xuống min size; form không bị clip nghiêm trọng.
- Tab/Shift+Tab điều hướng được input chính.
- DataGrid đọc được, có scroll, không tự thêm dòng trống.
- Caps Lock warning và hiện/ẩn mật khẩu hoạt động trên login/register/change password.

## 6. Release checklist

- Backup database `btl`.
- Chạy `Database\CreateDatabase.sql` hoặc để app auto-migrate security schema.
- Chạy smoke test auth + hóa đơn trên database test.
- Publish bằng `FolderProfile.pubxml` hoặc lệnh trong `HUONG_DAN_CHAY_WPF.md`.
- Copy folder publish sang máy người dùng và cấu hình `QLXEMAY_CONNECTION_STRING` nếu cần.
