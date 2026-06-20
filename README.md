# 🏺 CeramicShop API

Hệ thống quản lý bán hàng cho cửa hàng gốm sứ thủ công — xây dựng bằng **C# .NET 6 Web API** + **Entity Framework Core** + **SQL Server**.

---

## 📐 Thiết kế Database

### Sơ đồ ERD

```
┌───────────────────────────┐         ┌────────────────────────────────┐
│         Products          │         │      ProductPriceHistories      │
├───────────────────────────┤         ├────────────────────────────────┤
│ Id          INT (PK)      │◄────────│ Id            INT (PK)         │
│ Name        NVARCHAR(200) │  1───N  │ ProductId     INT (FK)         │
│ Description NVARCHAR(1000)│         │ Price         DECIMAL(18,2)    │
│ Category    NVARCHAR(100) │         │ EffectiveFrom DATETIME2        │
│ IsActive    BIT           │         │ EffectiveTo   DATETIME2 (NULL) │
│ CreatedAt   DATETIME2     │         │ Note          NVARCHAR(500)    │
│ UpdatedAt   DATETIME2     │         └────────────────────────────────┘
└───────────────────────────┘
         │ 1
         │ N
┌────────┴──────────┐         ┌──────────────────────────────┐
│    OrderDetails   │         │           Orders             │
├───────────────────┤         ├──────────────────────────────┤
│ Id        INT(PK) │  N───1  │ Id            INT (PK)       │
│ OrderId   INT(FK) │────────►│ OrderCode     NVARCHAR(50)   │
│ ProductId INT(FK) │         │ CustomerName  NVARCHAR(100)  │
│ Quantity  INT     │         │ CustomerPhone NVARCHAR(20)   │
│ UnitPrice DECIMAL │         │ Note          NVARCHAR(500)  │
└───────────────────┘         │ OrderDate     DATETIME2      │
                              │ TotalAmount   DECIMAL(18,2)  │
                              │ CreatedAt     DATETIME2      │
                              └──────────────────────────────┘
```

### Mô tả các bảng

| Bảng | Mục đích |
|------|----------|
| **Products** | Danh mục sản phẩm gốm sứ. Dùng `IsActive = false` để xoá mềm, bảo toàn lịch sử bán hàng. |
| **ProductPriceHistories** | Lịch sử thay đổi giá. Mỗi lần thay giá tạo bản ghi mới, `EffectiveTo = NULL` là giá đang hiện hành. |
| **Orders** | Một giao dịch bán hàng. `OrderCode` duy nhất theo ngày (ORD-YYYYMMDD-NNN). |
| **OrderDetails** | Chi tiết dòng sản phẩm. `UnitPrice` lưu **snapshot giá lúc bán** — độc lập với thay đổi giá sau đó. |

### Quyết định thiết kế quan trọng

- **Lịch sử giá (ProductPriceHistories):** Tách thành bảng riêng để truy vết giá tại bất kỳ thời điểm nào trong quá khứ.
- **Snapshot giá (UnitPrice):** Khi tạo đơn hàng, giá tại thời điểm đó được lưu vào `UnitPrice`. Dù giá sản phẩm thay đổi sau này, báo cáo lịch sử vẫn chính xác.
- **Soft Delete:** Sản phẩm không bao giờ bị xoá thật (`IsActive = false`), tránh mất dữ liệu lịch sử đơn hàng.

---

## 🚀 Hướng dẫn chạy chương trình

### Yêu cầu

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- SQL Server (LocalDB, Express, hoặc Full)
- Visual Studio 2022 hoặc VS Code

### Các bước

```bash
# 1. Clone repository
git clone https://github.com/Hoanhuy2003/ceramicshop.git
cd ceramic

# 2. Cấu hình connection string
# Mở appsettings.json và sửa DefaultConnection theo SQL Server của bạn
"DefaultConnection": "Server=YOUR_SERVER;Database=ceramicshop;User ID=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;"

# 3. Tạo database — mở SSMS và chạy file data.sql

# 4. Chạy ứng dụng
dotnet run

# 5. Mở Swagger UI tại
https://localhost:7110/swagger
```

