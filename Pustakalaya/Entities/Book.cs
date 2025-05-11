using System.ComponentModel.DataAnnotations;

namespace Pustakalaya.Entities
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string ISBN { get; set; }

        public string Description { get; set; }

        [Required]
        public string Author { get; set; }

        public string Genre { get; set; }

        public string Publisher { get; set; }

        public string Language { get; set; }

        public string Format { get; set; } // paperback, hardcover, etc.

        [Required]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public DateTime PublicationDate { get; set; }

        public DateTime? ListedDate { get; set; } // For "New Arrivals"

        public bool IsAvailableInLibrary { get; set; }

        public double AverageRating { get; set; }

        public bool IsOnSale { get; set; }

        public decimal DiscountPercentage { get; set; }

        public DateTime? DiscountStartDate { get; set; }

        public DateTime? DiscountEndDate { get; set; }

        public bool IsBestseller { get; set; }

        public bool IsAwardWinner { get; set; }

        public bool IsComingSoon { get; set; }

        public string ImageUrl { get; set; }

        // Navigation properties
        //public ICollection<Review> Reviews { get; set; }
        //public ICollection<OrderItem> OrderItems { get; set; }
        //public ICollection<BookmarkItem> BookmarkItems { get; set; }
    }
}