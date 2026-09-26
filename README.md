# SFileManager

Ứng dụng quản lý tệp tin trên Windows, xây dựng bằng **C# / Windows Forms (.NET Framework 4.7.2)**. Đây là đồ án môn học với mục tiêu bổ sung cho File Explorer những chức năng mà công cụ mặc định của Windows còn thiếu: **ghi nhật ký thao tác**, **giám sát toàn vẹn tệp bằng giá trị băm** và **tìm tệp trùng lặp**.

## Tính năng

**Quản lý tệp cơ bản**
- Duyệt ổ đĩa và cây thư mục (nạp dần khi mở rộng), tự cập nhật khi cắm/rút USB.
- Điều hướng: thanh địa chỉ, Quay lại / Tiến tới / Lên trên / Làm mới.
- Tạo, đổi tên, sao chép, cắt, dán, xóa tệp và thư mục; chọn nhiều mục; kéo-thả (kể cả kéo từ ứng dụng khác vào).
- Hỏi khi trùng tên: Ghi đè, Bỏ qua hoặc Đổi tên (gợi ý sẵn tên mới); hiển thị tiến độ và cho phép hủy khi sao chép.
- Xóa vào Thùng rác của Windows hoặc xóa vĩnh viễn; xem và khôi phục Thùng rác ngay trong ứng dụng.
- 4 chế độ xem, sắp xếp theo cột, lọc theo loại tệp, xem trước ảnh.
- Xem và sửa thuộc tính (Chỉ đọc, Ẩn, Hệ thống, Lưu trữ), tính dung lượng thư mục.

**Tìm kiếm**
- Tìm theo tên, hỗ trợ ký tự đại diện `*` và `?`, tìm đệ quy trong thư mục con.
- Chạy nền, hiện kết quả ngay khi tìm thấy, có thể hủy giữa chừng.

**Nhật ký và giám sát toàn vẹn**
- Ghi nhật ký mọi thao tác (thời điểm, thao tác, nguồn, đích, kết quả); lọc và xuất CSV.
- Giám sát toàn vẹn một thư mục: chụp baseline băm **SHA-256** cho từng tệp, sau đó theo dõi bằng `FileSystemWatcher` và cảnh báo khi tệp bị sửa nội dung, bị xóa hoặc có tệp lạ xuất hiện.
- Mỗi vi phạm được ghi kèm giá trị băm trước/sau và tài khoản Windows, có thể xuất báo cáo điều tra.

**Công cụ mở rộng**
- Tìm tệp trùng lặp: nhóm theo kích thước rồi so sánh băm **MD5**, chọn và xóa bản thừa.
- Đổi tên hàng loạt theo mẫu với các token `{name}`, `{ext}`, `{n}` / `{n:000}`, `{date}`, có bảng xem trước.
- Nén thư mục thành `.zip` và giải nén `.zip`.

**Giao diện**
- Toàn bộ giao diện tiếng Việt.
- Chữ và icon được phóng to (mặc định 12pt) để dễ đọc khi trình chiếu, cửa sổ tự vừa màn hình nhỏ.

## Yêu cầu

| | Chạy chương trình | Phát triển |
|---|---|---|
| Hệ điều hành | Windows 7 SP1 / 10 / 11 | Windows 10 / 11 |
| Runtime | .NET Framework 4.7.2 trở lên | .NET Framework 4.7.2 Developer Pack |
| Công cụ | — | Visual Studio 2022 (17.13+) hoặc mới hơn, workload *.NET desktop development* |

Ứng dụng chỉ dùng thư viện có sẵn của .NET Framework (`System.IO`, `System.IO.Compression`, `System.Security.Cryptography`), không cần gói NuGet để chạy.

## Build và chạy từ mã nguồn

```bash
git clone https://github.com/Haidang25/FileExplorer.git
```

