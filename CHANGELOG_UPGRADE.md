# CHANGELOG_UPGRADE

## Mục tiêu

Nâng cấp project QLXeMay WPF theo mẫu trong folder `giao_dien` và bổ sung các chức năng để dự án có cấu trúc gần với một ứng dụng nội bộ chuyên nghiệp hơn.

## Các nhóm thay đổi chính

### 1. Giao diện theo mẫu `giao_dien`

- Thiết kế lại `MainWindow.xaml` theo layout sidebar hồng bên trái và vùng nội dung xanh bên phải.
- Thiết kế lại `LoginWindow.xaml` theo tông hồng giống mockup đăng nhập.
- Thiết kế lại các màn hình nghiệp vụ chính: danh mục, tìm kiếm, báo cáo, hóa đơn nhập, hóa đơn bán, AI assistant, quản trị tài khoản.
- Chuyển nhiều window sang `ResizeMode=CanResize`, có `MinWidth/MinHeight` để tránh vỡ layout.
- Đồng bộ màu nền, border, button và DataGrid theo palette hồng/xanh trong ảnh mẫu.

### 2. Đăng nhập, đăng ký và bảo mật tài khoản

- Thêm model `AuthenticationResult` và `AuthFailureReason` để trả về lỗi đăng nhập rõ ràng.
- Thêm đăng ký tài khoản qua `RegisterWindow`; tài khoản mới mặc định chưa active và cần admin duyệt.
- Thêm `ChangePasswordWindow` cho luồng đổi mật khẩu bắt buộc.
- Thêm chính sách mật khẩu mạnh: tối thiểu 10 ký tự, chữ hoa, chữ thường, số và ký tự đặc biệt.
- Thêm lockout 15 phút sau 5 lần đăng nhập sai.
- Thêm trường `lockoutendat`, `mustchangepassword`, `passwordchangedat` trong `tblusers`.
- Reset password bởi admin sẽ đặt mật khẩu tạm và bắt buộc user đổi ở lần đăng nhập tiếp theo.
- Thêm chức năng mở khóa tài khoản trong màn hình quản trị tài khoản.
- Thêm bảng `tblauditlog` để ghi sự kiện bảo mật.

### 3. Quản trị người dùng

- Màn hình quản trị tài khoản hiển thị trạng thái tài khoản, lockout, số lần đăng nhập sai, trạng thái phải đổi mật khẩu.
- Bổ sung nút: Thêm, Sửa, Reset, Mở khóa, Xóa form, Làm mới.
- Tài khoản tự đăng ký được admin kích hoạt bằng cách chọn user, bật `Tài khoản hoạt động`, chọn role phù hợp rồi cập nhật.

### 4. Session management

- Thêm timeout phiên làm việc mặc định 20 phút bằng `DispatcherTimer`.
- Có thể cấu hình bằng biến môi trường `QLXEMAY_IDLE_TIMEOUT_MINUTES`.

### 5. Database migration

- `AuthenticationService.EnsureSecuritySchema()` tự bổ sung cột bảo mật nếu DB cũ chưa có.
- `Database/CreateDatabase.sql` đã được cập nhật để tạo đầy đủ schema mới.

## Lưu ý build

Môi trường tạo bản nâng cấp không có .NET SDK/Windows WPF runtime nên chưa thể build/chạy end-to-end tại đây. Cần build trên Windows bằng Visual Studio 2022 hoặc .NET SDK tương ứng.

Lệnh kiểm tra trên Windows:

```powershell
dotnet restore
dotnet build QLXeMay.sln -c Release
dotnet run --project QLXeMay.csproj
```

## Bổ sung trong bản hoàn thiện tiếp theo

### 6. Nhật ký hệ thống

- Thêm `AuditLogWindow`, `AuditLogWindowViewModel`, `AuditLogService`, `AuditLogEntry`.
- Thêm quyền `AuditLog`; mặc định chỉ `Administrator` được mở.
- Màn hình nhật ký cho phép lọc theo ngày, từ khóa và số dòng.
- Nhật ký hiển thị các sự kiện đăng nhập, đăng nhập sai, khóa tài khoản, mở khóa, reset mật khẩu, đổi mật khẩu và đăng ký.

### 7. Kiểm thử và phát hành

- Tách password policy thành `Domain/PasswordPolicy.cs` để kiểm thử độc lập.
- Mở rộng `QLXeMay.Tests` với test password policy.
- Thêm `docs/TEST_PLAN.md` làm checklist bàn giao.
- Thêm `tools/verify-source.ps1` để restore, build, chạy test và publish trên Windows.
- Thêm publish profile `Properties/PublishProfiles/FolderProfile.pubxml`.

Lệnh kiểm tra đầy đủ trên Windows:

```powershell
.\tools\verify-source.ps1
```
