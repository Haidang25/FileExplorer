# Quy ước đặt tên Control và Phím tắt — SFileManager

Tổng hợp quy ước đặt tên control (Windows Forms) và các phím tắt của SFileManager, trích trực tiếp từ mã nguồn (`Forms/*.Designer.cs`).

## 1. Quy ước chung

- Tên control = **tiền tố viết thường theo loại control** + **tên mô tả dạng PascalCase**, ví dụ `btnSearch`, `lvwFiles`, `chkRecursive`.
- Tên mô tả bằng tiếng Anh, nói rõ chức năng (`btnExportCsv`), không dùng tên mặc định như `button1`.
- Mục menu ghép theo cấp: `mnu` + menu cha + chức năng, ví dụ `mnuEditRename` (Chỉnh sửa → Đổi tên).
- Cột ListView: `col` + tên danh sách rút gọn + tên cột, ví dụ `colLogTime`, `colDupSize`.
- Cặp nhãn tiêu đề/giá trị dùng hậu tố `Caption` / `Value`, ví dụ `lblSizeCaption` / `lblSizeValue`.
- Đường phân cách: tiền tố của cha + `Separator` + số thứ tự (`tsbSeparator1`, `mnuEditSeparator2`, `cmsSeparator3`).
- Form: tên chức năng + hậu tố `Form` (`SearchForm`, `RecycleBinForm`).

## 2. Bảng tiền tố theo loại control

| Loại control | Tiền tố | Ví dụ | Ghi chú |
|:--------------------|:--------|:--------------------|:--------------------------|
| Form | — | `SearchForm` | Tên chức năng + hậu tố Form |
| MenuStrip | `mns` | `mnsMain` |  |
| ToolStripMenuItem (menu chính) | `mnu` | `mnuFileNewFolder` | mnu + menu cha + chức năng |
| ToolStripMenuItem (menu chuột phải) | `cms` | `cmsCopy` |  |
| ContextMenuStrip | `cms` | `cmsListView` |  |
| ToolStrip | `tls` | `tlsMain` |  |
| ToolStripButton | `tsb` | `tsbBack` |  |
| ToolStripSeparator | theo cha | `tsbSeparator1` | tiền tố cha + Separator + số |
| StatusStrip | `sts` | `stsMain` |  |
| ToolStripStatusLabel | `tssl` | `tsslStatus` |  |
| ToolStripProgressBar | `tsp` | `tspProgress` |  |
| Button | `btn` | `btnSearch` |  |
| Label | `lbl` | `lblStatus` | Cặp tiêu đề/giá trị: hậu tố Caption / Value |
| TextBox | `txt` | `txtKeyword` |  |
| ComboBox | `cbo` | `cboFileTypeFilter` |  |
| CheckBox | `chk` | `chkRecursive` |  |
| RadioButton | `rdo` | `rdoDetails` |  |
| GroupBox | `grp` | `grpOptions` |  |
| Panel | `pnl` | `pnlAddressBar` |  |
| SplitContainer | `spc` | `spcMain` |  |
| TreeView | `trv` | `trvFolders` |  |
| ListView | `lvw` | `lvwFiles` |  |
| ColumnHeader | `col` | `colLogTime` | col + tên danh sách rút gọn + tên cột |
| TabControl | `tbc` | `tbcLog` |  |
| TabPage | `tab` | `tabViolations` |  |
| PictureBox | `pbx` | `pbxPreview` |  |
| ProgressBar | `pgb` | `pgbScan` |  |
| DateTimePicker | `dtp` | `dtpFilterFrom` |  |
| NumericUpDown | `num` | `numWatcherDelay` |  |
| ImageList | `iml` | `imlIcons` |  |
| Timer | `tmr` | `tmrAutoClose` |  |

## 3. Danh sách control theo từng form

### 3.1. MainForm — Màn hình chính

