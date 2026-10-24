/*
  QuanLySinhVienDB — schema + seed cho project Quản lý sinh viên (BTH 10-10)
  Dựa trên Entity/LopHoc.cs và Entity/SinhVien.cs hiện tại (chưa migrate ADO.NET).

  Chạy an toàn nhiều lần (idempotent):
  sqlcmd -S localhost\SQLEXPRESS -E -d master -i QuanLySinhVien-schema.sql

  Quy ước khớp code hiện tại (không thay business rule):
  - MaLop/MaSV so khớp không phân biệt hoa thường trong DAL (OrdinalIgnoreCase)
    -> collation Vietnamese_100_CI_AS (CI = case-insensitive).
  - "Không thể xóa lớp đang có sinh viên" (LopHocDAL.DeleteLopHoc)
    -> FK ON DELETE NO ACTION (mặc định, chặn xóa lớp còn SV).
  - MaSV regex ^SV\d{4,6}$ ; Điện thoại ^\d{9,11}$ ; Điểm 0..10 ; Ngày sinh >= 1900 và < hôm nay
    -> CHECK constraint tương ứng.
  - GioiTinh ('Nam', N'Nữ') và TrangThai ('Đang học', N'Bảo lưu', N'Đã tốt nghiệp')
    là các giá trị duy nhất UI (MainForm) cho phép nhập -> CHECK theo đúng tập đó.
  - Diem là decimal? (nullable) trong entity -> cột cho phép NULL.
*/
:SETVAR DatabaseName QuanLySinhVienDB
GO
IF DB_ID('QuanLySinhVienDB') IS NULL
    CREATE DATABASE QuanLySinhVienDB COLLATE Vietnamese_100_CI_AS;
GO
USE QuanLySinhVienDB;
GO

/* ---------- Tables ---------- */
IF OBJECT_ID(N'dbo.LopHoc', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LopHoc
    (
        MaLop  nvarchar(20)  NOT NULL,
        TenLop nvarchar(100) NOT NULL,
        CONSTRAINT PK_LopHoc PRIMARY KEY (MaLop),
        CONSTRAINT CK_LopHoc_TenLop CHECK (LEN(TenLop) >= 3)  -- mirror: Tên lớp 3–100 ký tự
    );
END;
GO

IF OBJECT_ID(N'dbo.SinhVien', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SinhVien
    (
        MaSV         nvarchar(8)   NOT NULL,                       -- SV + 4–6 chữ số => tối đa 8 ký tự
        HoTen        nvarchar(100) NOT NULL,
        NgaySinh     date          NOT NULL,
        GioiTinh     nvarchar(10)  NOT NULL,
        Email        nvarchar(100) NOT NULL,
        SoDienThoai  nvarchar(11)  NOT NULL,
        Diem         decimal(4,1)  NULL,                           -- entity: decimal? + Range 0..10
        TrangThai    nvarchar(20)  NOT NULL,
        MaLop        nvarchar(20)  NOT NULL,
        CONSTRAINT PK_SinhVien PRIMARY KEY (MaSV),
        CONSTRAINT FK_SinhVien_LopHoc FOREIGN KEY (MaLop)
            REFERENCES dbo.LopHoc (MaLop),                          -- NO ACTION: chặn xóa lớp còn SV
        CONSTRAINT CK_SinhVien_MaSV CHECK
            (LEN(MaSV) BETWEEN 6 AND 8                              -- SV + 4..6 chữ số
             AND MaSV LIKE N'SV[0-9]%'
             AND SUBSTRING(MaSV, 3, LEN(MaSV) - 2) NOT LIKE N'%[^0-9]%'),  -- phần sau SV chỉ là chữ số
        CONSTRAINT CK_SinhVien_HoTen CHECK (LEN(HoTen) >= 2),       -- Họ tên 2–100 ký tự
        CONSTRAINT CK_SinhVien_NgaySinh CHECK
            (NgaySinh >= '1900-01-01' AND NgaySinh < CAST(GETDATE() AS date)),
        CONSTRAINT CK_SinhVien_GioiTinh CHECK (GioiTinh IN (N'Nam', N'Nữ')),
        CONSTRAINT CK_SinhVien_Email CHECK (Email LIKE N'%_@_%._%'), -- EmailAddress (xấp xỉ, giữ nhẹ)
        CONSTRAINT CK_SinhVien_SoDienThoai CHECK
            (LEN(SoDienThoai) BETWEEN 9 AND 11
             AND SoDienThoai NOT LIKE N'%[^0-9]%'),                 -- ^\d{9,11}$
        CONSTRAINT CK_SinhVien_Diem CHECK (Diem >= 0 AND Diem <= 10),
        CONSTRAINT CK_SinhVien_TrangThai CHECK
            (TrangThai IN (N'Đang học', N'Bảo lưu', N'Đã tốt nghiệp'))
    );

    CREATE INDEX IX_SinhVien_MaLop ON dbo.SinhVien (MaLop);         -- phục vụ GetSinhViensByMaLop
END;
GO

/* ---------- Seed (an toàn chạy lại: chỉ thêm nếu chưa có) ----------
   Dữ liệu mẫu khớp với bộ seed gốc của bản DataList (v3) mà Tests/Program.cs
   và MainForm dựa vào: 3 lớp, 4 sinh viên. */
IF NOT EXISTS (SELECT 1 FROM dbo.LopHoc WHERE MaLop = N'KTPM01')
    INSERT INTO dbo.LopHoc (MaLop, TenLop) VALUES
        (N'KTPM01', N'Kỹ thuật phần mềm 01'),
        (N'AI01',   N'Trí tuệ nhân tạo 01'),
        (N'KHDL01', N'Khoa học dữ liệu 01');
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = N'SV000123')
    INSERT INTO dbo.SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, Email, SoDienThoai, Diem, TrangThai, MaLop) VALUES
        (N'SV000123', N'Nguyễn Văn An', '2006-08-15', N'Nam', N'sv000123@example.com', '0912345678', 8.5, N'Đang học', N'KTPM01'),
        (N'SV000124', N'Trần Minh Anh', '2006-08-15', N'Nữ',  N'sv000124@example.com', '0987654321', 9.0, N'Đang học', N'AI01'),
        (N'SV000125', N'Lê Hoàng Bình', '2006-08-15', N'Nam', N'sv000125@example.com', '0355556677', 7.4, N'Đang học', N'KTPM01'),
        (N'SV000126', N'Đỗ Thị Hồng',   '2006-08-15', N'Nữ',  N'sv000126@example.com', '0777888999', 8.1, N'Đang học', N'KHDL01');
GO
