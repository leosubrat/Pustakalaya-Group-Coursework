using Pustakalaya.DTOs;

namespace Pustakalaya.Services.Interface
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto> GetBookByIdAsync(int id);
        Task<BookDto> CreateBookAsync(CreateBookDto bookDto);
        Task<BookDto> UpdateBookAsync(int id, UpdateBookDto bookDto);
        Task<bool> DeleteBookAsync(int id);
        Task<BookDto> UpdateBookStockAsync(int id, int quantity);
        Task<BookDto> ApplyDiscountAsync(int id, DiscountDto discountDto);
        Task<BookDto> RemoveDiscountAsync(int id);
        Task<IEnumerable<BookDto>> GetLowStockBooksAsync(int threshold = 10);
        Task<IEnumerable<BookDto>> GetBooksByTypeAsync(bool bestseller = false, bool newRelease = false, bool awardWinner = false);
        Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm);
        Task<(IEnumerable<BookDto> Books, int TotalCount, int TotalPages)> GetPaginatedBooksAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string searchTerm = null,
            string sortBy = "Title",
            bool ascending = true);
    }
}