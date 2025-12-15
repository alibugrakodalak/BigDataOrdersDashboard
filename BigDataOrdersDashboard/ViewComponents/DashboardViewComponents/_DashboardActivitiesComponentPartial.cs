using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BigDataOrdersDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardActivitiesComponentPartial : ViewComponent
    {
        private readonly BigDataOrdersDbContext _context;

        public _DashboardActivitiesComponentPartial(BigDataOrdersDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var values = _context.Products.Include(x=>x.Category)
                                          .Where(y=>y.StockQuantity <= 9)
                                          .OrderBy(z=>z.StockQuantity)
                                          .Take(9)
                                          .ToList();

            return View(values);
        }
    }
}
