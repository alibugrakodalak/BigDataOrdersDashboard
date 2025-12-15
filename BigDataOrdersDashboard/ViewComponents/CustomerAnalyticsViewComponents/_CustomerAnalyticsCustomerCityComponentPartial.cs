using BigDataOrdersDashboard.Context;
using BigDataOrdersDashboard.DTOS.ChartDtos;
using BigDataOrdersDashboard.DTOS.CustomerDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BigDataOrdersDashboard.ViewComponents.CustomerAnalyticsViewComponents
{
    public class _CustomerAnalyticsCustomerCityComponentPartial : ViewComponent
    {
        private readonly BigDataOrdersDbContext _context;

        public _CustomerAnalyticsCustomerCityComponentPartial(BigDataOrdersDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {

            var topCities = _context.Orders
                                    .Include(o => o.Customer)
                                    .Where(o => o.Customer.CustomerCity != null)
                                    .GroupBy(o => o.Customer.CustomerCity)
                                    .Select(g => new
                                    {
                                        CityName = g.Key,
                                        OrderCount = g.Count()
                                    })
                                    .OrderByDescending(x => x.OrderCount)
                                    .Take(7)
                                    .ToList();

            // Chart.js'e göndermek için ViewBag veya ViewModel kullanılabilir
            ViewBag.CityLabels = topCities.Select(x => x.CityName).ToList();
            ViewBag.CityValues = topCities.Select(x => x.OrderCount).ToList();

            return View();
        }
    }
}
