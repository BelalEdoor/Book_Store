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
        public async Task<IEnumerable<Book>> GetBooks(string sTerm = "", int genreId = 0)
        {
            sTerm = sTerm.ToLower();

            // أول واحد سميته booksFromJoin بدل books
            IEnumerable<Book> booksFromJoin = await (from book in _db.Books
                                                     join genre in _db.Genres
                                                     on book.GenreId equals genre.Id
                                                     join stock in _db.Stocks
                                                     on book.Id equals stock.BookId
                                                     into book_stocks
                                                     from bookWithStock in book_stocks.DefaultIfEmpty()
                                                     where string.IsNullOrWhiteSpace(sTerm)
                                                           || (book != null && book.BookName.ToLower().StartsWith(sTerm))
                                                     select new Book
                                                     {
                                                         Id = book.Id,
                                                         Image = book.Image,
                                                         AuthorName = book.AuthorName,
                                                         BookName = book.BookName,
                                                         GenreId = book.GenreId,
                                                         Price = book.Price,
                                                         GenreName = genre.GenreName,
                                                         Quantity = bookWithStock == null ? 0 : bookWithStock.Quantity
                                                     }
                                 ).ToListAsync();


            if (genreId > 0)
            {
                booksFromJoin = booksFromJoin.Where(a => a.GenreId == genreId).ToList();
            }


            var bookQuery = _db.Books
               .AsNoTracking()
               .Include(x => x.Genre)
               .Include(x => x.Stock)
               .AsQueryable();

            if (!string.IsNullOrWhiteSpace(sTerm))
            {
                bookQuery = bookQuery.Where(b => b.BookName.StartsWith(sTerm.ToLower()));
            }

            if (genreId > 0)
            {
                bookQuery = bookQuery.Where(b => b.GenreId == genreId);
            }

            // الثاني زي ما هو books
            List<Book> books = await bookQuery
                .AsNoTracking()
                .Select(book => new Book
                {
                    Id = book.Id,
                    Image = book.Image,
                    AuthorName = book.AuthorName,
                    BookName = book.BookName,
                    GenreId = book.GenreId,
                    Price = book.Price,
                    GenreName = book.Genre.GenreName,
                    Quantity = book.Stock == null ? 0 : book.Stock.Quantity
                }).ToListAsync();

            return books;
        }
    }
}
