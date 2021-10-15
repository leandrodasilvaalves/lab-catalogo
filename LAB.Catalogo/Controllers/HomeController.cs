using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LAB.Catalogo.Models;
using LAB.Catalogo.Services;

namespace LAB.Catalogo.Controllers
{
    //OBSERVACAO
    //Diferentes tipos de logs foram aplicados apenas com finalidade de testes e estudos
    public class HomeController : Controller
    {
        private readonly ILoggerService _logger;
        private readonly CatalogoContext _db;
        private readonly ICacheService _cacheService;

        public HomeController(
            ILoggerService logger, 
            CatalogoContext db, 
            ICacheService cacheService
        )
        {
            _logger = logger;
            _db = db;
            _cacheService = cacheService;
        }

        public async Task<IActionResult> Index()
        {
            string cachekey= "all_products";
            var products = await _cacheService.GetOrCreateAsync<IEnumerable<Produto>>(cachekey, ()=>{
                var data = _db.Produtos.ToList();
                return data;
            });

            _logger.Info($"Numero de produtos obtidos: {products?.Count()}"); 
            return View(products);
        }

        public IActionResult Privacy()
        {
            _logger.Critical("Você está vendo a página: Privacy");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.Error("Você está vendo a página: Error");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
