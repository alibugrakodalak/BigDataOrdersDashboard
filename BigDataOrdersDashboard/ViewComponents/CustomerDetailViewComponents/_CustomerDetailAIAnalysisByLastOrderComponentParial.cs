using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BigDataOrdersDashboard.ViewComponents.CustomerDetailViewComponents
{
    public class _CustomerDetailAIAnalysisByLastOrderComponentParial : ViewComponent
    {
        private readonly BigDataOrdersDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public _CustomerDetailAIAnalysisByLastOrderComponentParial(
                BigDataOrdersDbContext context,
                IHttpClientFactory httpClientFactory,
                IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            id = 1001;

            //Müşteri Listesi
            var customer = _context.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.Product)
                .ThenInclude(p => p.Category)
                .Where(c => c.CustomerId == id)
                .Select(c => new
                {
                    c.CustomerName,
                    c.CustomerSurname,
                    Orders = c.Orders
                    .OrderByDescending(o => o.OrderDate)
                    .Take(20)
                    .Select(o => new
                    {
                        o.OrderDate,
                        Product = o.Product.ProductName,
                        Category = o.Product.Category.CategoryName,
                        o.Quantity,
                        o.Product.UnitPrice,
                        TotalPrice = o.Quantity * o.Product.UnitPrice
                    })
                }).FirstOrDefault();

            var jsonData = JsonSerializer.Serialize(customer);

            //Prompt Yazımı
            string prompt = $@"
                            ⚠️ Çok önemli:
                            Kesinlikle ``` (backtick) veya kod bloğu verme.
                            Sadece saf HTML üret. Markdown verme. Kod bloğu verme.

                            Sen bir veri analisti ve müşteri davranış uzmanısın.
                            Aşağıdaki veriyi analiz et ve sonucu HTML formatında ver.

                            Bu başlıkları kullan (sırasını ve isimleri değiştirme):

                            <h3>👤 Müşteri Profili</h3>
                            <p><b>Ad:</b> ...</p>
                            <p><b>Soyad:</b> ...</p>
                            <p><b>Toplam Sipariş:</b> ...</p>
                            <p><b>Toplam Harcama:</b> ...</p>

                            <h3>🛍️ Ürün Tercihleri</h3>
                            <ul>
                              <li>🏠 Ev & Dekorasyon – X sipariş</li>
                              <li>💄 Kozmetik – X sipariş</li>
                            </ul>
                            <p><b>Öne çıkan ürünler:</b></p>
                            <ul>
                              <li>Ürün adı (adet — fiyat)</li>
                            </ul>

                            <h3>⏰ Zaman Bazlı Alışveriş Davranışı</h3>
                            <p>En yoğun ay: ...</p>
                            <p>En yoğun gün: ...</p>
                            <p>Favori saat aralığı: ...</p>

                            <h3>💰 Ortalama Harcama ve Sıklık</h3>
                            <p>Aylık ortalama sipariş: ...</p>
                            <p>Ortalama sepet tutarı: ...</p>
                            <p>En yüksek sipariş: ...</p>
                            <p>En düşük sipariş: ...</p>

                            <h3>🎯 Sadakat ve Tekrar Harcama Eğilimi</h3>
                            <p>Tekrar alışveriş eğilimi: ...</p>
                            <p>Marka sadakati: ...</p>
                            <p>Kategori sadakati: ...</p>

                            <h3>🚀 Pazarlama Önerileri</h3>
                            <ul>
                              <li>🎁 Kampanya önerisi: ...</li>
                              <li>✉️ Hedefli e-posta: ...</li>
                              <li>🆕 Yeni ürün tanıtımı önerisi: ...</li>
                            </ul>

                            Veri:{jsonData}";

            //OpenAI Api İsteği
            var httpClient = _httpClientFactory.CreateClient();
            var apiKey = _configuration["OpenAI:ApiKey"];

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new {role="system",content="You are a senior marketing data analyst."},
                    new {role="user",content=prompt},
                },
                temperature = 0.5
            };

            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            //İsteği Gönderme
            var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseString = await response.Content.ReadAsStringAsync();

            //Json Cevabı Ayrıştır
            var doc = JsonDocument.Parse(responseString);
            var completion = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            string[] sections = completion.Split("<h3>");

            ViewBag.AnalysisSection1 = "<h3>" + sections.ElementAtOrDefault(1);
            ViewBag.AnalysisSection2 = "<h3>" + sections.ElementAtOrDefault(2);
            ViewBag.AnalysisSection3 = "<h3>" + sections.ElementAtOrDefault(3);
            ViewBag.AnalysisSection4 = "<h3>" + sections.ElementAtOrDefault(4);
            ViewBag.AnalysisSection5 = "<h3>" + sections.ElementAtOrDefault(5);
            ViewBag.AnalysisSection6 = "<h3>" + sections.ElementAtOrDefault(6);

            //ViewBag.Customer = $"{customer.CustomerName} {customer.CustomerSurname}";
            //ViewBag.Analysis = completion;

            return View();
        }
    }
}