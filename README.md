# ⚔️ BÀI THI VÒNG 4 - Xây dựng hệ thống quản lý tồn kho đa cửa hàng

> ⏱️ **Thời gian:** 72 giờ kể từ khi tạo repo
> 🎯 **Mục tiêu:** Code đúng yêu cầu, pass CI/CD, được merge thì qua vòng

---

<img src='http://103.98.152.198:9000/requirement-images/a84a67ed-a4cd-4eef-a8d7-c8e8a5d73bb9_2024-05-19_10-15-29.png' alt='Hình ảnh minh họa' style='max-width:100%; margin: 10px 0; border-radius: 8px; border: 1px solid #333;' />
<img src='http://103.98.152.198:9000/requirement-images/5b51ffc9-dd69-42ae-a779-31f27d1de0b4_2024-05-21_16-27-39.png' alt='Hình ảnh minh họa' style='max-width:100%; margin: 10px 0; border-radius: 8px; border: 1px solid #333;' />
<img src='http://103.98.152.198:9000/requirement-images/1216ed3f-e77c-4d57-8a6f-27ca6b2730c8_2024-05-21_16-35-02.png' alt='Hình ảnh minh họa' style='max-width:100%; margin: 10px 0; border-radius: 8px; border: 1px solid #333;' />

## 📋 YÊU CẦU NGHIỆP VỤ

Xây dựng hệ thống quản lý tồn kho cho chuỗi cửa hàng bán lẻ với các yêu cầu sau:

Bối cảnh:
Một chuỗi cửa hàng có nhiều chi nhánh, mỗi chi nhánh bán nhiều sản phẩm. Hệ thống cần quản lý số lượng tồn kho theo từng cửa hàng, hỗ trợ nhập hàng, bán hàng, kiểm tra tồn kho khi bán, và cảnh báo khi hàng sắp hết.

Yêu cầu chức năng:

Quản lý sản phẩm: Thêm, sửa, xóa sản phẩm (mã SP, tên SP, giá, mô tả)

Quản lý cửa hàng: Thêm, sửa, xóa cửa hàng (mã cửa hàng, tên, địa chỉ)

Nhập kho: Cập nhật số lượng tồn khi nhập hàng về từng cửa hàng

Bán hàng: Khi bán hàng, kiểm tra tồn kho đủ không, nếu đủ thì trừ tồn kho, nếu không thì báo lỗi

Kiểm tra tồn kho: Xem số lượng tồn của 1 sản phẩm tại 1 cửa hàng

Cảnh báo tồn kho thấp: Tự động gửi cảnh báo khi sản phẩm có số lượng tồn dưới ngưỡng (mặc định 10)

Ví dụ nghiệp vụ:

Cửa hàng A có sản phẩm "Áo thun đen" tồn 20 cái

Khách mua 5 cái → kiểm tra 20 >= 5 → trừ còn 15 → thành công

Khách mua 30 cái → kiểm tra 15 < 30 → báo lỗi "Không đủ hàng"

---

##  YÊU CẦU KỸ THUẬT

- Sử dụng Entity Framework (Database First) để thao tác với SQL Server
- Thiết kế database chuẩn hóa (3NF), có quan hệ giữa các bảng
- Sử dụng Repository Pattern để tách biệt logic truy vấn dữ liệu
- Sử dụng Unit of Work để quản lý transaction khi nhập/bán hàng
- Viết Unit Test cho các logic quan trọng (kiểm tra tồn kho, nhập/bán hàng)
- Xử lý Concurrency (khi nhiều người cùng bán sản phẩm cuối cùng)
- Log đầy đủ các hành động (nhập hàng, bán hàng, cảnh báo)
- API trả về JSON chuẩn RESTful
- Có xử lý exception và validate dữ liệu đầu vào

---

## ✅ ĐIỀU KIỆN PASS

