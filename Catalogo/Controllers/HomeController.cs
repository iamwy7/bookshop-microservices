using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Catalogo.Models;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace Catalogo.Controllers
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
            List<Products> products;
            try
            {
                string responseBody = string.Empty;
                StringBuilder url = new StringBuilder();
                url.Append(Environment.GetEnvironmentVariable("urlApi"));
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url.ToString());
                // var tlsHandshakeFeature = request.HttpWebRequest.Features.Get<ITlsHandshakeFeature>();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        responseBody = new StreamReader(response.GetResponseStream()).ReadToEnd();                    
                        products = JsonConvert.DeserializeObject<List<Products>>(responseBody);
                    }
            }
            catch(Exception ex)
            {
                //return RedirectToAction(nameof(Error), new { message = "Retorno vazio ou não serealizavel." });
                return RedirectToAction(nameof(Error), new { message = ex.Message });
            }    
            //return Json(product);
            return View(products);
        }
        public IActionResult Details(string id = null)
        {
            Products product;
            try
            {
                string responseBody = string.Empty;
                StringBuilder url = new StringBuilder();
                url.Append(Environment.GetEnvironmentVariable("urlApi"));
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
            //return Json(product);
            return View(product);
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
