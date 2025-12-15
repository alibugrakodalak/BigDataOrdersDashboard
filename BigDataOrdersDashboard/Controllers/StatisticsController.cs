using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Xml.Linq;

namespace BigDataOrdersDashboard.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly BigDataOrdersDbContext _context;

        public StatisticsController(BigDataOrdersDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {            

            ViewBag.ProductCount = _context.Products.Count().ToString("N0", new CultureInfo("tr-TR"));
            ViewBag.BestCountry = _context.Orders
                                          .Include(o => o.Customer)
                                          .GroupBy(o => o.Customer.CustomerCountry)
                                          .OrderByDescending(g => g.Count())
                                          .Select(g => g.Key)
                                          .FirstOrDefault();
            ViewBag.CustomerCount = _context.Customers.Count().ToString("N0", new CultureInfo("tr-TR"));
            ViewBag.OrderCount = _context.Orders.Count().ToString("N0", new CultureInfo("tr-TR"));

            ViewBag.CustomersCountry = _context.Customers.Select(x => x.CustomerCountry).Distinct().Count();
            ViewBag.CustomersCity = _context.Customers.Select(x => x.CustomerCity).Distinct().Count();
            ViewBag.OrderStatusByCompleted = _context.Orders.Where(x => x.OrderStatus == "Tamamlandı").Count().ToString("N0", new CultureInfo("tr-TR"));
            ViewBag.OrderStatusByWaiting = _context.Orders.Where(x => x.OrderStatus == "Beklemede").Count().ToString("N0", new CultureInfo("tr-TR"));

            ViewBag.TotalStock = _context.Products.Select(x => x.StockQuantity).Sum().ToString("N0", new CultureInfo("tr-TR"));
            ViewBag.NovemberOrders = _context.Orders.FromSqlRaw("Select * From Orders WHERE OrderDate >= '2025-11-01' AND OrderDate < '2025-11-30'").Count().ToString("N0", new CultureInfo("tr-TR"));
            ViewBag.Orders2025Count = _context.Orders.Where(y => y.OrderDate.Year == 2025).Count().ToString("N0", new CultureInfo("tr-TR"));

            ViewBag.TotalOrderAmount = _context.Orders
                .Include(o => o.Product)
                .Select(o => o.Quantity * o.Product.UnitPrice)
                .Sum()
                .ToString("N0", new CultureInfo("tr-TR")) + " €";

            return View();
        }

        public IActionResult TextStatistic()
        {
            // ----------------------------------- 1-4 -----------------------------------

            // 1.En Çok Sipariş Gelen Ülke 
            ViewBag.MostOrderCountry = _context.Orders
                                          .Include(o => o.Customer)
                                          .GroupBy(o => o.Customer.CustomerCountry)
                                          .OrderByDescending(g => g.Count())
                                          .Select(g => g.Key)
                                          .FirstOrDefault();

            // 2.En Çok Sipariş Gelen Şehir
            ViewBag.MostOrderCity = _context.Orders
                                          .Include(o => o.Customer)
                                          .GroupBy(o => o.Customer.CustomerCity)
                                          .OrderByDescending(g => g.Count())
                                          .Select(g => g.Key)
                                          .FirstOrDefault();

            // 3. En Çok Satılan Kategori
            ViewBag.TopCategory = _context.Orders
                                          .GroupBy(o => o.Product.Category.CategoryName)
                                          .Select(g => new
                                          {
                                              CategoryName = g.Key,
                                              TotalSales = g.Sum(x => x.Quantity)
                                          })
                                          .OrderByDescending(x => x.TotalSales)
                                          .Select(x => x.CategoryName)
                                          .FirstOrDefault();

            // 4. En Çok Satan Ürün
            ViewBag.TopOrderedProduct = _context.Orders
                                                .GroupBy(o => o.Product.ProductName)
                                                .Select(g => new
                                                {
                                                    ProductName = g.Key,
                                                    TotalQuantity = g.Sum(o => o.Quantity)
                                                })
                                                .OrderByDescending(x => x.TotalQuantity)
                                                .Select(x => x.ProductName)
                                                .FirstOrDefault();

            // ----------------------------------- 5-8 -----------------------------------

            // 5. En Yüksek Fiyatlı Ürün
            ViewBag.MostExpensiveProduct = _context.Products
                                                   .Where(x => x.UnitPrice == (_context.Products.Max(x => x.UnitPrice)))
                                                   .Select(y => y.ProductName)
                                                   .FirstOrDefault();

            // 6. En Düşük Fiyatlı Ürün
            ViewBag.CheapestProduct = _context.Products
                                              .Where(x => x.UnitPrice == (_context.Products.Min(x => x.UnitPrice)))
                                              .Select(y => y.ProductName)
                                              .FirstOrDefault();

            // 7. En Yüksek Stoklu Ürün
            ViewBag.TopStockProduct = _context.Products
                                              .OrderByDescending(x => x.StockQuantity)
                                              .Take(1)
                                              .Select(y => y.ProductName)
                                              .FirstOrDefault();

            // 8. En Az Stoklu Ürün
            ViewBag.LowestStockProduct = _context.Products
                                                 .OrderBy(x => x.StockQuantity)
                                                 .Take(1)
                                                 .Select(y => y.ProductName)
                                                 .FirstOrDefault();

            // ----------------------------------- 9-12 -----------------------------------

            // 9. En Az Satan Kategori
            ViewBag.LowCategory = _context.Orders
                                          .GroupBy(o => o.Product.Category.CategoryName)
                                          .Select(g => new
                                          {
                                              CategoryName = g.Key,
                                              TotalSales = g.Sum(x => x.Quantity)
                                          })
                                          .OrderBy(x => x.TotalSales)
                                          .Select(x => x.CategoryName)
                                          .FirstOrDefault();

            // 10. En Az Satan Ürün
            ViewBag.MinOrderedProduct = _context.Orders
                                                .GroupBy(o => o.Product.ProductName)
                                                .Select(g => new
                                                {
                                                    ProductName = g.Key,
                                                    TotalQuantity = g.Sum(o => o.Quantity)
                                                })
                                                .OrderBy(x => x.TotalQuantity)
                                                .Select(x => x.ProductName)
                                                .FirstOrDefault();

            // 11. En Çok İade Alan Ürün
            ViewBag.TopReturnedProduct = _context.Orders
                                                  .Where(o => o.OrderStatus == "iptal Edildi")
                                                  .GroupBy(o => o.Product.ProductName) 
                                                  .Select(g => new
                                                  {
                                                      ProductName = g.Key,
                                                      ReturnCount = g.Count() 
                                                  })
                                                  .OrderByDescending(x => x.ReturnCount)
                                                  .Select(x => x.ProductName)
                                                  .FirstOrDefault();

            // 12. Son Eklenen Ürün
            ViewBag.LastAddedProduct = _context.Products
                                               .OrderByDescending(x => x.ProductId)
                                               .Take(1)
                                               .Select(y => y.ProductName)
                                               .FirstOrDefault();

            // ----------------------------------- 13-16 -----------------------------------

            // 13. İlk Eklenen Müşteri
            ViewBag.FirstAddedCustomer = _context.Customers
                                                 .OrderBy(x => x.CustomerId)
                                                 .Take(1)
                                                 .Select(y => y.CustomerName + " " + y.CustomerSurname)
                                                 .FirstOrDefault();

            // 14. Son Eklenen Müşteri
            ViewBag.LastAddedCustomer = _context.Customers
                                                .OrderByDescending(x => x.CustomerId)
                                                .Take(1)
                                                .Select(y => y.CustomerName + " " + y.CustomerSurname)
                                                .FirstOrDefault();

            // 15. En Aktif Müşteri
            ViewBag.TopCustomer = _context.Orders
                                                         .GroupBy(o => new { o.Customer.CustomerName, o.Customer.CustomerSurname })
                                                         .Select(g => new
                                                         {
                                                             FullName = g.Key.CustomerName + " " + g.Key.CustomerSurname,
                                                             TotalOrders = g.Count()
                                                         })
                                                         .OrderByDescending(x => x.TotalOrders)
                                                         .Select(x => x.FullName)
                                                         .FirstOrDefault();

            // 16. En Popüler Ödeme Yöntemi
            ViewBag.TopPaymentMethod = _context.Orders
                                               .GroupBy(o => o.PaymentMethod)
                                               .Select(g => new
                                               {
                                                   PaymentMethod = g.Key,
                                                   TotalOrders = g.Count()
                                               })
                                               .OrderByDescending(x => x.TotalOrders)
                                               .Select(y => y.PaymentMethod)
                                               .FirstOrDefault();     

            return View();
        }
    }
}
