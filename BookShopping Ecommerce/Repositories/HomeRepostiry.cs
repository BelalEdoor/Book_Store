using BookShopping_Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BookShopping_Ecommerce.Repositories
{
    public class HomeRepostiry:IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepostiry(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _db.Genres.ToListAsync();    
        }
        public async Task<IEnumerable<Book>> GetBooks(string sTerm, int genreId = 0)
        {
            sTerm = sTerm?.ToLower() ?? string.Empty;

                IEnumerable <Book> books = await (
                from book in _db.Books
                join genre in _db.Genres
                on book.GenreId equals genre.Id
                where string.IsNullOrWhiteSpace(sTerm)
                   || (book.BookName != null && book.BookName.ToLower().StartsWith(sTerm))
                select new Book
                {
                    Id = book.Id,
                    BookName = book.BookName,
                    AuthorName = book.AuthorName,
                    Price = book.Price,
                    Image = book.Image,
                    GenreId = book.GenreId,
                    GenreName = genre.GenreName
                }
            ).ToListAsync();
             if(genreId > 0)
            {
                books = books.Where(a => a.GenreId == genreId).ToList();
            }
            return books;
        }
    }
}
