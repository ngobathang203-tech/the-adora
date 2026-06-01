-- ========================
-- TẠO DATABASE
-- ========================
CREATE DATABASE Adora;
GO
USE Adora;
GO

-- ========================
-- 1. CHI NHÁNH
-- ========================
CREATE TABLE ChiNhanh (
    MaChiNhanh INT IDENTITY PRIMARY KEY,
    TenChiNhanh NVARCHAR(255),
    DiaChi NVARCHAR(255),
    SoDienThoai VARCHAR(15)
);

-- ========================
-- 2. SẢNH
-- ========================
CREATE TABLE Sanh (
    MaSanh INT IDENTITY PRIMARY KEY,
    TenSanh NVARCHAR(255),
    SucChua INT,
    GiaThue DECIMAL(12,2) CHECK (GiaThue >= 0),
    MaChiNhanh INT,
    HinhAnh NVARCHAR(500),
    FOREIGN KEY (MaChiNhanh) REFERENCES ChiNhanh(MaChiNhanh)
);

-- ========================
-- 3. KHÁCH HÀNG
-- ========================
CREATE TABLE KhachHang (
    MaKhachHang INT IDENTITY PRIMARY KEY,
    TenKhachHang NVARCHAR(255),
    SoDienThoai VARCHAR(15),
    Email VARCHAR(255)
);

-- ========================
-- 4. LOẠI SỰ KIỆN
-- ========================
CREATE TABLE LoaiSuKien (
    MaLoai INT IDENTITY PRIMARY KEY,
    TenLoai NVARCHAR(100)
);

-- ========================
-- 5. MENU
-- ========================
CREATE TABLE Menu (
    MaMenu INT IDENTITY PRIMARY KEY,
    TenMenu NVARCHAR(255),
    LoaiMenu NVARCHAR(50)
);

-- ========================
-- 6. MÓN ĂN
-- ========================
CREATE TABLE MonAn (
    MaMon INT IDENTITY PRIMARY KEY,
    TenMon NVARCHAR(255),
    Gia DECIMAL(12,2) CHECK (Gia >= 0)
);

-- ========================
-- 7. CHI TIẾT MENU
-- ========================
CREATE TABLE ChiTietMenu (
    MaMenu INT,
    MaMon INT,
    SoLuong INT CHECK (SoLuong > 0),
    PRIMARY KEY (MaMenu, MaMon),
    FOREIGN KEY (MaMenu) REFERENCES Menu(MaMenu),
    FOREIGN KEY (MaMon) REFERENCES MonAn(MaMon)
);

-- ========================
-- 8. ĐẶT SẢNH
-- ========================
CREATE TABLE DatSanh (
    MaDat INT IDENTITY PRIMARY KEY,
    MaSanh INT,
    MaKhachHang INT,
    MaLoai INT,
    MaMenu INT,
    TenSuKien NVARCHAR(255),
    NgayToChuc DATE,
    GioBatDau TIME,
    GioKetThuc TIME,
    TrangThai NVARCHAR(50),

    FOREIGN KEY (MaSanh) REFERENCES Sanh(MaSanh),
    FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang),
    FOREIGN KEY (MaLoai) REFERENCES LoaiSuKien(MaLoai),
    FOREIGN KEY (MaMenu) REFERENCES Menu(MaMenu),

    CONSTRAINT CK_Gio CHECK (GioBatDau < GioKetThuc),
    CONSTRAINT CK_TrangThai CHECK (
        TrangThai IN (N'Đang đặt', N'Đã cọc', N'Hoàn thành', N'Đã hủy')
    )
);

-- ========================
-- 9. LỊCH SỬ ĐỔI LỊCH
-- ========================
CREATE TABLE LichSuDoiLich (
    MaDoiLich INT IDENTITY PRIMARY KEY,
    MaDat INT,
    NgayCu DATE,
    NgayMoi DATE,
    LyDo NVARCHAR(255),
    NgayYeuCau DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(50),

    FOREIGN KEY (MaDat) REFERENCES DatSanh(MaDat) ON DELETE CASCADE
);

-- ========================
-- 10. BÀN
-- ========================
CREATE TABLE Ban (
    MaBan INT IDENTITY PRIMARY KEY,
    MaDat INT,
    TenBan NVARCHAR(50),
    SoKhach INT CHECK (SoKhach >= 0),

    FOREIGN KEY (MaDat) REFERENCES DatSanh(MaDat) ON DELETE CASCADE
);

