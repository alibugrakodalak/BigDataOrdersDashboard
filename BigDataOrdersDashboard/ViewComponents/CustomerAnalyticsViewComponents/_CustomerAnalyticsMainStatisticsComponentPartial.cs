using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.ViewComponents.CustomerAnalyticsViewComponents
{
    public class _CustomerAnalyticsMainStatisticsComponentPartial : ViewComponent
    {
        private readonly BigDataOrdersDbContext _context;

        public _CustomerAnalyticsMainStatisticsComponentPartial(BigDataOrdersDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var totalCustomer = _context.Customers.Count();
            ViewBag.TotalCustomer = totalCustomer;

            var totalOrderCount = _context.Orders.Count();
            var avarageOrderPerCustomerCount = totalOrderCount/totalCustomer;
            ViewBag.AvarageOrderPerCustomerCount = avarageOrderPerCustomerCount;

            var threeMonthAgo = DateTime.Now.AddMonths(-3);
            var activeCustomerCount = _context.Orders
                                              .Where(o => o.OrderDate >= threeMonthAgo)
                                              .Select(o=>o.CustomerId)
                                              .Distinct()
                                              .Count();
            ViewBag.ActiveCustomerCount = activeCustomerCount;

            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var inActiveCustomerCount = _context.Customers
                                                .Count(c => !_context.Orders
                                                .Any(o => o.CustomerId == c.CustomerId 
                                                && o.OrderDate >= sixMonthsAgo));
            ViewBag.InActiveCustomerCount = inActiveCustomerCount;
                                                

            return View();
        }
    }
}
