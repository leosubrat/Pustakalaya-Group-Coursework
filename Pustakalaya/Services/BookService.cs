using Microsoft.EntityFrameworkCore;
using Pustakalaya.Data;
using Pustakalaya.DTOs;
using Pustakalaya.Entities;
using Pustakalaya.Services.Interface;

namespace Pustakalaya.Services
{
    public class BookService : IBookService
    {
        private readonly AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await _context.Books.ToListAsync();
            return books.Select(MapToDto);
        }

        public async Task<BookDto> GetBookByIdAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return null;
            }

            return MapToDto(book);
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto bookDto)
        {
            var book = new Book
            {
                Title = bookDto.Title,
                ISBN = bookDto.ISBN,
                Description = bookDto.Description,
                Author = bookDto.Author,
                Genre = bookDto.Genre,
                Publisher = bookDto.Publisher,
                Language = bookDto.Language,
                Format = bookDto.Format,
                Price = bookDto.Price,
                StockQuantity = bookDto.StockQuantity,
                PublicationDate = DateTime.SpecifyKind(bookDto.PublicationDate, DateTimeKind.Utc),
                ListedDate = DateTime.UtcNow,
                IsAvailableInLibrary = bookDto.IsAvailableInLibrary,
                AverageRating = 0, // Default rating for new book
                IsOnSale = false,
                DiscountPercentage = 0,
                IsBestseller = bookDto.IsBestseller,
                IsAwardWinner = bookDto.IsAwardWinner,
                IsComingSoon = bookDto.IsComingSoon,
                ImageUrl = bookDto.ImageUrl
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return MapToDto(book);
        }

        public async Task<BookDto> UpdateBookAsync(int id, UpdateBookDto bookDto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return null;
            }

            book.Title = bookDto.Title;
            book.ISBN = bookDto.ISBN;
            book.Description = bookDto.Description;
            book.Author = bookDto.Author;
            book.Genre = bookDto.Genre;
            book.Publisher = bookDto.Publisher;
            book.Language = bookDto.Language;
            book.Format = bookDto.Format;
            book.Price = bookDto.Price;
            book.StockQuantity = bookDto.StockQuantity;
            book.PublicationDate = DateTime.SpecifyKind(bookDto.PublicationDate, DateTimeKind.Utc);
            book.IsAvailableInLibrary = bookDto.IsAvailableInLibrary;
            book.IsBestseller = bookDto.IsBestseller;
            book.IsAwardWinner = bookDto.IsAwardWinner;
            book.IsComingSoon = bookDto.IsComingSoon;
            book.ImageUrl = bookDto.ImageUrl;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return MapToDto(book);
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return false;
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<BookDto> UpdateBookStockAsync(int id, int quantity)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return null;
            }

            book.StockQuantity = quantity;
            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return MapToDto(book);
        }

        public async Task<BookDto> ApplyDiscountAsync(int id, DiscountDto discountDto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return null;
            }

            book.IsOnSale = discountDto.IsOnSale;
            book.DiscountPercentage = discountDto.DiscountPercentage;
            // Convert date to UTC
            book.DiscountStartDate = discountDto.StartDate.HasValue
                ? DateTime.SpecifyKind(discountDto.StartDate.Value, DateTimeKind.Utc)
                : null;
            book.DiscountEndDate = discountDto.EndDate.HasValue
                ? DateTime.SpecifyKind(discountDto.EndDate.Value, DateTimeKind.Utc)
                : null;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return MapToDto(book);
        }

        public async Task<BookDto> RemoveDiscountAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return null;
            }

            book.IsOnSale = false;
            book.DiscountPercentage = 0;
            book.DiscountStartDate = null;
            book.DiscountEndDate = null;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return MapToDto(book);
        }
        public async Task<IEnumerable<BookDto>> GetLowStockBooksAsync(int threshold = 10)
        {
            var books = await _context.Books
                .Where(b => b.StockQuantity < threshold)
                .ToListAsync();
            return books.Select(MapToDto);
        }

        public async Task<IEnumerable<BookDto>> GetBooksByTypeAsync(bool bestseller = false, bool newRelease = false, bool awardWinner = false)
        {
            var query = _context.Books.AsQueryable();

            if (bestseller)
                query = query.Where(b => b.IsBestseller);
            if (newRelease)
                query = query.Where(b => b.ListedDate != null && b.ListedDate > DateTime.UtcNow.AddMonths(-3));
            if (awardWinner)
                query = query.Where(b => b.IsAwardWinner);

            var books = await query.ToListAsync();
            return books.Select(MapToDto);
        }
        public async Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllBooksAsync();

            searchTerm = searchTerm.ToLower();

            var books = await _context.Books
                .Where(b => b.Title.ToLower().Contains(searchTerm) ||
                           b.Author.ToLower().Contains(searchTerm) ||
                           b.Genre.ToLower().Contains(searchTerm) ||
                           b.Description.ToLower().Contains(searchTerm) ||
                           b.ISBN.ToLower().Contains(searchTerm) ||
                           b.Publisher.ToLower().Contains(searchTerm))
                .ToListAsync();

            return books.Select(MapToDto);
        }

        public async Task<(IEnumerable<BookDto> Books, int TotalCount, int TotalPages)> GetPaginatedBooksAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string searchTerm = null,
            string sortBy = "Title",
            bool ascending = true)
        {
            IQueryable<Book> query = _context.Books;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(b =>
                    b.Title.ToLower().Contains(searchTerm) ||
                    b.Author.ToLower().Contains(searchTerm) ||
                    b.Genre.ToLower().Contains(searchTerm) ||
                    b.Description.ToLower().Contains(searchTerm) ||
                    b.ISBN.ToLower().Contains(searchTerm) ||
                    b.Publisher.ToLower().Contains(searchTerm));
            }


            var totalCount = await query.CountAsync();

      
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

    
            query = ApplySorting(query, sortBy, ascending);

     
            var books = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (books.Select(MapToDto), totalCount, totalPages);
        }

        private IQueryable<Book> ApplySorting(IQueryable<Book> query, string sortBy, bool ascending)
        {
            switch (sortBy.ToLower())
            {
                case "title":
                    return ascending ? query.OrderBy(b => b.Title) : query.OrderByDescending(b => b.Title);
                case "author":
                    return ascending ? query.OrderBy(b => b.Author) : query.OrderByDescending(b => b.Author);
                case "price":
                    return ascending ? query.OrderBy(b => b.Price) : query.OrderByDescending(b => b.Price);
                case "publicationdate":
                    return ascending ? query.OrderBy(b => b.PublicationDate) : query.OrderByDescending(b => b.PublicationDate);
                case "stock":
                    return ascending ? query.OrderBy(b => b.StockQuantity) : query.OrderByDescending(b => b.StockQuantity);
                case "rating":
                    return ascending ? query.OrderBy(b => b.AverageRating) : query.OrderByDescending(b => b.AverageRating);
                default:
                    return ascending ? query.OrderBy(b => b.Title) : query.OrderByDescending(b => b.Title);
            }
        }
        private BookDto MapToDto(Book book)
        {
            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                Description = book.Description,
                Author = book.Author,
                Genre = book.Genre,
                Publisher = book.Publisher,
                Language = book.Language,
                Format = book.Format,
                Price = book.Price,
                StockQuantity = book.StockQuantity,
                PublicationDate = book.PublicationDate,
                ListedDate = book.ListedDate,
                IsAvailableInLibrary = book.IsAvailableInLibrary,
                AverageRating = book.AverageRating,
                IsOnSale = book.IsOnSale,
                DiscountPercentage = book.DiscountPercentage,
                DiscountStartDate = book.DiscountStartDate,
                DiscountEndDate = book.DiscountEndDate,
                IsBestseller = book.IsBestseller,
                IsAwardWinner = book.IsAwardWinner,
                IsComingSoon = book.IsComingSoon,
                ImageUrl = book.ImageUrl
            };
        }
    }
}