-- ========================
-- 11. NGUYÊN LIỆU
-- ========================
CREATE TABLE NguyenLieu (
    MaNguyenLieu INT IDENTITY PRIMARY KEY,
    TenNguyenLieu NVARCHAR(255),
    DonVi NVARCHAR(50)
);

-- ========================
-- 12. CÔNG THỨC
-- ========================
CREATE TABLE CongThuc (
    MaMon INT,
    MaNguyenLieu INT,
    SoLuong FLOAT,

    PRIMARY KEY (MaMon, MaNguyenLieu),
    FOREIGN KEY (MaMon) REFERENCES MonAn(MaMon),
    FOREIGN KEY (MaNguyenLieu) REFERENCES NguyenLieu(MaNguyenLieu)
);

-- ========================
-- 13. DỊCH VỤ
-- ========================
CREATE TABLE DichVu (
    MaDichVu INT IDENTITY PRIMARY KEY,
    TenDichVu NVARCHAR(255),
    Gia DECIMAL(12,2) CHECK (Gia >= 0)
);

-- ========================
-- 14. DỊCH VỤ THEO TIỆC
-- ========================
CREATE TABLE DichVuSuKien (
    MaDat INT,
    MaDichVu INT,
    SoLuong INT CHECK (SoLuong > 0),

    PRIMARY KEY (MaDat, MaDichVu),
    FOREIGN KEY (MaDat) REFERENCES DatSanh(MaDat) ON DELETE CASCADE,
    FOREIGN KEY (MaDichVu) REFERENCES DichVu(MaDichVu)
);

-- ========================
-- 15. HÓA ĐƠN
-- ========================
CREATE TABLE HoaDon (
    MaHoaDon INT IDENTITY PRIMARY KEY,
    MaDat INT,
    TongTien DECIMAL(12,2) CHECK (TongTien >= 0),
    NgayLap DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (MaDat) REFERENCES DatSanh(MaDat) ON DELETE CASCADE
);

-- ========================
-- 16. KHUYẾN MÃI
-- ========================
CREATE TABLE KhuyenMai (
    MaKhuyenMai INT IDENTITY PRIMARY KEY,
    TenKhuyenMai NVARCHAR(255),

    PhanTramGiam DECIMAL(5,2) NULL CHECK (PhanTramGiam BETWEEN 0 AND 100),
    SoTienGiam DECIMAL(12,2) NULL CHECK (SoTienGiam >= 0),

    NgayBatDau DATE,
    NgayKetThuc DATE,
    TrangThai NVARCHAR(50),

    CONSTRAINT CK_KM CHECK (
        (PhanTramGiam IS NOT NULL AND SoTienGiam IS NULL)
        OR
        (PhanTramGiam IS NULL AND SoTienGiam IS NOT NULL)
    )
);

-- ========================
-- 17. ÁP KHUYẾN MÃI CHO TIỆC
-- ========================
CREATE TABLE KhuyenMaiDatSanh (
    MaDat INT,
    MaKhuyenMai INT,

    PRIMARY KEY (MaDat, MaKhuyenMai),

    FOREIGN KEY (MaDat) REFERENCES DatSanh(MaDat) ON DELETE CASCADE,
    FOREIGN KEY (MaKhuyenMai) REFERENCES KhuyenMai(MaKhuyenMai)
);

-- ========================
-- INDEX
-- ========================
CREATE INDEX IX_CheckTrung
ON DatSanh (MaSanh, NgayToChuc, GioBatDau, GioKetThuc);

CREATE INDEX IX_Ban_MaDat ON Ban(MaDat);
CREATE INDEX IX_DatSanh_MaKhachHang ON DatSanh(MaKhachHang);

CREATE INDEX IX_KM_MaDat ON KhuyenMaiDatSanh(MaDat);



-- ========================
-- CHI NHÁNH
-- ========================
INSERT INTO ChiNhanh (TenChiNhanh, DiaChi, SoDienThoai) VALUES
(N'Adora Quận 1', N'Q1, TP.HCM', '0900000001'),
(N'Adora Quận 3', N'Q3, TP.HCM', '0900000002'),
(N'Adora Gò Vấp', N'Gò Vấp, TP.HCM', '0900000003'),
(N'Adora Bình Thạnh', N'Bình Thạnh', '0900000004'),
(N'Adora Tân Bình', N'Tân Bình', '0900000005'),
(N'Adora Thủ Đức', N'Thủ Đức', '0900000006'),
(N'Adora Quận 7', N'Q7', '0900000007'),
(N'Adora Quận 10', N'Q10', '0900000008'),
(N'Adora Phú Nhuận', N'Phú Nhuận', '0900000009'),
(N'Adora Nhà Bè', N'Nhà Bè', '0900000010');