| Tên control | Loại | Nội dung hiển thị / Chức năng | Phím tắt |
|:--------------------|:--------------|:------------------------------|:----------|
| `mnsMain` | MenuStrip | Thanh menu chính |  |
| `tlsMain` | ToolStrip | Thanh công cụ |  |
| `tsbBack` | ToolStripButton | ← Quay lại — Quay lại thư mục trước |  |
| `tsbForward` | ToolStripButton | → Tiến tới — Đi tới thư mục vừa quay lại |  |
| `tsbUp` | ToolStripButton | ↑ Lên trên — Lên thư mục cha |  |
| `tsbRefresh` | ToolStripButton | ⟳ Làm mới — Làm mới (F5) |  |
| `tsbSeparator1` | ToolStripSeparator |  |  |
| `tsbNewFolder` | ToolStripButton | + Thư mục mới — Tạo thư mục mới |  |
| `tsbSeparator2` | ToolStripSeparator |  |  |
| `tsbCopy` | ToolStripButton | ⧉ Sao chép — Sao chép (Ctrl+C) |  |
| `tsbPaste` | ToolStripButton | ▤ Dán — Dán (Ctrl+V) |  |
| `tsbSeparator3` | ToolStripSeparator |  |  |
| `tsbDelete` | ToolStripButton | ✕ Xóa — Xóa (Del) |  |
| `pnlAddressBar` | Panel | Vùng thanh địa chỉ |  |
| `btnUp` | Button | ▲ |  |
| `btnGo` | Button | ▶ |  |
| `txtPath` | TextBox | Nhập/hiển thị đường dẫn hiện tại |  |
| `txtSearch` | TextBox | Tìm kiếm... |  |
| `cboFileTypeFilter` | ComboBox | Lọc danh sách theo loại tệp |  |
| `txtQuickFilter` | TextBox | Lọc theo tên... |  |
| `spcMain` | SplitContainer | Chia cây thư mục (trái) / danh sách tệp (phải) |  |
| `trvFolders` | TreeView | Cây ổ đĩa và thư mục |  |
| `lvwFiles` | ListView | Danh sách tệp/thư mục |  |
| `colName` | ColumnHeader | Tên |  |
| `colSize` | ColumnHeader | Kích thước |  |
| `colType` | ColumnHeader | Loại |  |
| `colModified` | ColumnHeader | Ngày sửa |  |
| `lblEmptyFolder` | Label | Thư mục này trống |  |
| `spcFilesPreview` | SplitContainer | Chia danh sách tệp / khung xem trước |  |
| `pnlPreview` | Panel | Khung xem trước |  |
| `pbxPreview` | PictureBox | Hiển thị ảnh xem trước |  |
| `txtPreview` | TextBox | Vùng văn bản xem trước (hiện không dùng) |  |
| `lblPreviewCaption` | Label | Không có ảnh để xem trước |  |
| `stsMain` | StatusStrip | Thanh trạng thái |  |
| `tsslStatus` | ToolStripStatusLabel | Sẵn sàng |  |
| `tsslIntegrityAlert` | ToolStripStatusLabel | ⚠ 0 cảnh báo toàn vẹn — Bấm để xem chi tiết các cảnh báo toàn vẹn thư mục gần đây |  |
| `tsslItemCount` | ToolStripStatusLabel | 0 mục |  |
| `tsslTotalSize` | ToolStripStatusLabel | 0 byte |  |
| `tspProgress` | ToolStripProgressBar | Thanh tiến độ thao tác |  |
| `cmsListView` | ContextMenuStrip | Menu chuột phải trên danh sách tệp |  |
| `cmsOpen` | ToolStripMenuItem | Mở |  |
| `cmsSeparator1` | ToolStripSeparator |  |  |
| `cmsCut` | ToolStripMenuItem | Cắt |  |
| `cmsCopy` | ToolStripMenuItem | Sao chép |  |
| `cmsPaste` | ToolStripMenuItem | Dán |  |
| `cmsSeparator2` | ToolStripSeparator |  |  |
| `cmsDelete` | ToolStripMenuItem | Xóa |  |
| `cmsRename` | ToolStripMenuItem | Đổi tên |  |
| `cmsSeparator3` | ToolStripSeparator |  |  |
| `cmsNewFolder` | ToolStripMenuItem | Tạo thư mục mới |  |
| `cmsRefresh` | ToolStripMenuItem | Làm mới |  |
| `cmsSeparator4` | ToolStripSeparator |  |  |
| `cmsProperties` | ToolStripMenuItem | Thuộc tính |  |
| `imlIcons` | ImageList | Icon cho cây thư mục và danh sách tệp |  |
| `mnuFile` | ToolStripMenuItem | Tệp |  |
| `mnuFileNewFolder` | ToolStripMenuItem | Tạo thư mục mới | Ctrl+Shift+N |
| `mnuFileNewFile` | ToolStripMenuItem | Tạo file mới |  |
| `mnuFileSeparator1` | ToolStripSeparator |  |  |
| `mnuFileExit` | ToolStripMenuItem | Thoát | Alt+F4 |
| `mnuEdit` | ToolStripMenuItem | Chỉnh sửa |  |
| `mnuEditCut` | ToolStripMenuItem | Cắt | Ctrl+X |
| `mnuEditCopy` | ToolStripMenuItem | Sao chép | Ctrl+C |
| `mnuEditPaste` | ToolStripMenuItem | Dán | Ctrl+V |
| `mnuEditSeparator1` | ToolStripSeparator |  |  |
| `mnuEditDelete` | ToolStripMenuItem | Xóa | Delete |
| `mnuEditRename` | ToolStripMenuItem | Đổi tên | F2 |
| `mnuEditSeparator2` | ToolStripSeparator |  |  |
| `mnuEditSelectAll` | ToolStripMenuItem | Chọn tất cả | Ctrl+A |
| `mnuEditSeparator3` | ToolStripSeparator |  |  |
| `mnuEditProperties` | ToolStripMenuItem | Thuộc tính | Alt+Enter |
| `mnuView` | ToolStripMenuItem | Xem |  |
| `mnuViewRefresh` | ToolStripMenuItem | Làm mới | F5 |
| `mnuViewShowHidden` | ToolStripMenuItem | Hiện file/thư mục ẩn |  |
| `mnuViewSeparator1` | ToolStripSeparator |  |  |
| `mnuViewMode` | ToolStripMenuItem | Chế độ xem |  |
| `mnuViewModeLargeIcon` | ToolStripMenuItem | Biểu tượng lớn |  |
| `mnuViewModeSmallIcon` | ToolStripMenuItem | Biểu tượng nhỏ |  |
| `mnuViewModeList` | ToolStripMenuItem | Danh sách |  |
| `mnuViewModeDetails` | ToolStripMenuItem | Chi tiết |  |
| `mnuTools` | ToolStripMenuItem | Công cụ |  |
| `mnuToolsSearch` | ToolStripMenuItem | Tìm kiếm... | Ctrl+F |
| `mnuToolsFindDuplicates` | ToolStripMenuItem | Tìm file trùng lặp... |  |
| `mnuToolsBatchRename` | ToolStripMenuItem | Đổi tên hàng loạt... |  |
| `mnuToolsIntegrityMonitor` | ToolStripMenuItem | Giám sát toàn vẹn thư mục này |  |
| `mnuToolsSeparator1` | ToolStripSeparator |  |  |
| `mnuToolsRecycleBin` | ToolStripMenuItem | Thùng rác |  |
| `mnuToolsLogs` | ToolStripMenuItem | Xem nhật ký hoạt động |  |
| `mnuToolsSeparator2` | ToolStripSeparator |  |  |
| `mnuToolsSettings` | ToolStripMenuItem | Cài đặt... |  |
| `mnuHelp` | ToolStripMenuItem | Trợ giúp |  |
| `mnuHelpAbout` | ToolStripMenuItem | Giới thiệu... |  |
| `cmsCompressionSeparator` | ToolStripSeparator | (tạo trong code) |  |
| `cmsCompressToZip` | ToolStripMenuItem | Nén thành ZIP (tạo trong code) |  |
| `cmsExtractHere` | ToolStripMenuItem | Giải nén tại đây (tạo trong code) |  |

