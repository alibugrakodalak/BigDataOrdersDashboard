using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BigDataOrdersDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardMainChartComponentPartial : ViewComponent
    {
        private readonly BigDataOrdersDbContext _context;

        public _DashboardMainChartComponentPartial(BigDataOrdersDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {

            #region SatışRaporDay
            var today = DateTime.Today;

            var todaySales = _context.Orders
                                     .Where(o => o.OrderDate >= today && o.OrderDate < today.AddDays(1) && o.OrderStatus == "Tamamlandı")
                                     .Join(_context.Products,
                                            order => order.ProductId,
                                            product => product.ProductId,
                                            (order, product) => new { order.Quantity, product.UnitPrice })
                                     .Sum(x => x.Quantity * x.UnitPrice);

            ViewBag.TodaySales = todaySales;
            #endregion

            #region ToplamCiro
            DateTime YearStart = new DateTime(2023, 1, 1);
            DateTime YearEnd = new DateTime(2025, 12, 31);

            var yearTotal = _context.Orders
                .Where(o => o.OrderDate >= YearStart && o.OrderDate <= YearEnd && o.OrderStatus == "Tamamlandı")
                .Join(_context.Products,
                      o => o.ProductId,
                      p => p.ProductId,
                      (o, p) => o.Quantity * p.UnitPrice)
                .Sum();

            ViewBag.YearTotal = yearTotal;
            #endregion

            #region ToplamKar

            var profit = _context.Orders
                                 .Where(o => o.OrderDate >= YearStart && o.OrderDate <= YearEnd && o.OrderStatus == "Tamamlandı")
                                 .Join(_context.Products,
                                    o => o.ProductId,
                                    p => p.ProductId,
                                    (o, p) => o.Quantity * (p.UnitPrice - (p.UnitPrice * 0.40m)))
                                 .Sum();

            ViewBag.Profit = profit;

            #endregion

            #region Grafik

            var sixMonthsAgo = DateTime.Today.AddMonths(-6);

            var monthlySalesRaw = _context.Orders
                .Where(o => o.OrderDate >= sixMonthsAgo)
                .GroupBy(o => new {
                    o.OrderDate.Year,
                    o.OrderDate.Month,
                    o.OrderStatus
                })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Durum = g.Key.OrderStatus,
                    SatisAdedi = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            var monthlySales = monthlySalesRaw.Select(x => new
            {
                Ay = $"{x.Year}-{x.Month:D2}",
                x.Durum,
                x.SatisAdedi
            }).ToList();

            ViewBag.MonthlySalesJson = System.Text.Json.JsonSerializer.Serialize(monthlySales);

            #endregion

            return View();
        }
    }
}