-- ========================
-- SẢNH
-- ========================
select * from Sanh
INSERT INTO Sanh (TenSanh, SucChua, GiaThue, MaChiNhanh) VALUES
(N'Sảnh Ruby', 200, 10000000, 1),
(N'Sảnh Diamond', 300, 15000000, 1),
(N'Sảnh Gold', 150, 8000000, 2),
(N'Sảnh Silver', 120, 6000000, 2),
(N'Sảnh Platinum', 400, 20000000, 3),
(N'Sảnh Pearl', 250, 12000000, 4),
(N'Sảnh Sapphire', 180, 9000000, 5),
(N'Sảnh Emerald', 220, 11000000, 6),
(N'Sảnh Topaz', 160, 8500000, 7),
(N'Sảnh Opal', 140, 7000000, 8);

UPDATE Sanh
SET HinhAnh = '/Images/sanh' + CAST(MaSanh AS NVARCHAR) + '.png';



-- ========================
-- KHÁCH HÀNG
-- ========================
INSERT INTO KhachHang (TenKhachHang, SoDienThoai, Email) VALUES
(N'Nguyễn Văn A', '0911111111', 'a@gmail.com'),
(N'Trần Thị B', '0922222222', 'b@gmail.com'),
(N'Lê Văn C', '0933333333', 'c@gmail.com'),
(N'Phạm Thị D', '0944444444', 'd@gmail.com'),
(N'Hoàng Văn E', '0955555555', 'e@gmail.com'),
(N'Võ Thị F', '0966666666', 'f@gmail.com'),
(N'Đặng Văn G', '0977777777', 'g@gmail.com'),
(N'Bùi Thị H', '0988888888', 'h@gmail.com'),
(N'Ngô Văn I', '0999999999', 'i@gmail.com'),
(N'Phan Văn K', '0901234567', 'k@gmail.com');

-- ========================
-- LOẠI SỰ KIỆN
-- ========================
INSERT INTO LoaiSuKien (TenLoai) VALUES
(N'Tiệc cưới'),(N'Sinh nhật'),(N'Hội nghị'),(N'Tiệc công ty'),
(N'Lễ kỷ niệm'),(N'Workshop'),(N'Tiệc tất niên'),
(N'Tiệc khai trương'),(N'Tiệc thôi nôi'),(N'Tiệc gia đình');

-- ========================
-- MENU
-- ========================
INSERT INTO Menu (TenMenu, LoaiMenu) VALUES
(N'Menu A', N'Cơ bản'),(N'Menu B', N'Cao cấp'),
(N'Menu C', N'Tiệc cưới'),(N'Menu D', N'Hội nghị'),
(N'Menu E', N'Sinh nhật'),(N'Menu F', N'Công ty'),
(N'Menu G', N'Tiệc nhẹ'),(N'Menu H', N'Buffet'),
(N'Menu I', N'VIP'),(N'Menu J', N'Truyền thống');

-- ========================
-- MÓN ĂN
-- ========================
INSERT INTO MonAn (TenMon, Gia) VALUES
(N'Gà nướng', 200000),(N'Bò lúc lắc', 250000),
(N'Tôm hấp', 300000),(N'Cá chiên', 180000),
(N'Lẩu thái', 350000),(N'Canh chua', 150000),
(N'Rau xào', 100000),(N'Cơm chiên', 120000),
(N'Mì xào', 130000),(N'Chè tráng miệng', 90000);

-- ========================
-- CHI TIẾT MENU
-- ========================
INSERT INTO ChiTietMenu VALUES
(1,1,10),(1,2,10),(2,3,8),(2,4,8),(3,5,6),
(3,6,6),(4,7,10),(5,8,10),(6,9,10),(7,10,12);

