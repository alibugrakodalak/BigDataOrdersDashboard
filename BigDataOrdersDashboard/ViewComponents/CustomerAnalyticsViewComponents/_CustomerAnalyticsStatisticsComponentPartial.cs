using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.ViewComponents.CustomerAnalyticsViewComponents
{
    public class _CustomerAnalyticsStatisticsComponentPartial : ViewComponent
    {
        private readonly BigDataOrdersDbContext _context;

        public _CustomerAnalyticsStatisticsComponentPartial(BigDataOrdersDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {

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

            ViewBag.FirstAddedCustomer = _context.Customers
                                                 .OrderBy(x => x.CustomerId)
                                                 .Take(1)
                                                 .Select(y => y.CustomerName + " " + y.CustomerSurname)
                                                 .FirstOrDefault();

            ViewBag.LastAddedCustomer = _context.Customers
                                                .OrderByDescending(x => x.CustomerId)
                                                .Take(1)
                                                .Select(y => y.CustomerName + " " + y.CustomerSurname)
                                                .FirstOrDefault();

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
