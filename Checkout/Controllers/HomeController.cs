using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Checkout.Models;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Checkout.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string id = null)
        {
            Products product = null;
            try
            {
                string responseBody = string.Empty;
                StringBuilder url = new StringBuilder();
                url.Append(Environment.GetEnvironmentVariable("urlApi"));
                url.Append("/"+id);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url.ToString());
                // var tlsHandshakeFeature = request.HttpWebRequest.Features.Get<ITlsHandshakeFeature>();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        responseBody = new StreamReader(response.GetResponseStream()).ReadToEnd();                    
                        product = JsonConvert.DeserializeObject<Products>(responseBody);
                    }
            }
            catch(Exception ex)
            {
                //return RedirectToAction(nameof(Error), new { message = "Retorno vazio ou não serealizavel." });
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }    

            CheckoutDto productt = new CheckoutDto{Product = product, Customer = null};

            return View(productt);
        }

        public IActionResult Checkout(CheckoutDto Dto)
        {
            var order = new Order{Name = Dto.Customer.Name, Email = Dto.Customer.Email, Phone = Dto.Customer.Phone, ProductId = Dto.Product.id};
            
            // RabbitMQ
            // https://www.rabbitmq.com/dotnet-api-guide.html
            
            // Connection
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(Environment.GetEnvironmentVariable("conRabbitMQ"));

            // All process
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {           
                        // Exchange
                        channel.ExchangeDeclare(exchange:"checkout_ex", type:ExchangeType.Direct);

                        // Queue
                        channel.QueueDeclare(queue: "checkout_queue",
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);

                        // Binding Ex with Queue
                        channel.QueueBind("checkout_queue", "checkout_ex", "checkout");
                        
                        // Serealize Order
                        string message = JsonConvert.SerializeObject(order);
                        var body = Encoding.UTF8.GetBytes(message);

                        // Publish
                        channel.BasicPublish(exchange: "checkout_ex",
                                            routingKey: "checkout",
                                            basicProperties: null,
                                            body: body);
            }

            return Ok("Processado com Sucesso");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                // macete do pramework pra pegar o Id interno da requisição
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(viewModel);
        }
    }
}