-- ========================
-- ĐẶT SẢNH
-- ========================
INSERT INTO DatSanh 
(MaSanh, MaKhachHang, MaLoai, MaMenu, TenSuKien, NgayToChuc, GioBatDau, GioKetThuc, TrangThai)
VALUES
(1,1,1,1,N'Đám cưới A','2026-05-01','17:00','21:00',N'Đã cọc'),
(2,2,2,2,N'Sinh nhật B','2026-05-02','18:00','21:00',N'Đang đặt'),
(3,3,3,3,N'Hội nghị C','2026-05-03','08:00','12:00',N'Hoàn thành'),
(4,4,4,4,N'Tiệc công ty D','2026-05-04','17:00','21:00',N'Đã cọc'),
(5,5,5,5,N'Kỷ niệm E','2026-05-05','18:00','22:00',N'Đang đặt'),
(6,6,6,6,N'Workshop F','2026-05-06','08:00','11:00',N'Hoàn thành'),
(7,7,7,7,N'Tất niên G','2026-05-07','18:00','22:00',N'Đã cọc'),
(8,8,8,8,N'Khai trương H','2026-05-08','09:00','12:00',N'Đang đặt'),
(9,9,9,9,N'Thôi nôi I','2026-05-09','17:00','20:00',N'Hoàn thành'),
(10,10,10,10,N'Tiệc gia đình K','2026-05-10','18:00','21:00',N'Đã hủy');

-- ========================
-- LỊCH SỬ ĐỔI LỊCH
-- ========================
INSERT INTO LichSuDoiLich (MaDat, NgayCu, NgayMoi, LyDo, TrangThai) VALUES
(1,'2026-05-01','2026-05-02',N'Bận',N'Đã duyệt'),
(2,'2026-05-02','2026-05-03',N'Mưa',N'Chờ'),
(3,'2026-05-03','2026-05-04',N'Khách yêu cầu',N'Đã duyệt');

-- ========================
-- BÀN
-- ========================
INSERT INTO Ban (MaDat, TenBan, SoKhach) VALUES
(1,N'Bàn 1',10),(1,N'Bàn 2',10),(2,N'Bàn 1',8),
(3,N'Bàn 1',12),(4,N'Bàn 1',10),(5,N'Bàn 1',9),
(6,N'Bàn 1',10),(7,N'Bàn 1',10),(8,N'Bàn 1',8),(9,N'Bàn 1',6);

-- ========================
-- NGUYÊN LIỆU
-- ========================
INSERT INTO NguyenLieu (TenNguyenLieu, DonVi) VALUES
(N'Thịt gà',N'kg'),(N'Thịt bò',N'kg'),
(N'Tôm',N'kg'),(N'Cá',N'kg'),(N'Rau',N'kg');

-- ========================
-- CÔNG THỨC
-- ========================
INSERT INTO CongThuc VALUES
(1,1,1.5),(2,2,1.2),(3,3,2),(4,4,1),(5,5,0.5);

-- ========================
-- DỊCH VỤ
-- ========================
INSERT INTO DichVu (TenDichVu, Gia) VALUES
(N'Âm thanh',2000000),(N'Ánh sáng',1500000),
(N'MC',3000000),(N'Trang trí',5000000),
(N'Chụp ảnh',4000000);

-- ========================
-- DỊCH VỤ SỰ KIỆN
-- ========================
INSERT INTO DichVuSuKien VALUES
(1,1,1),(1,2,1),(2,3,1),(3,4,1),(4,5,1);

-- ========================
-- HÓA ĐƠN
-- ========================
INSERT INTO HoaDon (MaDat, TongTien) VALUES
(1,50000000),(2,30000000),(3,20000000),
(4,45000000),(5,35000000);

-- ========================
-- KHUYẾN MÃI
-- ========================
INSERT INTO KhuyenMai 
(TenKhuyenMai, PhanTramGiam, SoTienGiam, NgayBatDau, NgayKetThuc, TrangThai)
VALUES
(N'Giảm 10%',10,NULL,'2026-01-01','2026-12-31',N'Hoạt động'),
(N'Giảm 5 triệu',NULL,5000000,'2026-01-01','2026-06-30',N'Hoạt động');

-- ========================
-- ÁP KHUYẾN MÃI
-- ========================
INSERT INTO KhuyenMaiDatSanh VALUES
(1,1),(2,2),(3,1);


select * from KhachHang
select * from ChiNhanh

ALTER TABLE Sanh ADD TenChiNhanh NVARCHAR(255);

-- Cập nhật dữ liệu
UPDATE s SET s.TenChiNhanh = c.TenChiNhanh
FROM Sanh s
JOIN ChiNhanh c ON s.MaChiNhanh = c.MaChiNhanh;

