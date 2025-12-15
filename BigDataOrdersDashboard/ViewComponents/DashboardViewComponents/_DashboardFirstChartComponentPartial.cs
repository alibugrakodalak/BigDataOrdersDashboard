using BigDataOrdersDashboard.Context;
using BigDataOrdersDashboard.DTOS.ChartDtos;
using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardFirstChartComponentPartial : ViewComponent
    {
        private readonly BigDataOrdersDbContext _context;

        public _DashboardFirstChartComponentPartial(BigDataOrdersDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var totalOrders = _context.Orders.Count();

            var completedCount = _context.Orders.Where(x => x.OrderStatus == "Tamamlandı").Count();
            var preparingCount = _context.Orders.Where(x => x.OrderStatus == "Hazırlanıyor").Count();
            var shippedCount = _context.Orders.Where(x => x.OrderStatus == "Kargoda").Count();
            //var canceledCount = _context.Orders.Where(x => x.OrderStatus == "iptal Edildi").Count();
            var waitingCount = _context.Orders.Where(x => x.OrderStatus == "Beklemede").Count();

            var result = new List<OrderStatusChartDto>
            {
                new OrderStatusChartDto
                {
                    Title = "Tamamlandı",
                    Percentage = totalOrders == 0 ? 0 : (int)Math.Round(completedCount * 100.0 / totalOrders),
                    ChangeText = "+6% Artış ⬆",
                    IsPositive = true,
                    Color = "#2196F3"
                },
                new OrderStatusChartDto
                {
                    Title = "Bekleyen",
                    Percentage = totalOrders == 0 ? 0 : (int)Math.Round(waitingCount * 100.0 / totalOrders),
                    ChangeText = "-5% Azalış ⬇",
                    IsPositive = false,
                    Color = "#FF7043"
                },
                new OrderStatusChartDto
                {
                    Title = "Hazırlanıyor",
                    Percentage = totalOrders == 0 ? 0 : (int)Math.Round(preparingCount * 100.0 / totalOrders),
                    ChangeText = "+2% Artış ⬆",
                    IsPositive = true,
                    Color = "#00BCD4"
                },
                new OrderStatusChartDto
                {
                    Title = "Kargoda",
                    Percentage = totalOrders == 0 ? 0 : (int)Math.Round(shippedCount * 100.0 / totalOrders),
                    ChangeText = "+4% Artış ⬆",
                    IsPositive = true,
                    Color = "#66BB6A"
                },
                //new OrderStatusChartDto
                //{
                //    Title = "İptal Edildi",
                //    Percentage = totalOrders == 0 ? 0 : (int)Math.Round(canceledCount * 100.0 / totalOrders),
                //    ChangeText = "-1% Azalış ⬇",
                //    IsPositive = false,
                //    Color = "#FF0000"
                //}

            };

            return View(result);

        }

    }
}

