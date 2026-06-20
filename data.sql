CREATE DATABASE ceramicshop;
GO

USE ceramicshop;
GO

CREATE TABLE Products (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(200)   NOT NULL,
    Description NVARCHAR(1000)  NULL,
    Category    NVARCHAR(100)   NOT NULL,
    IsActive    BIT             NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt   DATETIME2       NULL
);

CREATE TABLE ProductPriceHistories (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    ProductId     INT             NOT NULL,
    Price         DECIMAL(18,2)   NOT NULL,
    EffectiveFrom DATETIME2       NOT NULL,
    EffectiveTo   DATETIME2       NULL,
    Note          NVARCHAR(500)   NULL,
    CONSTRAINT FK_PriceHistory_Product FOREIGN KEY (ProductId)
        REFERENCES Products(Id) ON DELETE CASCADE
);

CREATE TABLE Orders (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    OrderCode     NVARCHAR(50)    NOT NULL UNIQUE,
    CustomerName  NVARCHAR(100)   NULL,
    CustomerPhone NVARCHAR(20)    NULL,
    Note          NVARCHAR(500)   NULL,
    OrderDate     DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
    TotalAmount   DECIMAL(18,2)   NOT NULL,
    CreatedAt     DATETIME2       NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE OrderDetails (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    OrderId   INT           NOT NULL,
    ProductId INT           NOT NULL,
    Quantity  INT           NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_OrderDetail_Order   FOREIGN KEY (OrderId)
        REFERENCES Orders(Id)   ON DELETE CASCADE,
    CONSTRAINT FK_OrderDetail_Product FOREIGN KEY (ProductId)
        REFERENCES Products(Id) ON DELETE NO ACTION
);

-- 2. THÊM DỮ LIỆU MẪU VÀO BẢNG PRODUCTS (6 Sản phẩm thuộc 3 danh mục)
SET IDENTITY_INSERT Products ON;
INSERT INTO Products (Id, Name, Description, Category, IsActive, CreatedAt) VALUES
(1, N'Bình Hoa Chu Đậu Cổ Điển', N'Bình hoa gốm Chu Đậu vẽ tay họa tiết truyền thống', N'Bình hoa', 1, '2026-01-01 00:00:00'),
(2, N'Bình Gốm Bát Tràng Men Hỏa Biến', N'Bình hoa gốm nghệ thuật men hỏa biến cao cấp', N'Bình hoa', 1, '2026-01-05 00:00:00'),
(3, N'Bộ Bát Đĩa Hoa Đào 12 Món', N'Bộ bát đĩa gốm sứ trắng vẽ hoa đào hồng chịu nhiệt', N'Bộ bát đĩa', 1, '2026-01-01 00:00:00'),
(4, N'Bộ Chén Trà Tử Sa Nghi Hưng', N'Bộ ấm chén trà làm bằng đất tử sa cao cấp', N'Bộ bát đĩa', 1, '2026-01-10 00:00:00'),
(5, N'Tượng Tỳ Hưu Gốm Ngọc', N'Tượng phong thủy gốm sứ cao cấp phủ men ngọc', N'Tượng trang trí', 1, '2026-01-01 00:00:00'),
(6, N'Tượng Thiền Sư Men Rạn', N'Tượng decor trang trí phòng trà men rạn độc đáo', N'Tượng trang trí', 1, '2026-01-15 00:00:00');
SET IDENTITY_INSERT Products OFF;
GO

-- 3. THÊM DỮ LIỆU LỊCH SỬ GIÁ (Mô phỏng sản phẩm có sự thay đổi giá theo thời gian)
-- Ví dụ: Sản phẩm 1 ban đầu giá 500k, sau tăng lên 550k từ tháng 3
-- Sản phẩm 3 ban đầu giá 1200k, sau giảm giá khuyến mãi xuống 1100k từ giữa tháng 2
SET IDENTITY_INSERT ProductPriceHistories ON;
INSERT INTO ProductPriceHistories (Id, ProductId, Price, EffectiveFrom, EffectiveTo, Note) VALUES
-- Bình Hoa Chu Đậu Cổ Điển
(1, 1, 500000.00, '2026-01-01 00:00:00', '2026-02-28 23:59:59', N'Giá mở bán năm mới'),
(2, 1, 550000.00, '2026-03-01 00:00:00', NULL, N'Điều chỉnh tăng giá do chi phí sản xuất'),

-- Bình Gốm Bát Tràng Men Hỏa Biến
(3, 2, 750000.00, '2026-01-05 00:00:00', NULL, N'Giá gốc niêm yết'),

-- Bộ Bát Đĩa Hoa Đào 12 Món
(4, 3, 1200000.00, '2026-01-01 00:00:00', '2026-02-14 23:59:59', N'Giá niêm yết gốc'),
(5, 3, 1100000.00, '2026-02-15 00:00:00', NULL, N'Giá chiến dịch giảm giá sau Tết'),

-- Bộ Chén Trà Tử Sa Nghi Hưng
(6, 4, 850000.00, '2026-01-10 00:00:00', NULL, N'Giá niêm yết gốc'),

-- Tượng Tỳ Hưu Gốm Ngọc
(7, 5, 2000000.00, '2026-01-01 00:00:00', NULL, N'Giá vật phẩm phong thủy'),

-- Tượng Thiền Sư Men Rạn
(8, 6, 450000.00, '2026-01-15 00:00:00', NULL, N'Giá niêm yết gốc');
SET IDENTITY_INSERT ProductPriceHistories OFF;
GO

-- 4. THÊM DỮ LIỆU ĐƠN HÀNG (4 Hóa đơn phân bổ rải rác từ tháng 1 đến tháng 3)
SET IDENTITY_INSERT Orders ON;
INSERT INTO Orders (Id, OrderCode, CustomerName, CustomerPhone, Note, OrderDate, TotalAmount, CreatedAt) VALUES
(1, 'HD001', N'Nguyễn Văn Anh', '0912345678', N'Giao hàng giờ hành chính', '2026-01-15 10:30:00', 1700000.00, '2026-01-15 10:30:00'),
(2, 'HD002', N'Trần Thị Bình', '0988888888', N'Hàng dễ vỡ, bọc kỹ xốp hơi', '2026-02-20 14:15:00', 3100000.00, '2026-02-20 14:15:00'),
(3, 'HD003', N'Lê Hoàng Cường', '0905556666', N'Khách quen mua làm quà tặng', '2026-03-05 09:00:00', 1400000.00, '2026-03-05 09:00:00'),
(4, 'HD004', N'Phạm Minh Đức', '0934112233', N'Giao gấp trong ngày', '2026-03-12 16:45:00', 2550000.00, '2026-03-12 16:45:00');
SET IDENTITY_INSERT Orders OFF;
GO

-- 5. THÊM CHI TIẾT ĐƠN HÀNG (Mỗi hóa đơn chứa các sản phẩm khác nhau)
SET IDENTITY_INSERT OrderDetails ON;
INSERT INTO OrderDetails (Id, OrderId, ProductId, Quantity, UnitPrice) VALUES
-- HD001 (Ngày 15/01): 1 Bình Chu Đậu (Giá lúc này 500k) + 1 Bộ Bát đĩa Hoa Đào (Giá lúc này 1200k) -> Tổng 1700k
(1, 1, 1, 1, 500000.00),
(2, 1, 3, 1, 1200000.00),

-- HD002 (Ngày 20/02): 2 Bộ Bát đĩa Hoa Đào (Giá sau giảm 1100k = 2200k) + 1 Bộ Chén Trà Tử Sa (850k) -> Tổng 3050k ~ 3100k (kèm phụ phí ship tròn tiền)
(3, 2, 3, 2, 1100000.00),
(4, 2, 4, 1, 850000.00),

-- HD003 (Ngày 05/03): 1 Bình Chu Đậu (Giá sau khi tăng ngày 1/3 là 550k) + 1 Bộ Chén Trà Tử Sa (850k) -> Tổng 1400k
(5, 3, 1, 1, 550000.00),
(6, 3, 4, 1, 850000.00),

-- HD004 (Ngày 12/03): 3 Bình Gốm Bát Tràng (3 x 750k = 2250k) + 1 Tượng Thiền Sư (300k - khuyến mãi đặc biệt lúc mua) -> Tổng 2550k
(7, 4, 2, 3, 750000.00),
(8, 4, 6, 1, 300000.00);
SET IDENTITY_INSERT OrderDetails OFF;
GO

GO