UPDATE Sanh SET TenChiNhanh = N'Adora Quận 1'   WHERE MaSanh = 1;
UPDATE Sanh SET TenChiNhanh = N'Adora Quận 1'   WHERE MaSanh = 2;
UPDATE Sanh SET TenChiNhanh = N'Adora Quận 3'   WHERE MaSanh = 3;
UPDATE Sanh SET TenChiNhanh = N'Adora Quận 3'   WHERE MaSanh = 4;
UPDATE Sanh SET TenChiNhanh = N'Adora Gò Vấp'   WHERE MaSanh = 5;
UPDATE Sanh SET TenChiNhanh = N'Adora Bình Thạnh' WHERE MaSanh = 6;
UPDATE Sanh SET TenChiNhanh = N'Adora Tân Bình'  WHERE MaSanh = 7;
UPDATE Sanh SET TenChiNhanh = N'Adora Thủ Đức'   WHERE MaSanh = 8;
UPDATE Sanh SET TenChiNhanh = N'Adora Quận 7'    WHERE MaSanh = 9;
UPDATE Sanh SET TenChiNhanh = N'Adora Quận 10'   WHERE MaSanh = 10;

-- =====================================================
-- XÓA RÀNG BUỘC DỮ LIỆU CŨ
-- =====================================================

DELETE FROM ChiTietMenu;
DELETE FROM CongThuc;
DELETE FROM DatSanh;

DELETE FROM Menu;
DELETE FROM MonAn;

-- RESET IDENTITY
DBCC CHECKIDENT ('Menu', RESEED, 0);
DBCC CHECKIDENT ('MonAn', RESEED, 0);

-- =====================================================
-- MENU
-- =====================================================

INSERT INTO Menu (TenMenu, LoaiMenu) VALUES
(N'Menu Chay Thanh Đạm', N'Chay'),
(N'Menu Chay Thiền Tâm', N'Chay'),
(N'Menu Chay Sen Vàng', N'Chay'),
(N'Menu Chay Bình An', N'Chay'),
(N'Menu Chay Hoa Sen', N'Chay'),

(N'Menu Hoàng Gia', N'Mặn'),
(N'Menu Ruby', N'Mặn'),
(N'Menu Diamond', N'Mặn'),
(N'Menu Sapphire', N'Mặn'),
(N'Menu Platinum', N'Mặn');

-- =====================================================
-- MÓN ĂN
-- =====================================================

INSERT INTO MonAn (TenMon, Gia) VALUES

-- CHAY
(N'Chả giò chay',120000),
(N'Đậu hũ sốt nấm',140000),
(N'Lẩu nấm chay',260000),
(N'Gỏi ngó sen chay',130000),
(N'Cơm chiên lá sen',150000),
(N'Mì xào rau củ',145000),
(N'Canh rong biển chay',110000),
(N'Bánh bao chay',100000),
(N'Súp bí đỏ chay',120000),
(N'Canh chua chay',115000),

-- MẶN
(N'Gà quay mật ong',320000),
(N'Bò sốt tiêu đen',350000),
(N'Tôm sú hấp bia',420000),
(N'Cá mú hấp Hong Kong',500000),
(N'Lẩu cua biển',550000),
(N'Sườn nướng BBQ',360000),
(N'Mực nướng sa tế',330000),
(N'Vịt quay Bắc Kinh',480000),
(N'Súp cua hải sản',220000),
(N'Lẩu thái hải sản',500000),
(N'Bò nướng đá',420000),
(N'Tôm nướng muối ớt',410000),
(N'Cua sốt Singapore',580000),
(N'Lẩu cá kèo',450000),
(N'Bò Mỹ áp chảo',620000);

-- =====================================================
-- CHI TIẾT MENU
-- =====================================================

INSERT INTO ChiTietMenu (MaMenu, MaMon) VALUES

-- MENU 1
(1,1),(1,2),(1,3),(1,4),(1,5),

-- MENU 2
(2,6),(2,7),(2,8),(2,9),(2,10),

-- MENU 3
(3,1),(3,3),(3,5),(3,7),(3,9),

-- MENU 4
(4,2),(4,4),(4,6),(4,8),(4,10),

-- MENU 5
(5,1),(5,6),(5,7),(5,8),(5,9),

-- MENU 6
(6,11),(6,12),(6,13),(6,14),(6,15),

-- MENU 7
(7,16),(7,17),(7,18),(7,19),(7,20),

-- MENU 8
(8,11),(8,13),(8,15),(8,17),(8,19),

-- MENU 9
(9,12),(9,14),(9,16),(9,18),(9,20),

-- MENU 10
(10,11),(10,12),(10,13),(10,14),(10,15);

select * from DatSanh

select * from KhachHang
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'

DBCC CHECKIDENT ('DatSanh', RESEED, 0);