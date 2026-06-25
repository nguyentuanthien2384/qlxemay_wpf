# Các phần đã cải tiến UI/UX

- Chuẩn hóa tiêu đề cột DataGrid từ tên cột kỹ thuật (`manv`, `tennv`, `mahang`, `soddh`...) sang tiếng Việt chuyên nghiệp.
- Áp dụng chuẩn header DataGrid cho danh mục, bán hàng, nhập hàng, tìm kiếm, báo cáo và xuất Excel.
- Căn lại form danh mục bằng panel rộng hơn, label/input đều hàng, DatePicker/ComboBox/TextBox đồng bộ chiều cao.
- Căn lại bộ lọc báo cáo và nhật ký hệ thống bằng Grid thay vì bố trí rời rạc.
- Chuẩn hóa tên trường: mã khách hàng, mã nhà cung cấp, mã xe, tên xe, năm sản xuất, hãng sản xuất, nước sản xuất, số điện thoại...
- Cải tiến giao diện quản trị tài khoản: tên màn hình, nút thao tác, tiêu đề cột và thông tin mật khẩu rõ ràng hơn.
- Chỉnh DockPanel biểu đồ doanh thu để dòng lợi nhuận nằm đúng bên phải tiêu đề.
- Dọn bản nén nguồn, loại bỏ thư mục build `bin/obj` và `.git`.

Lưu ý: bản chỉnh sửa đã được kiểm tra cú pháp XML/XAML trong môi trường này. Môi trường hiện tại không có .NET SDK nên chưa chạy được `dotnet build`; hãy build lại bằng Visual Studio/.NET SDK trên máy Windows.