1. Mở `FileExplorerApp.slnx` bằng Visual Studio.
2. Chọn cấu hình **Release** (hoặc Debug), build bằng **Ctrl+Shift+B**.
3. Nhấn **F5** để chạy. Tệp chạy nằm ở `bin\Release\FileExplorerApp.exe`.

## Chạy bản đóng gói

Ứng dụng chạy độc lập (portable), không cần cài đặt hay quyền quản trị:

1. Giải nén `SFileManager_v1.0.zip` vào một thư mục bất kỳ.
2. Giữ `SFileManager.exe` và `SFileManager.exe.config` trong cùng thư mục, chạy `SFileManager.exe`.

Tự đóng gói: build Release, sao chép `bin\Release\FileExplorerApp.exe` và `FileExplorerApp.exe.config` ra thư mục mới (có thể đổi tên thành `SFileManager.exe` / `SFileManager.exe.config`, hai tên phải khớp nhau), rồi nén lại.

## Dữ liệu ứng dụng

| Dữ liệu | Vị trí mặc định |
|---|---|
| Nhật ký thao tác, báo cáo điều tra | `%AppData%\SFileManager\logs` (đổi được trong Cài đặt) |
| Baseline giám sát toàn vẹn | `%AppData%\SFileManager\baselines` |
| Tệp đã xóa (không xóa vĩnh viễn) | Thùng rác của Windows |

## Kiến trúc và cấu trúc thư mục

Ứng dụng tổ chức theo 3 lớp: **Presentation** (Forms) → **Business Logic** (Services) → **System Access** (`System.IO`, `System.IO.Compression`, `System.Security.Cryptography`). Lớp trên chỉ gọi lớp liền dưới.

```
FileExplorerApp/
├── FileExplorerApp.slnx / .csproj
├── Program.cs              # Điểm khởi chạy, bộ xử lý ngoại lệ toàn cục
├── Forms/                  # 12 form: MainForm, SearchForm, PropertiesForm, DuplicateForm,
│                           # BatchRenameForm, RecycleBinForm, LogForm, SettingsForm,
│                           # AboutForm, ConflictResolutionForm, CopyProgressForm, IntegrityToastForm
├── Services/               # 10 service: File, Folder, Search, Log, FileMonitor, Integrity,
│                           # Baseline, Duplicate, Compression, RecycleBin
├── Models/                 # FileItemModel, FolderItemModel, LogEntryModel, FolderBaselineModel,
│                           # IntegrityInvestigationEntry, RecycleBinItemModel, OperationResult...
├── Helpers/                # FileHelper, PermissionHelper, HashHelper, FormatHelper,
│                           # ErrorHandler, AppTheme, UiScale...
├── Properties/             # AssemblyInfo, Resources, Settings
├── Resources/              # app.ico
└── docs/                   # Quy ước đặt tên control
```

Quy ước: mỗi thư mục con là một namespace con của `FileExplorerApp` (ví dụ `FileExplorerApp.Services`), được kiểm tra qua `.editorconfig`. Quy ước đặt tên control xem tại [docs/QuyUocDatTenControl.md](docs/QuyUocDatTenControl.md).

**Chỉnh cỡ giao diện:** đổi hằng số `FontSize` trong [`Helpers/UiScale.cs`](Helpers/UiScale.cs) (ví dụ `11F` nhỏ hơn, `13F` to hơn). Cỡ icon danh sách tệp đặt trong `MainForm.InitializeUiScaling()`.

## Nhóm thực hiện

| Thành viên | Vai trò |
|---|---|
| Nguyễn Hải Đăng (nhóm trưởng) | Phân tích, thiết kế, lập trình toàn bộ chức năng |
| Lương Mỹ Hoa | Phân tích yêu cầu, kiểm thử chức năng |
| Phạm Thanh Trọng | Kiểm thử hiệu năng, tương thích, đóng gói |
| Hồ Lê Quốc Khang | Soạn thảo báo cáo, tài liệu tham khảo |

## Giấy phép

Đồ án phục vụ mục đích học tập.
