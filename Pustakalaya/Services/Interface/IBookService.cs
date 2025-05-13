using Pustakalaya.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pustakalaya.Services.Interface
{
    public interface IBookService
    {
        // Used in both Admin and User dashboards
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto> GetBookByIdAsync(int id);

        // Admin CRUD
        Task<BookDto> CreateBookAsync(CreateBookDto bookDto);
        Task<BookDto> UpdateBookAsync(int id, UpdateBookDto bookDto);
        Task<bool> DeleteBookAsync(int id);

        // Stock update (Admin)
        Task<BookDto> UpdateBookStockAsync(int id, int quantity);

        // Discount management (Admin)
        Task<BookDto> ApplyDiscountAsync(int id, DiscountDto discountDto);
        Task<BookDto> RemoveDiscountAsync(int id);

        // For filtering or dashboard use
        Task<IEnumerable<BookDto>> GetLowStockBooksAsync(int threshold = 10);
        Task<IEnumerable<BookDto>> GetBooksByTypeAsync(bool bestseller = false, bool newRelease = false, bool awardWinner = false);

        // Search
        Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm);

        // Paginated book list (optional use for frontend table)
        Task<(IEnumerable<BookDto> Books, int TotalCount, int TotalPages)> GetPaginatedBooksAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string searchTerm = null,
            string sortBy = "Title",
            bool ascending = true);

        Task<IEnumerable<BookDto>> GetDiscountedBooksAsync();
    }
}
