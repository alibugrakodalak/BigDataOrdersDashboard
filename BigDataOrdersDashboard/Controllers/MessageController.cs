using BigDataOrdersDashboard.Context;
using BigDataOrdersDashboard.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BigDataOrdersDashboard.Controllers
{
    public class MessageController : Controller
    {
        private readonly BigDataOrdersDbContext _context;

        public MessageController(BigDataOrdersDbContext context)
        {
            _context = context;
        }
        public IActionResult MessageList(int page = 1)
        {
            int pageSize = 18; // her seferinde 18 kayıt
            var values = _context.Messages
                                 .OrderBy(p => p.MessageId)
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .Include(y => y.Customer)
                                 .ToList();

            int totalCount = _context.Messages.Count();
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View(values);
        }

        public IActionResult GetMessageDetail(int id)
        {
            var message = _context.Messages
                .Include(x => x.Customer)
                .FirstOrDefault(x => x.MessageId == id);

            if (message == null)
                return Json(new { success = false });

            return Json(new
            {
                success = true,
                messageId = message.MessageId,
                sender = $"{message.Customer.CustomerName} {message.Customer.CustomerSurname}",
                subject = message.MessageSubject,
                content = message.MessageText,
                date = message.CreatedDate.ToString("dd MMMM yyyy HH:mm"),
                sentiment = message.SentimentLabel
            });
        }


        [HttpGet] 
        public IActionResult CreateMessage()
        {
            return View();
        }

        [HttpPost] 
        public async Task<IActionResult> CreateMessage(Message message)
        {
            // Hugging Face Yetkikendirmesi
            var client = new HttpClient();
            var apiKey = "YOUR_HUGGINGKEY";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            client.DefaultRequestHeaders.Add("User-Agent", "BugraApp/1.0");
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            try
            {
                // İngilizce Türkçe dil modeoli
                var translateRequestBody = new
                {
                    inputs = message.MessageText
                };

                var translateJson = JsonSerializer.Serialize(translateRequestBody);
                var translateContent = new StringContent(translateJson, Encoding.UTF8, "application/json");
                var translateResponse = await client.PostAsync("https://router.huggingface.co/hf-inference/models/Helsinki-NLP/opus-mt-tr-en", translateContent);
                var translateResponseString = await translateResponse.Content.ReadAsStringAsync();

                string englishText = message.MessageText;

                if (translateResponseString.TrimStart().StartsWith("["))
                {
                    var translateDoc = JsonDocument.Parse(translateResponseString);
                    englishText = translateDoc.RootElement[0].GetProperty("translation_text").GetString();
                }

                // Toksiklik Kontrolü 
                var toxicRequestBody = new
                {
                    inputs = englishText
                };

                var toxicJson = JsonSerializer.Serialize(toxicRequestBody);
                var toxicContent = new StringContent(toxicJson, Encoding.UTF8, "application/json");
                var toxicResponse = await client.PostAsync("https://router.huggingface.co/hf-inference/models/unitary/toxic-bert", toxicContent);
                var toxicResponseString = await toxicResponse.Content.ReadAsStringAsync();

                if (toxicResponseString.TrimStart().StartsWith("["))
                {
                    var toxicDoc = JsonDocument.Parse(toxicResponseString);
                    foreach(var item in toxicDoc.RootElement[0].EnumerateArray())
                    {
                        string label = item.GetProperty("label").GetString();
                        double score = item.GetProperty("score").GetDouble();

                        if(score > 0.5)
                        {
                            message.SentimentLabel = "Toksik İçerik";
                            break;
                        }
                        else
                        {
                            message.SentimentLabel = "Uygun İçerik";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message.SentimentLabel = "Hata Oluştu!!" + ex.ToString();
            }

            message.CreatedDate = DateTime.Now;
            _context.Messages.Add(message);
            _context.SaveChanges(); 
            return RedirectToAction("MessageList"); 
        }

        public IActionResult DeleteMessage(int id)
        {
            var value = _context.Messages.Find(id);
            _context.Messages.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("MessageList");
        }

        [HttpGet]
        public IActionResult UpdateMessage(int id)
        {
            var value = _context.Messages.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateMessage(Message message)
        {
            _context.Messages.Update(message);
            _context.SaveChanges();
            return RedirectToAction("MessageList");
        }
    }
}
