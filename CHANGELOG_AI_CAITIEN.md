# CHANGELOG - Cải tiến Trợ lý AI

## Phiên bản cải tiến

### 1. Giao diện Trợ lý AI

- Thiết kế lại màn hình Trợ lý AI theo dạng dashboard chuyên nghiệp.
- Thêm khu vực phân tích nhanh: Tổng quan điều hành, Dự báo 30 ngày, Gợi ý nhập hàng, Cảnh báo rủi ro, Tồn kho, Doanh thu, Khách hàng, Nhân viên.
- Thêm danh sách câu hỏi mẫu để demo nhanh, bấm vào là AI tự chạy phân tích.
- Cải thiện ô nhập câu hỏi và vùng kết quả theo dạng báo cáo chuyên nghiệp.

### 2. Nâng cấp AIEngine offline

- Thêm phân tích tổng quan điều hành cửa hàng.
- Thêm dự báo doanh thu 30 ngày tới dựa trên dữ liệu 180 ngày và xu hướng 30 ngày gần nhất.
- Thêm gợi ý nhập hàng theo tốc độ bán 90 ngày, tồn kho hiện tại, mức tồn an toàn và biên lợi nhuận.
- Thêm cảnh báo rủi ro: doanh thu giảm, hết hàng, sắp hết hàng, vốn nằm ở sản phẩm chưa bán, biên lợi nhuận thấp, khách chưa mua.
- Nâng cấp tư vấn sản phẩm theo ngân sách, loại xe, thương hiệu, màu sắc, xe điện, xe tay ga, xe sinh viên, xe cao cấp.
- Nâng cấp tìm kiếm thông minh trên nhiều nhóm dữ liệu: sản phẩm, khách hàng, nhân viên, đơn bán.
- Nâng cấp phân tích khách hàng và nhân viên theo hướng KPI, VIP, chăm sóc, hiệu suất.

### 3. Dữ liệu mẫu chuyên nghiệp

- Thêm file `Database/SampleData_Professional.sql`.
- Bổ sung nhiều sản phẩm theo phân khúc: xe phổ thông, tay ga, xe điện, cao cấp, sportbike.
- Bổ sung khách hàng mẫu ở nhiều khu vực.
- Bổ sung hóa đơn nhập và đơn bán theo nhiều mốc thời gian tương đối với ngày hiện tại.
- Tạo đủ tình huống để AI demo: bán chạy, tồn cao, sắp hết, hết hàng, khách VIP, doanh thu nhiều tháng.

### 4. Tài liệu

- Thêm `docs/AI_ASSISTANT_GUIDE.md` hướng dẫn dùng Trợ lý AI, câu hỏi mẫu và cách chạy dữ liệu mẫu.
- Cập nhật tài liệu database để hướng dẫn chạy thêm file dữ liệu mẫu chuyên nghiệp.
