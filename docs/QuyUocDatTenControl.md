# Quy ước đặt tên control — MainForm (SFileManager)

Áp dụng Hungarian notation: `<tiền tố 3 chữ theo loại control><Tên mô tả PascalCase>`.
Riêng `ToolStripMenuItem`/`ToolStripButton`/`ColumnHeader`/`ToolStripStatusLabel` dùng tiền tố
ngắn hơn (`mnu`, `tsb`, `col`, `tssl`) vì số lượng nhiều và đã quen thuộc trong codebase từ đầu.

## Bảng tiền tố theo loại control

| Loại control          | Tiền tố | Ví dụ         |
|------------------------|---------|---------------|
| MenuStrip              | `mns`   | `mnsMain`     |
| ToolStripMenuItem      | `mnu`   | `mnuFile`     |
| ToolStrip              | `tls`   | `tlsMain`     |
| ToolStripButton        | `tsb`   | `tsbBack`     |
| ToolStripSeparator     | *(theo cha)* `tsbSeparatorN` / `mnuXSeparatorN` / `cmsSeparatorN` | `tsbSeparator1` |
| Panel                  | `pnl`   | `pnlAddressBar` |
| Button                 | `btn`   | `btnUp`       |
| TextBox                | `txt`   | `txtPath`     |
| SplitContainer         | `spc`   | `spcMain`     |
| TreeView               | `trv`   | `trvFolders`  |
| ListView               | `lvw`   | `lvwFiles`    |
| ColumnHeader           | `col`   | `colName`     |
| StatusStrip            | `sts`   | `stsMain`     |
| ToolStripStatusLabel   | `tssl`  | `tsslStatus`  |
| ToolStripProgressBar   | `tsp`   | `tspProgress` |
| ContextMenuStrip       | `cms`   | `cmsListView` |
| ImageList              | `iml`   | `imlIcons`    |

## Danh sách toàn bộ control trong MainForm

### Khung chính

| Tên control    | Loại           | Chức năng                                             |
|-----------------|----------------|--------------------------------------------------------|
| `mnsMain`       | MenuStrip      | Thanh menu chính (Tệp/Chỉnh sửa/Xem/Công cụ/Trợ giúp) |
| `tlsMain`       | ToolStrip      | Thanh công cụ (Back/Up/Refresh/New Folder/Copy/Paste/Delete) |
| `pnlAddressBar` | Panel          | Vùng chứa thanh địa chỉ (Up + txtPath + Go)           |
| `spcMain`       | SplitContainer | Chia 2 vùng làm việc: cây thư mục (trái) / danh sách file (phải) |
| `stsMain`       | StatusStrip    | Thanh trạng thái dưới cùng (số mục/dung lượng/trạng thái/tiến trình) |
| `imlIcons`      | ImageList      | Icon "folder"/"file" dùng chung cho `trvFolders` và `lvwFiles` |
| `cmsListView`   | ContextMenuStrip | Menu chuột phải trên `lvwFiles`                     |

### Thanh địa chỉ (trong `pnlAddressBar`)

| Tên control | Loại    | Chức năng                        |
|-------------|---------|-----------------------------------|
| `btnUp`     | Button  | Lên thư mục cha (Dock=Left)       |
| `btnGo`     | Button  | Điều hướng đến đường dẫn nhập (Dock=Right) |
| `txtPath`   | TextBox | Nhập/hiển thị đường dẫn hiện tại (Dock=Fill) |

### ToolStrip (`tlsMain`)

| Tên control     | Loại               | Chức năng          |
|------------------|--------------------|---------------------|
| `tsbBack`        | ToolStripButton    | Quay lại thư mục trước |
| `tsbUp`          | ToolStripButton    | Lên thư mục cha     |
| `tsbRefresh`     | ToolStripButton    | Làm mới (F5)        |
| `tsbSeparator1`  | ToolStripSeparator | —                   |
| `tsbNewFolder`   | ToolStripButton    | Tạo thư mục mới     |
| `tsbSeparator2`  | ToolStripSeparator | —                   |
| `tsbCopy`        | ToolStripButton    | Sao chép (Ctrl+C)   |
| `tsbPaste`       | ToolStripButton    | Dán (Ctrl+V)        |
| `tsbSeparator3`  | ToolStripSeparator | —                   |
| `tsbDelete`      | ToolStripButton    | Xóa (Del)           |

### Vùng làm việc (`spcMain`)

| Tên control     | Loại        | Vị trí          | Chức năng                              |
|------------------|-------------|-----------------|------------------------------------------|
| `trvFolders`     | TreeView    | Panel1 (trái)   | Cây thư mục, lazy load, đồng bộ với `_currentPath` |
| `lvwFiles`       | ListView    | Panel2 (phải)   | Danh sách file/thư mục, chế độ Details, 4 cột |
| `colName`        | ColumnHeader| trong `lvwFiles`| Cột "Tên"           |
| `colSize`        | ColumnHeader| trong `lvwFiles`| Cột "Kích thước"    |
| `colType`        | ColumnHeader| trong `lvwFiles`| Cột "Loại"          |
| `colModified`    | ColumnHeader| trong `lvwFiles`| Cột "Ngày sửa đổi"  |

