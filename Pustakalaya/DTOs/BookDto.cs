namespace Pustakalaya.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }

        // Basic Info
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Publisher { get; set; }
        public string Language { get; set; }
        public string Format { get; set; }

        // Pricing and Inventory
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        // Dates
        public DateTime PublicationDate { get; set; }
        public DateTime? ListedDate { get; set; }

        // Availability
        public bool IsAvailableInLibrary { get; set; }

        // Ratings & Tags
        public double AverageRating { get; set; }
        public bool IsBestseller { get; set; }
        public bool IsAwardWinner { get; set; }
        public bool IsComingSoon { get; set; }

        // Discount Info
        public bool IsOnSale { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }

        // Visual
        public string ImageUrl { get; set; }
    }

    public class CreateBookDto
    {
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Publisher { get; set; }
        public string Language { get; set; }
        public string Format { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime PublicationDate { get; set; }
        public bool IsAvailableInLibrary { get; set; }
        public bool IsBestseller { get; set; }
        public bool IsAwardWinner { get; set; }
        public bool IsComingSoon { get; set; }
        public string ImageUrl { get; set; }
    }

    public class UpdateBookDto
    {
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Publisher { get; set; }
        public string Language { get; set; }
        public string Format { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime PublicationDate { get; set; }
        public bool IsAvailableInLibrary { get; set; }
        public bool IsBestseller { get; set; }
        public bool IsAwardWinner { get; set; }
        public bool IsComingSoon { get; set; }
        public string ImageUrl { get; set; }
    }

    public class DiscountDto
    {
        public bool IsOnSale { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
