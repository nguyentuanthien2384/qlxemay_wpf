# Hướng Dẫn Chạy Dự Án QLXeMay WPF

## 1. Trạng Thái Dự Án

Dự án đã được nâng cấp theo hướng WPF/C# chuyên nghiệp hơn:

- Có màn hình đăng nhập trước khi vào hệ thống.
- Có phân quyền theo vai trò: quản trị, quản lý, bán hàng, thủ kho, chỉ xem báo cáo.
- Mật khẩu được lưu bằng PBKDF2 SHA-256 kèm salt riêng, không lưu mật khẩu rõ trong database.
- Màn hình chính, trợ lý AI, tìm kiếm, báo cáo, danh mục và hóa đơn dùng `ICommand`, `RelayCommand`, ViewModel và service riêng.
- Data access dùng `SqlParameter`, mở connection theo từng thao tác và có transaction cho nhập/bán hàng.
- Có logging qua `Infrastructure/AppLogger.cs`, mặc định ghi vào `%LOCALAPPDATA%\QLXeMay\Logs`.
- Có project test `QLXeMay.Tests` kiểm tra công thức tiền, thuế, đặt cọc và tồn kho.

## 2. Yêu Cầu Môi Trường

- Windows 10/11.
- Visual Studio 2022.
- .NET 9 SDK, hoặc đổi `TargetFramework` trong `QLXeMay.csproj` về phiên bản bạn đang cài, ví dụ `net8.0-windows`.
- SQL Server Express.
- Database tên `btl`.

## 3. Tạo Database

Mở SQL Server Management Studio và chạy:

```text
Database/CreateDatabase.sql
```

Khi ứng dụng khởi động, app cũng tự đảm bảo các bảng bảo mật sau tồn tại:

- `tblroles`
- `tblpermissions`
- `tblrolepermissions`
- `tblusers`

## 4. Tài Khoản Mặc Định

Ứng dụng tự tạo các tài khoản mẫu khi chạy lần đầu:

| Tài khoản | Mật khẩu | Vai trò |
| --- | --- | --- |
| `admin` | `Admin@123` | Quản trị hệ thống |
| `manager` | `Manager@123` | Quản lý cửa hàng |
| `sales` | `Sales@123` | Nhân viên bán hàng |
| `warehouse` | `Warehouse@123` | Thủ kho |
| `viewer` | `Viewer@123` | Chỉ xem báo cáo |

Quyền chính:

- `admin`: toàn quyền, bao gồm quản trị tài khoản.
- `manager`: quản lý nghiệp vụ, xem báo cáo, dùng AI, không quản trị tài khoản.
- `sales`: bán hàng, khách hàng, tìm kiếm, báo cáo, AI.
- `warehouse`: nhập hàng, nhà cung cấp, hàng hóa, tìm kiếm, báo cáo.
- `viewer`: chỉ tìm kiếm và báo cáo.

Sau khi đăng nhập bằng `admin`, mở `Tài khoản > Quản trị tài khoản` để tạo user mới, đổi vai trò, khóa/mở tài khoản hoặc reset mật khẩu.

## 5. Cấu Hình Chuỗi Kết Nối

Mặc định app dùng:

```text
Data Source=.\SQLEXPRESS;Initial Catalog=btl;Integrated Security=True;Encrypt=False
```

Nếu máy bạn dùng SQL Server khác, đặt biến môi trường:

```powershell
$env:QLXEMAY_CONNECTION_STRING="Data Source=.\SQLEXPRESS;Initial Catalog=btl;Integrated Security=True;Encrypt=False"
```

Hoặc sửa giá trị mặc định trong `Class/Function.cs`.

Có thể đổi thư mục log bằng biến môi trường:

```powershell
$env:QLXEMAY_LOG_DIR="D:\Logs\QLXeMay"
```

## 6. Build Và Chạy

Trong thư mục dự án:

```powershell
dotnet restore
dotnet build QLXeMay.sln
dotnet run --project QLXeMay.csproj
dotnet run --project QLXeMay.Tests\QLXeMay.Tests.csproj
```

Trong Visual Studio:

1. Mở `QLXeMay.sln`.
2. Chọn project `QLXeMay` làm Startup Project.
3. Restore NuGet nếu Visual Studio hỏi.
4. Nhấn `F5` hoặc `Ctrl + F5`.

## 7. Ghi Chú Kỹ Thuật

Phân quyền được kiểm tra ở `MainWindowViewModel` trước khi mở từng chức năng. Các nút/menu không có quyền sẽ bị disable. Mật khẩu chỉ được kiểm tra qua hash trong `AuthenticationService`, không so sánh chuỗi rõ trong database.
