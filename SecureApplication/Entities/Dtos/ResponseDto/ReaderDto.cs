namespace Entities.Dtos.ResponseDto
{
    public class ReaderDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string IsbnNumber { get; set; } = string.Empty;

    }
}