- [ ] Thiết kế database đúng chuẩn, có khóa chính, khóa ngoại
- [ ] Code chạy đúng tất cả chức năng yêu cầu
- [ ] 100% unit tests PASS (kiểm tra logic nghiệp vụ)
- [ ] Xử lý đúng transaction (nhập/bán hàng phải là atomic)
- [ ] Xử lý đúng concurrency (không bị oversell)
- [ ] CI/CD pipeline xanh (build + test thành công)
- [ ] Code review đạt yêu cầu (clean code, design pattern)
- [ ] Có log đầy đủ (dùng log4net hoặc serilog)

---

## 💡 GỢI Ý

> 💡 Thiết kế database:
> 💡 Products: ProductID, ProductCode, ProductName, Price, Description
> 💡 Stores: StoreID, StoreCode, StoreName, Address
> 💡 Inventories: InventoryID, StoreID, ProductID, Quantity, MinThreshold
> 💡 InventoryTransactions: TransactionID, StoreID, ProductID, Quantity, Type (IMPORT/SALE), CreatedAt, CreatedBy
> 💡 Xử lý concurrency: Dùng rowversion hoặc lock khi cập nhật tồn kho
> 💡 Transaction scope: Dùng TransactionScope hoặc DbContext.Database.BeginTransaction()
> 💡 Cảnh báo tồn kho thấp: Dùng BackgroundService hoặc trigger sau mỗi lần bán hàng

---

## 📌 CODE MẪU THAM KHẢO

// Repository Pattern
public interface IInventoryRepository
{
    Task<int> GetStockAsync(int storeId, int productId);
    Task<bool> CheckStockAsync(int storeId, int productId, int quantity);
    Task<bool> ImportAsync(int storeId, int productId, int quantity, string userId);
    Task<bool> SaleAsync(int storeId, int productId, int quantity, string userId);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int storeId, int threshold = 10);
}

public class InventoryRepository : IInventoryRepository
{
    private readonly YourDbContext _context;
    
    public InventoryRepository(YourDbContext context)
    {
        _context = context;
    }
    
    public async Task<int> GetStockAsync(int storeId, int productId)
    {
        var inventory = await _context.Inventories
            .FirstOrDefaultAsync(i => i.StoreID == storeId && i.ProductID == productId);
        
        return inventory?.Quantity ?? 0;
    }
    
    public async Task<bool> CheckStockAsync(int storeId, int productId, int quantity)
    {
        var currentStock = await GetStockAsync(storeId, productId);
        return currentStock >= quantity;
    }
    
    public async Task<bool> ImportAsync(int storeId, int productId, int quantity, string userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.StoreID == storeId && i.ProductID == productId);
            
            if (inventory == null)
            {
                inventory = new Inventory
                {
                    StoreID = storeId,
                    ProductID = productId,
                    Quantity = quantity,
                    MinThreshold = 10
                };
                _context.Inventories.Add(inventory);
            }
            else
            {
                inventory.Quantity += quantity;
            }
            
            // Ghi log transaction
            var log = new InventoryTransaction
            {
                StoreID = storeId,
                ProductID = productId,
                Quantity = quantity,
                Type = "IMPORT",
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };
            _context.InventoryTransactions.Add(log);
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<bool> SaleAsync(int storeId, int productId, int quantity, string userId)
    {
        // Kiểm tra tồn kho
        if (!await CheckStockAsync(storeId, productId, quantity))
            return false;
        
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.StoreID == storeId && i.ProductID == productId);
            
            inventory.Quantity -= quantity;
            
