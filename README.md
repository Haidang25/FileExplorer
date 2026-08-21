# FileExplorer

## Giới thiệu

**FileExplorer** là đồ án môn học xây dựng một ứng dụng quản lý tệp tin (File Explorer) trên nền tảng Windows, mô phỏng lại các chức năng cơ bản của Windows Explorer. Ứng dụng được phát triển bằng **C#** với **Windows Forms (WinForms)** trên nền tảng **.NET**.

Mục tiêu của đồ án là vận dụng kiến thức lập trình hướng đối tượng, thao tác với hệ thống tệp tin (File System) trong .NET, cùng kỹ năng xây dựng giao diện người dùng (GUI) để tạo ra một ứng dụng quản lý file/folder trực quan, dễ sử dụng.

## Công nghệ sử dụng

- **Ngôn ngữ:** C#
- **Nền tảng:** .NET (Windows Forms)
- **IDE:** Visual Studio
- **Hệ điều hành:** Windows

## Tính năng chính

- Duyệt cây thư mục (Treeview) và xem nội dung thư mục hiện tại (ListView).
- Tạo, đổi tên, xóa file và folder.
- Sao chép (Copy), cắt (Cut), dán (Paste) file/folder.
- Xem thông tin thuộc tính (properties) của file/folder: kích thước, ngày tạo, ngày sửa đổi.
- Tìm kiếm file/folder theo tên.
- Sắp xếp và hiển thị file theo loại, kích thước, ngày sửa đổi.
- Mở file bằng ứng dụng mặc định của hệ thống.
- Quản lý ổ đĩa (Drives) và điều hướng qua lại giữa các thư mục (Back/Forward/Up).

> Ghi chú: danh sách tính năng trên có thể được cập nhật theo tiến độ triển khai thực tế của đồ án.

## Cấu trúc dự án

```
FileExplorer/
├── FileExplorer.sln           # Solution file
├── FileExplorer/              # Source code chính
│   ├── Forms/                 # Các form giao diện
│   ├── Models/                 # Các lớp mô hình dữ liệu
│   ├── Services/                # Xử lý logic thao tác file/folder
│   └── Program.cs
└── README.md
```

## Yêu cầu hệ thống

- Windows 10/11
- .NET SDK (khuyến nghị .NET 6.0 trở lên) hoặc .NET Framework tương ứng với cấu hình project
- Visual Studio 2022 (hoặc phiên bản phù hợp)

## Hướng dẫn cài đặt & chạy chương trình

1. Clone repository:
   ```bash
   git clone https://github.com/Haidang25/FileExplorer.git
   ```
2. Mở file `FileExplorer.sln` bằng Visual Studio.
3. Build solution (Ctrl+Shift+B).
4. Nhấn F5 hoặc chọn **Start** để chạy ứng dụng.

## Tác giả

- Nhan Nguyen Huu

## Giấy phép

Đồ án phục vụ mục đích học tập.