### 3.2. SearchForm — Tìm kiếm

| Tên control | Loại | Nội dung hiển thị / Chức năng | Phím tắt |
|:--------------------|:--------------|:------------------------------|:----------|
| `lblKeyword` | Label | Từ khóa: |  |
| `txtKeyword` | TextBox | Nhập từ khóa tìm kiếm |  |
| `lblRootFolder` | Label | Thư mục gốc: |  |
| `txtRootFolder` | TextBox | Thư mục gốc để tìm |  |
| `btnBrowseRootFolder` | Button | ... |  |
| `grpOptions` | GroupBox | Tùy chọn |  |
| `chkRecursive` | CheckBox | Tìm cả trong thư mục con |  |
| `chkIncludeHidden` | CheckBox | Bao gồm mục ẩn/hệ thống |  |
| `btnSearch` | Button | Tìm kiếm |  |
| `btnCancelSearch` | Button | Hủy |  |
| `lblStatus` | Label | Sẵn sàng |  |
| `lvwResults` | ListView | Danh sách kết quả tìm kiếm |  |
| `colResultName` | ColumnHeader | Tên |  |
| `colResultLocation` | ColumnHeader | Vị trí |  |
| `colResultSize` | ColumnHeader | Kích thước |  |
| `colResultModified` | ColumnHeader | Ngày sửa |  |
| `btnClose` | Button | Đóng |  |