            // Ghi log transaction
            var log = new InventoryTransaction
            {
                StoreID = storeId,
                ProductID = productId,
                Quantity = quantity,
                Type = "SALE",
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };
            _context.InventoryTransactions.Add(log);
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            // Kiểm tra cảnh báo tồn kho thấp
            if (inventory.Quantity <= inventory.MinThreshold)
            {
                // Gửi cảnh báo (có thể dùng SignalR, Email, hoặc lưu vào bảng Alert)
                await SendLowStockAlert(storeId, productId, inventory.Quantity);
            }
            
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int storeId, int threshold = 10)
    {
        var query = from i in _context.Inventories
                    join p in _context.Products on i.ProductID equals p.ProductID
                    where i.StoreID == storeId && i.Quantity <= i.MinThreshold
                    select p;
        
        return await query.ToListAsync();
    }
    
    private async Task SendLowStockAlert(int storeId, int productId, int currentStock)
    {
        // Logic gửi cảnh báo
        var product = await _context.Products.FindAsync(productId);
        var store = await _context.Stores.FindAsync(storeId);
        
        // Log cảnh báo
        Console.WriteLine($"[ALERT] Store {store.StoreName} - Product {product.ProductName} low stock: {currentStock}");
        // Có thể gửi email hoặc SignalR
    }
}

// Unit Test
[TestClass]
public class InventoryTests
{
    [TestMethod]
    public async Task Sale_Should_Reduce_Stock_When_Sufficient()
    {
        // Arrange
        var repository = new InventoryRepository(_context);
        var storeId = 1;
        var productId = 1;
        var initialStock = 20;
        var saleQuantity = 5;
        
        await repository.ImportAsync(storeId, productId, initialStock, "admin");
        
        // Act
        var result = await repository.SaleAsync(storeId, productId, saleQuantity, "customer");
        var remainingStock = await repository.GetStockAsync(storeId, productId);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(15, remainingStock);
    }
    
    [TestMethod]
    public async Task Sale_Should_Fail_When_Insufficient_Stock()
    {
        // Arrange
        var repository = new InventoryRepository(_context);
        var storeId = 1;
        var productId = 1;
        var initialStock = 5;
        var saleQuantity = 10;
        
        await repository.ImportAsync(storeId, productId, initialStock, "admin");
        
        // Act
        var result = await repository.SaleAsync(storeId, productId, saleQuantity, "customer");
        var remainingStock = await repository.GetStockAsync(storeId, productId);
        
        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(5, remainingStock); // Stock không đổi
    }
}
📊 VÍ DỤ ĐẦU RA
text
API Response khi bán hàng thành công:

json
{
    "success": true,
    "message": "Bán hàng thành công",
    "data": {
        "storeId": 1,
        "storeName": "Cửa hàng A",
        "productId": 100,
        "productName": "Áo thun đen",
        "quantitySold": 5,
        "remainingStock": 15,
        "totalAmount": 750000,
        "transactionId": "TXN-20240101-0001",
        "timestamp": "2024-01-01T10:30:00Z"
    }
}
API Response khi bán hàng thất bại (thiếu tồn kho):

json
{
    "success": false,
    "message": "Bán hàng thất bại: Không đủ hàng tồn kho",
    "error": {
        "code": "INSUFFICIENT_STOCK",
        "productId": 100,
        "productName": "Áo thun đen",
        "requestedQuantity": 30,
        "availableStock": 15
    }
}
API Response kiểm tra tồn kho:

json
{
    "success": true,
    "data": {
        "storeId": 1,
        "storeName": "Cửa hàng A",
        "productId": 100,
        "productName": "Áo thun đen",
        "currentStock": 15,
        "minThreshold": 10,
        "isLowStock": false
    }
}
💰 THÔNG TIN THƯỞNG
Khoản mục	Giá trị
Thưởng gốc	0đ
Phạt mỗi ngày trễ	0đ
Cọc	0đ (0%)
⚠️ QUY ĐỊNH
🔴 Quá deadline:  không qua vòng

🔴 Code ẩu: Bị reject

🟢 Code sạch, đúng yêu cầu: Qua vòng 4 + mở unlock Task 
  cho phép bạn chính thức được quyền nhận Task để code kiếm tiền trên nền tảng của VGO

Chúc bạn hoàn thành bài thi xuất sắc! ⚔️