# Gói hoàn thiện QLXeMay WPF Pro

Bản này tiếp tục hoàn thiện sau gói Pro Upgrade ban đầu. Phạm vi đã bổ sung:

- Tách password policy thành `Domain/PasswordPolicy.cs` để dùng chung và kiểm thử tự động.
- Thêm màn hình `Nhật ký hệ thống` gồm model, service, view-model và window riêng.
- Thêm permission `AuditLog`, chỉ Administrator mặc định có quyền xem nhật ký.
- Thêm index cho `tblusers.roleid` và `tblauditlog` để truy vấn quản trị nhanh hơn.
- Mở rộng test console với các test password policy.
- Thêm publish profile `Properties/PublishProfiles/FolderProfile.pubxml`.
- Thêm script `tools/verify-source.ps1` để restore, build, test và publish trên Windows.
- Thêm `docs/TEST_PLAN.md` cho kiểm thử bàn giao.

Giới hạn kỹ thuật: môi trường tạo gói không có .NET SDK/Windows WPF runtime, vì vậy bước build/runtime cuối cùng phải chạy trên Windows. Script verify đã được chuẩn bị để bạn hoặc giảng viên/nhóm chạy lại một lệnh trên máy thật.