### 3.3. PropertiesForm — Thuộc tính

| Tên control | Loại | Nội dung hiển thị / Chức năng | Phím tắt |
|:--------------------|:--------------|:------------------------------|:----------|
| `picIcon` | PictureBox | Icon của tệp/thư mục |  |
| `lblName` | Label | Tên tệp/thư mục |  |
| `pnlSeparatorTop` | Panel | Đường kẻ phân cách |  |
| `pnlSeparatorBottom` | Panel | Đường kẻ phân cách |  |
| `lblTypeCaption` | Label | Loại: |  |
| `lblTypeValue` | Label | Giá trị: loại |  |
| `lblLocationCaption` | Label | Vị trí: |  |
| `lblLocationValue` | Label | Giá trị: vị trí |  |
| `lblSizeCaption` | Label | Kích thước: |  |
| `lblSizeValue` | Label | Giá trị: kích thước |  |
| `lblContentsCaption` | Label | Nội dung: |  |
| `lblContentsValue` | Label | Giá trị: số tệp/thư mục con |  |
| `lblCreatedCaption` | Label | Ngày tạo: |  |
| `lblCreatedValue` | Label | Giá trị: ngày tạo |  |
| `lblModifiedCaption` | Label | Ngày sửa đổi: |  |
| `lblModifiedValue` | Label | Giá trị: ngày sửa đổi |  |
| `lblAccessedCaption` | Label | Ngày truy cập: |  |
| `lblAccessedValue` | Label | Giá trị: ngày truy cập |  |
| `grpAttributes` | GroupBox | Thuộc tính |  |
| `chkReadOnly` | CheckBox | Chỉ đọc (Read-only) |  |
| `chkHidden` | CheckBox | Ẩn (Hidden) |  |
| `chkSystem` | CheckBox | Hệ thống (System) |  |
| `chkArchive` | CheckBox | Lưu trữ (Archive) |  |
| `btnOK` | Button | OK |  |
| `btnCancel` | Button | Hủy |  |
| `btnApply` | Button | Áp dụng |  |

### 3.4. DuplicateForm — Tìm tệp trùng lặp

| Tên control | Loại | Nội dung hiển thị / Chức năng | Phím tắt |
|:--------------------|:--------------|:------------------------------|:----------|
| `lblRootFolderCaption` | Label | Thư mục quét: |  |
| `lblRootFolderValue` | Label | - |  |
| `chkRecursive` | CheckBox | Quét cả thư mục con |  |
| `btnScan` | Button | Quét lại |  |
| `btnCancelScan` | Button | Hủy |  |
| `lblStatus` | Label | Sẵn sàng. |  |
| `pgbScan` | ProgressBar | Tiến độ quét |  |
| `lvwDuplicates` | ListView | Danh sách nhóm tệp trùng lặp |  |
| `colDupName` | ColumnHeader | Tên |  |
| `colDupLocation` | ColumnHeader | Vị trí |  |
| `colDupSize` | ColumnHeader | Kích thước |  |
| `colDupModified` | ColumnHeader | Ngày sửa |  |
| `btnDeleteSelected` | Button | Xóa tệp đã chọn |  |
| `btnClose` | Button | Đóng |  |

