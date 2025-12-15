using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardKpiComponentPartial : ViewComponent
    {
        private readonly BigDataOrdersDbContext _context;

        public _DashboardKpiComponentPartial(BigDataOrdersDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            # region MonthlyTarget
            DateTime startDate = new DateTime(2025, 12, 1);
            DateTime endDate = new DateTime(2025, 12, 31);

            decimal monthlyTarget = 20_000_000;

            var monthlyTotal = _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Join(_context.Products,
                      o => o.ProductId,
                      p => p.ProductId,
                      (o, p) => o.Quantity * p.UnitPrice)
                .Sum();

            decimal percentage = monthlyTarget == 0
                ? 0
                : (monthlyTotal / monthlyTarget) * 100;

            ViewBag.MonthlyTarget = monthlyTarget;
            ViewBag.MonthlyTotal = monthlyTotal;
            ViewBag.MonthlyPercent = percentage;
            #endregion

            #region YearTarget
            DateTime YearStar = new DateTime(2025, 1, 1);
            DateTime YearEnd = new DateTime(2025, 12, 31);

            decimal yearTarget = 500_000_000;

            var yearTotal = _context.Orders
                .Where(o => o.OrderDate >= YearStar && o.OrderDate <= YearEnd)
                .Join(_context.Products,
                      o => o.ProductId,
                      p => p.ProductId,
                      (o, p) => o.Quantity * p.UnitPrice)
                .Sum();

            decimal percentageYear = yearTarget == 0
                ? 0
                : (yearTotal / yearTarget) * 100;

            ViewBag.YearTarget = yearTarget;
            ViewBag.YearTotal = yearTotal;
            ViewBag.PercentageYear = percentageYear;
            #endregion

            #region ProductTarget
            var today = DateTime.Today;
            decimal productTarget = 50_000;

            var productTotal = _context.Orders
                .Where(o => o.OrderDate >= today)
                .Join(_context.Products,
                      o => o.ProductId,
                      p => p.ProductId,
                      (o, p) => o.Quantity)
                .Sum();

            decimal percentageProduct = productTarget == 0
                ? 0
                : (productTotal / productTarget) * 100;

            ViewBag.ProductTarget = productTarget;
            ViewBag.ProductTotal = productTotal;
            ViewBag.PercentageProduct = percentageProduct;
            #endregion

            //------------------------------------\\

            #region MonthIcon
            if (monthlyTotal > monthlyTarget)
            {
                ViewBag.TrendingIconForMonth = "zmdi zmdi-trending-up float-right";
            }
            else
            {
                ViewBag.TrendingIconForMonth = "zmdi zmdi-trending-down float-right";
            }
            #endregion

            #region YearIcon
            if (yearTotal > yearTarget)
            {
                ViewBag.TrendingIconForYear = "zmdi zmdi-trending-up float-right";
            }
            else
            {
                ViewBag.TrendingIconForYear = "zmdi zmdi-trending-down float-right";
            }
            #endregion

            #region 
            if (productTotal > productTarget)
            {
                ViewBag.TrendingIconForProduct = "zmdi zmdi-trending-up float-right";
            }
            else
            {
                ViewBag.TrendingIconForProduct = "zmdi zmdi-trending-down float-right";
            }
            #endregion

            return View();
        }
    }
}
