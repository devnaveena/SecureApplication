using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.RequestDto
{
    public class BookDto
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author is required")]
        public string Author { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Publish Date is required")]
        public DateTime PublishDate { get; set; }

        [Required(ErrorMessage = "ISBN Number is required")]
        public string IsbnNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Publisher is required")]
        public string Publisher { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Language is required")]
        public string Language { get; set; } = string.Empty;

        [Required(ErrorMessage = "Genre is required")]
        public string Genre { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Page Count must be greater than 0")]
        public int PageCount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative value")]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}