### 3.5. BatchRenameForm — Đổi tên hàng loạt

| Tên control | Loại | Nội dung hiển thị / Chức năng | Phím tắt |
|:--------------------|:--------------|:------------------------------|:----------|
| `lblPatternCaption` | Label | Mẫu tên mới: |  |
| `txtPattern` | TextBox | Nhập mẫu tên mới |  |
| `lblPatternHint` | Label | Hỗ trợ: {name} tên gốc, {ext} phần mở rộng, {n} hoặc {n:000} số thứ tự, {date} hoặc {date:yyyyMMdd} ngày hiện tại. |  |
| `lvwPreview` | ListView | Bảng xem trước tên cũ/mới |  |
| `colOldName` | ColumnHeader | Tên hiện tại |  |
| `colNewName` | ColumnHeader | Tên mới |  |
| `btnApply` | Button | Đổi tên |  |
| `btnClose` | Button | Đóng |  |

### 3.6. RecycleBinForm — Thùng rác

| Tên control | Loại | Nội dung hiển thị / Chức năng | Phím tắt |
|:--------------------|:--------------|:------------------------------|:----------|
| `lvwItems` | ListView | Danh sách mục trong Thùng rác |  |
| `colName` | ColumnHeader | Tên |  |
| `colOriginalPath` | ColumnHeader | Vị trí gốc |  |
| `colDeletedDate` | ColumnHeader | Ngày xóa |  |
| `colSize` | ColumnHeader | Kích thước |  |
| `colType` | ColumnHeader | Loại |  |
| `lblStatus` | Label | 0 mục |  |
| `btnRefresh` | Button | Làm mới |  |
| `btnRestore` | Button | Khôi phục |  |
| `btnEmptyRecycleBin` | Button | Dọn trống thùng rác |  |
| `btnClose` | Button | Đóng |  |

### 3.7. LogForm — Nhật ký hoạt động

| Tên control | Loại | Nội dung hiển thị / Chức năng | Phím tắt |
|:--------------------|:--------------|:------------------------------|:----------|
| `tabsLog` | TabControl | Hai tab nhật ký |  |
| `tabOperationLog` | TabPage | Nhật ký thao tác |  |
| `tabViolations` | TabPage | Vi phạm toàn vẹn |  |
| `lvwViolations` | ListView | Danh sách vi phạm toàn vẹn |  |
| `colViolationTime` | ColumnHeader | Thời gian |  |
| `colViolationPath` | ColumnHeader | Đường dẫn |  |
| `colViolationType` | ColumnHeader | Loại vi phạm |  |
| `colViolationHashBefore` | ColumnHeader | Hash trước |  |
| `colViolationHashAfter` | ColumnHeader | Hash sau |  |
| `colViolationUser` | ColumnHeader | Người dùng |  |
| `grpFilters` | GroupBox | Bộ lọc |  |
| `lblFilterOperation` | Label | Thao tác: |  |
| `cboFilterOperation` | ComboBox | Lọc theo thao tác |  |
| `lblFilterResult` | Label | Kết quả: |  |
| `cboFilterResult` | ComboBox | Lọc theo kết quả |  |
| `lblFilterFrom` | Label | Từ ngày: |  |
| `dtpFilterFrom` | DateTimePicker | Lọc từ ngày |  |
| `lblFilterTo` | Label | Đến ngày: |  |
| `dtpFilterTo` | DateTimePicker | Lọc đến ngày |  |
| `btnApplyFilter` | Button | Lọc |  |
| `btnResetFilter` | Button | Đặt lại |  |
| `lvwLogs` | ListView | Danh sách nhật ký thao tác |  |
| `colLogTime` | ColumnHeader | Thời gian |  |
| `colLogOperation` | ColumnHeader | Thao tác |  |
| `colLogSource` | ColumnHeader | Nguồn |  |
| `colLogDestination` | ColumnHeader | Đích |  |
| `colLogResult` | ColumnHeader | Kết quả |  |
| `colLogItemCount` | ColumnHeader | Số mục |  |
| `colLogDuration` | ColumnHeader | Thời lượng |  |
| `colLogMessage` | ColumnHeader | Ghi chú |  |
| `lblStatus` | Label | 0 dòng log |  |
| `btnRefresh` | Button | Làm mới |  |
| `btnExportCsv` | Button | Xuất CSV |  |
| `btnVerifyReport` | Button | Xác thực báo cáo |  |
| `btnExportInvestigationReport` | Button | Xuất báo cáo điều tra |  |
| `btnClearLogs` | Button | Xóa lịch sử |  |
| `btnClose` | Button | Đóng |  |

