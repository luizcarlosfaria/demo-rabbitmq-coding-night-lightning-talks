using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PublisherApplication.Models;
using RabbitMQ.Client;

namespace PublisherApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult Teste(string nome, int num)
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = "message_broker",
                Port = 5672,
                UserName = "user",
                Password = "password",
            };

            using var connection = connectionFactory.CreateConnection();

            using var model = connection.CreateModel();

            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes($"{{ nome:\"{nome}\", num:\"{num}\" }}");

            var prop = model.CreateBasicProperties();

            model.BasicPublish("", "coding-night", prop, messageBodyBytes);

            return View(new
            {
                nome,
                num
            });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
