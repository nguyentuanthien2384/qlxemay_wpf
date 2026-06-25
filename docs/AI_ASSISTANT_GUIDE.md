# Hướng dẫn dùng Trợ lý AI trong QLXeMay WPF

## Mục tiêu cải tiến

Trợ lý AI trong dự án là AI offline, không cần API key và không cần internet. AI đọc dữ liệu trong SQL Server rồi tạo báo cáo phân tích theo phong cách quản trị cửa hàng thật:

- Tổng quan điều hành cửa hàng.
- Phân tích tồn kho, hàng sắp hết, hàng bán chậm.
- Phân tích doanh thu, lợi nhuận ước tính, top sản phẩm.
- Dự báo doanh thu 30 ngày tới.
- Gợi ý nhập hàng dựa trên tốc độ bán 90 ngày.
- Cảnh báo rủi ro kinh doanh.
- Tư vấn sản phẩm theo nhu cầu khách hàng.
- Tìm kiếm thông minh trên sản phẩm, khách hàng, nhân viên và đơn bán.

## Cách chuẩn bị dữ liệu mẫu chuyên nghiệp

Chạy lần lượt trong SQL Server Management Studio:

```sql
-- 1. Tạo database đầy đủ
:r .\Database\CreateDatabase_Complete.sql

-- 2. Bổ sung dữ liệu mẫu lớn cho AI phân tích
:r .\Database\SampleData_Professional.sql
```

Nếu mở từng file bằng SSMS, chỉ cần mở file rồi bấm `Execute` theo thứ tự:

1. `Database/CreateDatabase_Complete.sql`
2. `Database/SampleData_Professional.sql`

File dữ liệu mẫu chuyên nghiệp bổ sung nhiều khách hàng, xe máy, hóa đơn nhập và đơn bán theo nhiều mốc thời gian tương đối với ngày hiện tại. Nhờ vậy các chức năng dự báo và cảnh báo vẫn có dữ liệu mới khi bạn chạy ở thời điểm khác.

## Câu hỏi mẫu nên demo

Bạn có thể gõ hoặc bấm trực tiếp trong giao diện Trợ lý AI:

1. Tổng quan tình hình kinh doanh hôm nay
2. Dự báo doanh thu 30 ngày tới và mức độ tin cậy
3. Xe nào nên nhập thêm trong tháng này?
4. Sản phẩm nào tồn kho cao cần khuyến mãi?
5. Phân tích rủi ro kinh doanh hiện tại
6. Gợi ý xe dưới 40 triệu cho sinh viên đi học
7. Tư vấn xe tay ga cho khách nữ đi trong thành phố
8. Top xe bán chạy và lý do nên tiếp tục nhập
9. Khách hàng VIP cần chăm sóc như thế nào?
10. Nhân viên nào đang có hiệu suất bán tốt nhất?
11. Tìm khách hàng ở Hà Nội
12. Tìm xe Honda màu trắng còn hàng
13. Có xe nào sắp hết hàng không?
14. Kế hoạch khuyến mãi để giảm tồn kho
15. So sánh doanh thu tháng này với tháng trước

## Giải thích logic AI offline

AI không gọi ChatGPT hay dịch vụ ngoài. Kết quả được tạo bằng các nhóm logic sau:

- Rule-based intent detection: nhận diện ý định từ câu hỏi tiếng Việt, ví dụ `dự báo`, `nhập thêm`, `rủi ro`, `khách hàng`, `nhân viên`, `xe rẻ`, `xe điện`.
- BI queries: truy vấn SQL để lấy doanh thu, đơn bán, chi phí nhập, tồn kho, lịch sử bán, khách VIP.
- Simple forecasting: dự báo 30 ngày dựa trên doanh thu bình quân ngày trong 180 ngày, có điều chỉnh theo xu hướng 30 ngày gần nhất.
- Reorder scoring: gợi ý nhập hàng dựa trên số bán 90 ngày, tồn kho hiện tại, mức tồn an toàn và biên lợi nhuận.
- Product scoring: chấm điểm sản phẩm theo ngân sách, loại xe, thương hiệu, màu sắc, lịch sử bán và tồn kho.

## Lưu ý khi chấm điểm bài

Khi trình bày với giảng viên, nên nói rõ đây là Trợ lý AI mô phỏng theo dữ liệu nội bộ, có khả năng phân tích và gợi ý quyết định quản trị. Vì không dùng API bên ngoài nên ưu điểm là chạy offline, dễ demo, không tốn chi phí và không lộ dữ liệu.