### 3.8. SettingsForm — Cài đặt

| Tên control | Loại | Nội dung hiển thị / Chức năng | Phím tắt |
|:--------------------|:--------------|:------------------------------|:----------|
| `groupBoxDisplay` | GroupBox | HIỂN THỊ |  |
| `chkShowHidden` | CheckBox | Hiện tệp ẩn (Hidden files) |  |
| `chkShowExtension` | CheckBox | Hiện phần mở rộng tệp |  |
| `lblViewMode` | Label | Chế độ xem mặc định: |  |
| `rbDetails` | RadioButton | Chi tiết |  |
| `rbLargeIcon` | RadioButton | Biểu tượng lớn |  |
| `rbList` | RadioButton | Danh sách |  |
| `groupBoxWatcher` | GroupBox | GIÁM SÁT THƯ MỤC |  |
| `chkAutoRefresh` | CheckBox | Tự động cập nhật khi có thay đổi |  |
| `lblWatcherCaption` | Label | (FileSystemWatcher) |  |
| `lblWatcherDelay` | Label | Độ trễ cập nhật: |  |
| `numWatcherDelay` | NumericUpDown | Độ trễ cập nhật (ms) |  |
| `lblWatcherDelayUnit` | Label | ms |  |
| `groupBoxLog` | GroupBox | NHẬT KÝ |  |
| `chkEnableLog` | CheckBox | Ghi nhật ký thao tác |  |
| `lblLogPath` | Label | Vị trí lưu log: |  |
| `txtLogPath` | TextBox | Vị trí lưu tệp log |  |
| `btnOpenLogFolder` | Button | Mở thư mục |  |
| `btnSave` | Button | Lưu |  |
| `btnCancel` | Button | Hủy |  |

### 3.9. ConflictResolutionForm — Xử lý trùng tên

| Tên control | Loại | Nội dung hiển thị / Chức năng | Phím tắt |
|:--------------------|:--------------|:------------------------------|:----------|
| `lblMessage` | Label | Thông báo mục bị trùng tên |  |
| `txtNewName` | TextBox | Tên mới gợi ý (khi chọn Đổi tên) |  |
| `btnOverwrite` | Button | Ghi đè |  |
| `btnRename` | Button | Đổi tên |  |
| `btnSkip` | Button | Bỏ qua |  |
| `chkApplyToAll` | CheckBox | Áp dụng cho tất cả các mục trùng tên còn lại |  |

### 3.10. CopyProgressForm — Tiến độ sao chép/nén

| Tên control | Loại | Nội dung hiển thị / Chức năng | Phím tắt |
|:--------------------|:--------------|:------------------------------|:----------|
| `lblCurrentItem` | Label | Đang chuẩn bị... |  |
| `progressBar` | ProgressBar | Thanh tiến độ |  |
| `lblPercent` | Label | 0% |  |
| `btnCancel` | Button | Hủy |  |

### 3.11. IntegrityToastForm — Thông báo vi phạm toàn vẹn

| Tên control | Loại | Nội dung hiển thị / Chức năng | Phím tắt |
|:--------------------|:--------------|:------------------------------|:----------|
| `pnlAccent` | Panel | Dải màu nhấn bên trái |  |
| `lblIcon` | Label | ⚠ |  |
| `lblTitle` | Label | Phát hiện tệp bị sửa |  |
| `lblFilePath` | Label | (đường dẫn tệp) |  |
| `tmrAutoClose` | Timer | Tự đóng thông báo sau vài giây |  |

## 4. Phím tắt

### 4.1. Màn hình chính

