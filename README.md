# QLXeMay WPF Pro - Hệ Thống Quản Lý Cửa Hàng Xe Máy

Hệ thống quản lý cửa hàng xe máy chuyên nghiệp được xây dựng trên nền tảng **WPF (C# / .NET 9)** và **SQL Server**, tuân thủ mô hình thiết kế hiện đại, tích hợp các tiêu chuẩn bảo mật doanh nghiệp (RBAC, PBKDF2, Audit Log) và tích hợp **Trợ lý AI Offline** phân tích dữ liệu kinh doanh.

---

## 👥 Thành Viên Thực Hiện

*   **Nguyễn Tuấn Thiền** – MSSV: `23010571`
*   **Đặng Việt Anh** – MSSV: `23010689`

---

## 🌟 Tính Năng Nổi Bật

### 1. Giao Diện & Trải Nghiệm Người Dùng (UI/UX)
*   **Thiết kế hiện đại:** Sidebar điều hướng màu hồng tinh tế kết hợp vùng làm việc nền xanh nhã nhặn, bố cục trực quan, hỗ trợ responsive tốt.
*   **Trải nghiệm đăng nhập mượt mà:** Form đăng nhập/đăng ký hỗ trợ tính năng hiển thị/ẩn mật khẩu, cảnh báo phím **Caps Lock** đang bật, và hiển thị thông báo lỗi chi tiết.
*   **Tự động hóa:** Tự động khóa màn hình hoặc hết hạn phiên làm việc (Session Timeout) sau một khoảng thời gian không tương tác (mặc định 20 phút).

### 2. Bảo Mật Doanh Nghiệp (Security & Audit)
*   **Phân quyền theo vai trò (RBAC):** Định nghĩa 5 vai trò mặc định:
    *   `admin`: Quản trị toàn hệ thống, quản lý tài khoản, xem nhật ký hệ thống.
    *   `manager`: Quản lý nghiệp vụ, xem báo cáo, sử dụng trợ lý AI.
    *   `sales`: Lập hóa đơn bán hàng, tìm kiếm, xem báo cáo bán hàng, dùng AI.
    *   `warehouse`: Lập hóa đơn nhập, quản lý kho, quản lý hàng hóa/nhà cung cấp.
    *   `viewer`: Chỉ được phép tìm kiếm và xem báo cáo.
*   **Mã hóa mật khẩu nâng cao:** Mật khẩu được mã hóa bằng thuật toán **PBKDF2 SHA-256 kèm Salt** ngẫu nhiên riêng biệt cho từng người dùng.
*   **Chính sách mật khẩu nghiêm ngặt:** Yêu cầu tối thiểu 10 ký tự, bao gồm ít nhất 1 chữ hoa, 1 chữ thường, 1 chữ số và 1 ký tự đặc biệt.
*   **Chống tấn công Brute-force:** Tự động khóa tài khoản trong vòng **15 phút** nếu nhập sai mật khẩu quá 5 lần liên tiếp.
*   **Nhật ký hệ thống (Audit Log):** Ghi nhận chi tiết tất cả các sự kiện bảo mật quan trọng (đăng nhập thành công/thất bại, khóa tài khoản, đổi mật khẩu, đăng ký mới,...) giúp quản trị viên dễ dàng theo dõi.

### 3. Trợ Lý AI Offline (Trí Tuệ Nhân Tạo Ngoại Tuyến)
*   Tích hợp **Trợ lý AI hoạt động hoàn toàn offline**, không cần kết nối internet hay sử dụng API Key bên ngoài.
*   Phân tích dữ liệu kinh doanh trực tiếp từ database để đưa ra các đề xuất điều hành:
    *   Tổng quan tình hình kinh doanh hôm nay.
    *   Dự báo doanh thu trong 30 ngày tới.
    *   Phát hiện các sản phẩm tồn kho cao cần đẩy mạnh khuyến mãi.
    *   Gợi ý nhập thêm hàng dựa trên tốc độ bán 90 ngày.
    *   Tư vấn sản phẩm phù hợp với nhu cầu và ngân sách của khách hàng.
    *   Cảnh báo các rủi ro kinh doanh hiện tại.

### 4. Nghiệp Vụ Quản Lý Toàn Diện
*   **Quản lý danh mục:** Nhân viên, Khách hàng, Nhà cung cấp, Xe máy và các danh mục phụ (Màu sắc, Hãng sản xuất, Động cơ, Nước sản xuất, Thể loại, Tình trạng, Phanh xe).
*   **Quản lý giao dịch:** Lập hóa đơn bán hàng, hóa đơn nhập hàng với quy trình tự động cập nhật số lượng tồn kho và cơ chế rollback dữ liệu nếu giao dịch bị lỗi.

---

## 📁 Cấu Trúc Thư Mục & Source Code

Dự án được tổ chức theo mô hình phân lớp rõ ràng, tách biệt giữa Giao diện (WPF / XAML), ViewModel (MVVM Logic), Nghiệp vụ (Services) và Cơ sở dữ liệu:

```text
QLXeMay_WPF/
├── Database/                   # Tập hợp mã nguồn SQL khởi tạo & dữ liệu mẫu
│   ├── CreateDatabase_Complete.sql   # Khởi tạo toàn bộ Database btl, các bảng & phân quyền
│   └── SampleData_Professional.sql    # Dữ liệu mẫu phong phú phục vụ Trợ lý AI phân tích
├── Domain/                     # Các quy tắc nghiệp vụ độc lập & cốt lõi
│   ├── AccessControl.cs        # Logic kiểm tra quyền hạn của các Account
│   ├── InvoiceCalculator.cs    # Tính toán tiền tệ, thuế VAT, đặt cọc cho hóa đơn
│   └── PasswordPolicy.cs       # Định nghĩa chính sách độ mạnh của mật khẩu
├── Models/                     # Khai báo các Model dữ liệu (Data Entities)
│   ├── AuditLogEntry.cs        # Mô hình dòng nhật ký hệ thống
│   ├── AuthenticationResult.cs # Trạng thái đăng nhập (thành công, sai pass, khóa...)
│   ├── InvoiceModels.cs        # Cấu trúc hóa đơn bán hàng & hóa đơn nhập hàng
│   ├── StoreModels.cs          # Mô hình Xe máy, Khách hàng, NCC, Màu sắc...
│   └── UserAccountInfo.cs      # Thông tin tài khoản người dùng & Role
├── Services/                   # Lớp xử lý logic nghiệp vụ & truy cập cơ sở dữ liệu
│   ├── AuthenticationService.cs# Logic Đăng nhập, Đăng ký, PBKDF2 hash, khóa TK sau 5 lần sai
│   ├── AiAssistantService.cs   # Trợ lý ảo AI phân tích kinh doanh ngoại tuyến (offline)
│   ├── AuditLogService.cs      # Dịch vụ ghi chép nhật ký hệ thống
│   ├── CategoryService.cs      # Nghiệp vụ quản lý các danh mục phụ
│   ├── GlobalSearchService.cs  # Tìm kiếm thông minh toàn hệ thống
│   ├── StoreService.cs         # Nghiệp vụ quản lý xe máy, nhà cung cấp, khách hàng
│   ├── SalesInvoiceService.cs  # Nghiệp vụ bán hàng & cập nhật tồn kho (có transaction)
│   └── PurchaseInvoiceService.cs# Nghiệp vụ nhập kho xe máy mới
├── ViewModels/                 # Tầng trung gian MVVM liên kết Data & View (UI Action commands)
│   ├── ViewModelBase.cs        # Lớp cơ sở hỗ trợ PropertyChanged event
│   ├── MainWindowViewModel.cs  # Xử lý điều hướng sidebar, kiểm tra phân quyền hiển thị menu
│   ├── AIAssistantViewModel.cs # Binding dữ liệu & phản hồi từ Trợ lý AI
│   ├── SalesInvoiceViewModel.cs# Xử lý nghiệp vụ lập hóa đơn bán hàng trực quan
│   └── UserAdminWindowViewModel.cs # Quản trị người dùng (kích hoạt, reset pass, phân vai trò)
├── Windows/                    # Chứa giao diện đồ họa người dùng (WPF XAML & Code-behind)
│   ├── MainWindow.xaml         # Cửa sổ chính chứa thanh điều hướng sidebar & vùng hiển thị chính
│   ├── LoginWindow.xaml        # Giao diện Đăng nhập hệ thống (Caps lock, Hiện/Ẩn mật khẩu)
│   ├── RegisterWindow.xaml      # Giao diện Đăng ký tài khoản mới cho cán bộ/nhân viên
│   ├── AIAssistantWindow.xaml  # Hộp thoại chat & phân tích số liệu của Trợ lý AI
│   └── AuditLogWindow.xaml     # Màn hình xem Nhật ký hệ thống dành cho Admin
├── Infrastructure/             # Tiện ích bổ trợ hệ thống
│   ├── AppLogger.cs            # Logging lỗi và vết chạy chương trình ra file vật lý
│   └── RelayCommand.cs         # Hỗ trợ bind các lệnh Command từ View đến ViewModel
├── QLXeMay.Tests/              # Dự án Unit Test kiểm thử tự động
│   └── Program.cs              # Bộ test kiểm định (thuế, tiền tệ, đổi mật khẩu)
├── docs/                       # Tài liệu hướng dẫn sử dụng, setup DB và Test Plan
│   ├── AI_ASSISTANT_GUIDE.md   # Hướng dẫn chi tiết sử dụng Trợ lý AI
│   ├── DATABASE_SETUP.md       # Các bước cài đặt và kiểm tra Database
│   └── TEST_PLAN.md            # Các ca kiểm thử khi bàn giao
└── tools/                      # Các kịch bản chạy tự động
    └── verify-source.ps1       # Script PowerShell tự động restore, build, test & publish
```

---

## 💻 Yêu Cầu Hệ Thống

*   **Hệ điều hành:** Windows 10 hoặc Windows 11.
*   **IDE:** Visual Studio 2022.
*   **SDK:** .NET 9 SDK (hoặc thay đổi target framework trong `QLXeMay.csproj` thành `.NET 8` tùy theo môi trường cài đặt của bạn).
*   **Cơ sở dữ liệu:** SQL Server / SQL Server Express.

---

## 🚀 Hướng Dẫn Cài Đặt & Khởi Chạy

### 1. Thiết lập Database
1.  Mở **SQL Server Management Studio (SSMS)**.
2.  Mở và chạy lần lượt các script sau để khởi tạo database `btl` và nạp dữ liệu mẫu lớn phục vụ demo AI:
    *   `Database/CreateDatabase_Complete.sql`
    *   `Database/SampleData_Professional.sql`

Hoặc bạn có thể dùng `sqlcmd` từ PowerShell:
```powershell
sqlcmd -S .\SQLEXPRESS -E -i ".\Database\CreateDatabase_Complete.sql"
sqlcmd -S .\SQLEXPRESS -E -i ".\Database\SampleData_Professional.sql"
```

### 2. Cấu hình Chuỗi Kết Nối
Mặc định hệ thống sử dụng Connection String sau:
`Data Source=.\SQLEXPRESS;Initial Catalog=btl;Integrated Security=True;Encrypt=False`

Nếu bạn sử dụng SQL Server instance khác, bạn có thể thiết lập biến môi trường:
```powershell
$env:QLXEMAY_CONNECTION_STRING="Data Source=TÊN_SERVER_CỦA_BẠN;Initial Catalog=btl;Integrated Security=True;Encrypt=False"
```

### 3. Tài Khoản Mặc Định
Hệ thống tự động tạo sẵn các tài khoản thử nghiệm sau:

| Tài khoản | Mật khẩu | Vai trò |
| :--- | :--- | :--- |
| `admin` | `Admin@12345` | Quản trị hệ thống (Toàn quyền + Nhật ký) |
| `manager` | `Manager@12345` | Quản lý cửa hàng (Nghiệp vụ + Báo cáo + AI) |
| `sales` | `Sales@12345` | Nhân viên bán hàng (Bán hàng + Báo cáo + AI) |
| `warehouse` | `Warehouse@12345` | Thủ kho (Nhập hàng + Danh mục sản phẩm) |
| `viewer` | `Viewer@12345` | Chỉ xem báo cáo & Tìm kiếm |

### 4. Build và Chạy Ứng Dụng
**Sử dụng Visual Studio 2022:**
1.  Mở file solution `QLXeMay.sln`.
2.  Chọn project `QLXeMay` làm Startup Project.
3.  Nhấn phím `F5` hoặc nút `Start` để bắt đầu.

**Sử dụng CLI (PowerShell):**
```powershell
dotnet restore
dotnet build QLXeMay.sln
dotnet run --project QLXeMay.csproj
```

---

## 🛠️ Công Cụ Xác Minh & Kiểm Thử Tự Động
Để kiểm tra nhanh chất lượng mã nguồn, build, chạy test và đóng gói ứng dụng (Publish) trên máy Windows, bạn chỉ cần thực thi script xác minh tự động:

```powershell
.\tools\verify-source.ps1
```

Script sẽ tự động thực hiện:
1.  Kiểm tra phiên bản .NET SDK hiện tại.
2.  Restore các gói NuGet cần thiết.
3.  Build dự án ở cấu hình `Release`.
4.  Chạy toàn bộ các ca kiểm thử trong project `QLXeMay.Tests`.
5.  Xuất bản gói phần mềm self-contained `win-x64` ra thư mục `publish\win-x64`.

---

## 📝 Tài Liệu Tham Khảo Thêm
*   Tài liệu về Trợ lý AI: [AI_ASSISTANT_GUIDE.md](file:///d:/QLXeMay_WPF/docs/AI_ASSISTANT_GUIDE.md)
*   Hướng dẫn chạy chi tiết: [HUONG_DAN_CHAY_WPF.md](file:///d:/QLXeMay_WPF/HUONG_DAN_CHAY_WPF.md)
*   Kế hoạch kiểm thử: [TEST_PLAN.md](file:///d:/QLXeMay_WPF/docs/TEST_PLAN.md)