### StatusStrip (`stsMain`)

| Tên control      | Loại                  | Chức năng                                   |
|-------------------|-----------------------|-----------------------------------------------|
| `tsslStatus`      | ToolStripStatusLabel  | Trạng thái hiện tại / số mục đang chọn (Spring=true, giãn hết chỗ trống) |
| `tspProgress`     | ToolStripProgressBar  | Thanh tiến trình, mặc định ẩn (`Visible=false`) |
| `tsslItemCount`   | ToolStripStatusLabel  | Tổng số mục trong thư mục hiện tại            |
| `tsslTotalSize`   | ToolStripStatusLabel  | Tổng dung lượng các file trong thư mục hiện tại |

### ContextMenuStrip (`cmsListView`)

| Tên control      | Loại                | Chức năng (dùng chung handler với menu chính) |
|-------------------|---------------------|-------------------------------------------------|
| `cmsOpen`         | ToolStripMenuItem   | Mở mục đang chọn                                |
| `cmsSeparator1`   | ToolStripSeparator  | —                                                |
| `cmsCut`          | ToolStripMenuItem   | Cắt (→ `mnuEditCut_Click`)                      |
| `cmsCopy`         | ToolStripMenuItem   | Sao chép (→ `mnuEditCopy_Click`)                |
| `cmsPaste`        | ToolStripMenuItem   | Dán (→ `mnuEditPaste_Click`)                    |
| `cmsSeparator2`   | ToolStripSeparator  | —                                                |
| `cmsDelete`       | ToolStripMenuItem   | Xóa (→ `mnuEditDelete_Click`)                   |
| `cmsRename`       | ToolStripMenuItem   | Đổi tên (→ `mnuEditRename_Click`)               |
| `cmsSeparator3`   | ToolStripSeparator  | —                                                |
| `cmsNewFolder`    | ToolStripMenuItem   | Tạo thư mục mới (→ `mnuFileNewFolder_Click`)    |
| `cmsRefresh`      | ToolStripMenuItem   | Làm mới (→ `mnuViewRefresh_Click`)              |

### Menu "Tệp" (`mnuFile`)

| Tên control          | Chức năng                     |
|------------------------|--------------------------------|
| `mnuFileNewFolder`     | Tạo thư mục mới (Ctrl+Shift+N) |
| `mnuFileNewFile`       | Tạo file mới                  |
| `mnuFileSeparator1`    | —                              |
| `mnuFileExit`          | Thoát (Alt+F4)                |

### Menu "Chỉnh sửa" (`mnuEdit`)

| Tên control          | Chức năng             |
|------------------------|------------------------|
| `mnuEditCut`           | Cắt (Ctrl+X)          |
| `mnuEditCopy`          | Sao chép (Ctrl+C)     |
| `mnuEditPaste`         | Dán (Ctrl+V)          |
| `mnuEditSeparator1`    | —                      |
| `mnuEditDelete`        | Xóa (Del)             |
| `mnuEditRename`        | Đổi tên (F2)          |
| `mnuEditSeparator2`    | —                      |
| `mnuEditSelectAll`     | Chọn tất cả (Ctrl+A)  |

### Menu "Xem" (`mnuView`)

| Tên control              | Chức năng                        |
|----------------------------|------------------------------------|
| `mnuViewRefresh`           | Làm mới (F5)                      |
| `mnuViewShowHidden`        | Hiện file/thư mục ẩn (checkbox)   |
| `mnuViewSeparator1`        | —                                  |
| `mnuViewMode`              | Submenu chọn chế độ xem            |
| `mnuViewModeLargeIcon`     | Biểu tượng lớn                    |
| `mnuViewModeSmallIcon`     | Biểu tượng nhỏ                    |
| `mnuViewModeList`          | Danh sách                          |
| `mnuViewModeDetails`       | Chi tiết (mặc định, `Checked=true`) |

### Menu "Công cụ" (`mnuTools`)

| Tên control              | Chức năng                    |
|----------------------------|--------------------------------|
| `mnuToolsSearch`           | Tìm kiếm (Ctrl+F)             |
| `mnuToolsFindDuplicates`   | Tìm file trùng lặp            |
| `mnuToolsSeparator1`       | —                              |
| `mnuToolsRecycleBin`       | Thùng rác                     |
| `mnuToolsLogs`             | Xem nhật ký hoạt động         |
| `mnuToolsSeparator2`       | —                              |
| `mnuToolsSettings`         | Cài đặt                       |

### Menu "Trợ giúp" (`mnuHelp`)

| Tên control    | Chức năng      |
|------------------|------------------|
| `mnuHelpAbout`   | Giới thiệu     |
