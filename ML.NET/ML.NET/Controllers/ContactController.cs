using Microsoft.AspNetCore.Mvc;
using ML.NET.Models;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Threading.Tasks;
using ML.NET.Models;

namespace ML.NET.Controllers
{
    public class ContactController : Controller
    {
        private readonly ILogger<ContactController> _logger;
        // 若要寄信，可注入 EmailSettings 或 IEmailSender

        public ContactController(ILogger<ContactController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ContactFormModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactFormModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 範例：寫入日誌
            _logger.LogInformation("Contact form submitted: {Name}, {Email}, {Phone}",
                                   model.Name, model.Email, model.Phone);

            // 範例：簡易寄信（請先在 appsettings.json 設定 SMTP 參數並注入）
            /*
            var msg = new MailMessage();
            msg.To.Add("your@company.com");
            msg.Subject = $"[網站聯絡] {model.Name}";
            msg.Body = $"Email: {model.Email}\nPhone: {model.Phone}\n\n{model.Message}";
            using var smtp = new SmtpClient("smtp.yourserver.com")
            {
                Credentials = new System.Net.NetworkCredential("user","pwd"),
                EnableSsl = true,
                Port = 587
            };
            await smtp.SendMailAsync(msg);
            */

            ViewBag.Success = true;
            return View(new ContactFormModel()); // 清空表單
        }

        // 提供 PDF 下載
        [HttpGet]
        public IActionResult Resume()
        {
            // 從 wwwroot/protected 讀檔
            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "protected", "resume.pdf"
            );
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes,
                        "application/pdf",
                        "林泓志_Resume.pdf");
        }
    }
}