| Phím tắt | Chức năng | Control / xử lý |
|:--------------------|:------------------------------|:------------------------------|
| Ctrl+Shift+N | Tạo thư mục mới | `mnuFileNewFolder` |
| Alt+F4 | Thoát chương trình | `mnuFileExit` |
| Ctrl+X | Cắt | `mnuEditCut` |
| Ctrl+C | Sao chép | `mnuEditCopy` |
| Ctrl+V | Dán | `mnuEditPaste` |
| Delete | Xóa vào Thùng rác của Windows | `mnuEditDelete` |
| Shift+Delete | Xóa vĩnh viễn (có xác nhận) | `lvwFiles_KeyDown` |
| F2 | Đổi tên | `mnuEditRename` |
| Ctrl+A | Chọn tất cả | `mnuEditSelectAll` |
| Alt+Enter | Xem thuộc tính | `mnuEditProperties` |
| F5 | Làm mới | `mnuViewRefresh` |
| Ctrl+F | Mở cửa sổ Tìm kiếm | `mnuToolsSearch` |
| Enter (trong ô đường dẫn) | Đi tới đường dẫn đã nhập | `txtPath_KeyDown` |
| Enter (trong ô Tìm kiếm) | Mở cửa sổ Tìm kiếm với từ khóa đã nhập | `txtSearch_KeyDown` |
| Giữ Ctrl khi kéo-thả | Sao chép thay vì di chuyển | `lvwFiles`, `trvFolders` |

### 4.2. Phím truy cập menu (Alt + chữ cái gạch chân)

| Phím | Menu |
|:--------------------|:------------------------------|
| Alt+T | Tệp (`mnuFile`) |
| Alt+C | Chỉnh sửa (`mnuEdit`) và Công cụ (`mnuTools`) — hai menu đang trùng chữ C (xem mục 5) |
| Alt+X | Xem (`mnuView`) |
| Alt+G | Trợ giúp (`mnuHelp`) |

### 4.3. Phím Enter / Esc trong hộp thoại

| Hộp thoại | Enter | Esc |
|:--------------------|:--------------------|:--------------------|
| SearchForm | Tìm kiếm (`btnSearch`) | — |
| PropertiesForm | OK (`btnOK`) | Hủy (`btnCancel`) |
| SettingsForm | Lưu (`btnSave`) | Hủy (`btnCancel`) |
| DuplicateForm | Quét lại (`btnScan`) | — |
| BatchRenameForm | Đóng (`btnClose`) | — |
| ConflictResolutionForm | Đổi tên (`btnRename`) | Bỏ qua (`btnSkip`) |
| CopyProgressForm | — | Hủy (`btnCancel`) |

## 5. Các tên chưa đúng quy ước

Các control dưới đây dùng tiền tố khác với bảng ở mục 2. Chương trình vẫn chạy bình thường, chỉ ảnh hưởng tính thống nhất của mã nguồn.

| Form | Tên hiện tại | Loại | Tên nên đổi |
|:--------------------|:--------------------|:--------------|:--------------------|
| PropertiesForm | `picIcon` | PictureBox | `pbxIcon` |
| LogForm | `tabsLog` | TabControl | `tbcLog` |
| SettingsForm | `groupBoxDisplay` | GroupBox | `grpDisplay` |
| SettingsForm | `rbDetails` | RadioButton | `rdoDetails` |
| SettingsForm | `rbLargeIcon` | RadioButton | `rdoLargeIcon` |
| SettingsForm | `rbList` | RadioButton | `rdoList` |
| SettingsForm | `groupBoxWatcher` | GroupBox | `grpWatcher` |
| SettingsForm | `groupBoxLog` | GroupBox | `grpLog` |
| CopyProgressForm | `progressBar` | ProgressBar | `pgbProgress` |

**Ghi chú thêm:**

- Menu **Chỉnh sửa** (`&Chỉnh sửa`) và **Công cụ** (`&Công cụ`) cùng dùng phím truy cập chữ **C**. Nên đổi một menu, ví dụ `Công c&ụ` (Alt+U), để mỗi menu có một phím riêng.
- Trong `BatchRenameForm`, `AcceptButton` đang là `btnClose`, nên nhấn **Enter** sẽ đóng hộp thoại thay vì đổi tên. Nếu muốn Enter thực hiện đổi tên, đặt `AcceptButton = btnApply`.