---

## 📋 Danh sách API đã hoàn thành

### 🗂️ Quản lý sản phẩm `/api/products`

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/products` | Danh sách sản phẩm |
| GET | `/api/products/{id}` | Chi tiết sản phẩm theo Id |
| POST | `/api/products` | Tạo sản phẩm mới (kèm giá ban đầu) |
| PUT | `/api/products/{id}` | Cập nhật thông tin sản phẩm |
| DELETE | `/api/products/{id}` | Xoá mềm sản phẩm |
| PATCH | `/api/products/{id}/price` | Cập nhật giá → tạo bản ghi lịch sử giá mới |
| GET | `/api/products/{id}/price-history` | Xem lịch sử thay đổi giá |
| GET | `/api/products/categories` | Danh sách danh mục hiện có |

### 🛍️ Nghiệp vụ bán hàng `/api/orders`

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| POST | `/api/orders` | Tạo đơn hàng (1 lần mua, nhiều sản phẩm) |
| GET | `/api/orders/{id}` | Chi tiết đơn hàng theo Id |
| GET | `/api/orders/code/{orderCode}` | Chi tiết đơn hàng theo mã |
| GET | `/api/orders` | Danh sách đơn hàng (lọc theo ngày) |

### 📊 Thống kê `/api/statistics`

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| GET | `/api/statistics/sales-quantity` | Thống kê sản lượng theo khoảng thời gian |
| GET | `/api/statistics/product-revenue` | Doanh số theo tên sản phẩm + khoảng thời gian |

---

## 📦 Ví dụ Request/Response

### Tạo sản phẩm mới
```json
POST /api/products
{
  "name": "Ấm trà gốm Bát Tràng",
  "description": "Ấm trà gốm men ngọc thủ công",
  "category": "Ấm trà",
  "price": 350000
}
```

### Tạo đơn hàng
```json
POST /api/orders
{
  "customerName": "Nguyễn Văn A",
  "customerPhone": "0901234567",
  "items": [
    { "productId": 1, "quantity": 2 },
    { "productId": 3, "quantity": 4 }
  ]
}
```

### Thống kê sản lượng
```
GET /api/statistics/sales-quantity?from=2024-06-01&to=2024-06-19
```

### Doanh số theo sản phẩm
```
GET /api/statistics/product-revenue?productName=ấm trà&from=2024-01-01&to=2024-06-19
```

---

## 🏗️ Kiến trúc

Luồng xử lý:
```
Controller → Service → Repository → DbContext → Database
```

Cấu trúc thư mục:
```
ceramic/
├── Controllers/        # Nhận request, trả response
│   ├── ProductsController.cs
│   ├── OrdersController.cs
│   └── StatisticsController.cs
├── Data/               # AppDbContext - kết nối database
│   └── AppDbContext.cs
├── DTOs/               # Request/Response models
│   ├── ApiResponse.cs
│   ├── ProductDto.cs
│   ├── OrderDto.cs
│   └── StatisticDto.cs
├── Models/             # Entity classes
│   ├── Product.cs
│   ├── ProductPriceHistory.cs
│   ├── Order.cs
│   └── OrderDetail.cs
├── Repositories/       # Truy vấn database
│   ├── IProductRepository.cs
│   ├── ProductRepository.cs
│   ├── IOrderRepository.cs
│   ├── OrderRepository.cs
│   ├── IStatisticsRepository.cs
│   └── StatisticsRepository.cs
├── Services/           # Business logic
│   ├── ProductService.cs
│   ├── OrderService.cs
│   └── StatisticsService.cs
├── Program.cs
└── appsettings.json
```

---

## 🔧 Công nghệ sử dụng

- **C# .NET 6**
- **ASP.NET Core Web API**
- **Entity Framework Core 6**
- **SQL Server**
- **Swagger / Swashbuckle**