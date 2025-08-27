using System.Diagnostics;
using System.Threading.Tasks;
using BookShopping_Ecommerce.Models;
using BookShopping_Ecommerce.Models.DTOs;
using BookShopping_Ecommerce.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BookShopping_Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;

        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
        {
            _logger = logger;
            _homeRepository = homeRepository;   
        }

        public async Task<IActionResult> Index(string sTerm="", int genreId=0)
        {
            IEnumerable<Book> books = await _homeRepository.GetBooks(sTerm, genreId); 
            IEnumerable<Genre> genres = await _homeRepository.Genres();
            BookDisplayModel bookModel = new BookDisplayModel
            {
                Books = books,
                Genres = genres,
                STerm = sTerm,
                GenreId = genreId
            };

            return View(bookModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
