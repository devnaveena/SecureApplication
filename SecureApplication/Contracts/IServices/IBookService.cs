using Entities.Dtos.RequestDto;

namespace Contracts.IServices
{
    public interface IBookService
    {
        /// <summary>
        /// This function creates a new book in the database, checking if a book with the same ISBN
        /// number already exists and throwing a conflict exception if it does.
        /// </summary>
        /// <param name="BookDto">contains information about a book,such as its title, author, and ISBN number.</param>
        public void CreateBook(BookDto bookDto);
        /// <summary>
        /// This function retrieves a list of books based on page number, page size, and an optional
        /// filter value, and then maps and decrypts the ISBN number for each book in the list.
        /// </summary>
        /// <param name="pageNumber">The page number of the results to retrieve.</param>
        /// <param name="pageSize">The number of items to be returned per page.</param>
        /// <param name="filterValue">A string value used to filter the books based on genre or author.If null or empty, all books will be returned.</param>
        /// <returns>
        /// A list of books
        /// </returns>
        public List<T> GetAllBooks<T>(int pageNumber, int pageSize, string? filterValue = null);

        /// <summary>
        /// This function retrieves a book by its ID, maps it to a specified type, decrypts and masks its ISBN number, and returns the mapped book.
        /// </summary>
        /// <param name="bookId">.</param>
        /// <returns>
        /// The method is returning a mapped and decrypted book object of type T(generic)
        /// </returns>
        public T GetBookById<T>(Guid bookId);

        /// <summary>
        /// This function deletes a book from a repository by its ID.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteBook(Guid id);
    }
}