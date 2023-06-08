using AutoMapper;
using Contracts.IRepositories;
using Contracts.IServices;
using Entities.Dtos.RequestDto;
using Entities.Dtos.ResponseDto;
using Entities.Models;
using System.Security.Cryptography;
using Exceptions;
using Microsoft.AspNetCore.Mvc;
using SecureApplication.Controllers;
using System.Text;
using System.Reflection;

namespace Services
{
    public class BookService : IBookService
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IConfiguration _configuration;
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<BookService> _logger;
        public BookService(IMapper mapper,
                           IRepositoryWrapper repositoryWrapper,
                           IConfiguration configuration,
                           ILogger<BookService> logger,
                           IBookRepository bookRepository)
        {
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _repositoryWrapper = repositoryWrapper;
            _bookRepository = bookRepository;
        }

        public void CreateBook(BookDto bookDto)
        {
            // Check if a book with the same ISBN number already exists in the database
            Book? existingBook = _repositoryWrapper.Book.FindByCondition(b => b.IsbnNumber == Encrypt(bookDto.IsbnNumber), includeInactive: true).FirstOrDefault();

            if (existingBook != null)
            {
                _logger.LogError("A book with the same ISBN number already exists. ISBN: {IsbnNumber}", bookDto.IsbnNumber);
                // A book with the same ISBN number already exists, throw a conflict exception
                throw new ConflictException("A book with the same ISBN number already exists.");
            }

            Book book = _mapper.Map<Book>(bookDto);
            book.IsbnNumber = Encrypt(bookDto.IsbnNumber);
            _repositoryWrapper.Book.Create(book);//Creates and save the changes to database
            _repositoryWrapper.Save();
            _logger.LogInformation("Book created successfully with ISBN: {IsbnNumber}", bookDto.IsbnNumber);

        }
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
        public List<T> GetAllBooks<T>(int pageNumber, int pageSize, string? filterValue = null)
        {
            IQueryable<Book> query = _repositoryWrapper.Book.FindAll(includeInactive: true);

            if (!string.IsNullOrEmpty(filterValue))
            {
                _logger.LogInformation("Filtering books by genre or author with value: {FilterValue}", filterValue);
                query = query.Where(book => book.Genre == filterValue || book.Author == filterValue);
            }

            List<Book> bookList = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (bookList.Count == 0)
            {
                _logger.LogError("No books found in the collection with filter value: {FilterValue}", filterValue);
                throw new Exceptions.NoContentException("No books exists. Please add some books to the collection");
            }

            List<T> books = _mapper.Map<List<T>>(bookList);
            // Decrypt the IsbnNumber property for each book in the list
            foreach (T book in books)
            {
                DecryptAndMaskIsbnNumber(book);
            }
            _logger.LogInformation("Retrieved {Count} books from the collection", bookList.Count);

            return books;
        }

        public T GetBookById<T>(Guid bookId)
        {
            Book? book = _repositoryWrapper.Book.FindByCondition(b => b.Id == bookId, includeInactive: true).FirstOrDefault();
            if (book == null)
            {
                _logger.LogError("No book found with ID: {BookId}", bookId);
                throw new NotFoundException($"Book with ID {bookId} not found");
            }

            T mappedBook = _mapper.Map<T>(book);
            DecryptAndMaskIsbnNumber(mappedBook); // Decrypt and Mask the IsbnNumber property of the  book 
            _logger.LogInformation("Retrieved book with ID: {BookId}", bookId);

            return mappedBook;
        }

        /// <summary>
        /// This function deletes a book from a repository by its ID.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteBook(Guid id)
        {
            // Find the book to be deleted by ID
            Book? existingBook = _repositoryWrapper.Book.FindByCondition(b => b.Id == id, includeInactive: true).FirstOrDefault();
            if (existingBook == null)
            {
                _logger.LogError("Book with ID {BookId} not found", id);
                throw new NotFoundException($"Book with ID {id} not found");
            }
            // Delete the book
            _repositoryWrapper.Book.Delete(existingBook);
            _repositoryWrapper.Save();
            _logger.LogInformation("Book with ID {BookId} deleted successfully", id);

        }

        /// <summary>
        /// This function decrypts and masks the ISBN number property of a given book object.
        /// </summary>
        /// <param name="T">a generic type parameter that represents the type of the book object being
        /// passed in as an argument to the method.</param>
        private void DecryptAndMaskIsbnNumber<T>(T book)
        {
            PropertyInfo? isbnNumberProperty = book?.GetType().GetProperty("IsbnNumber");
            if (isbnNumberProperty != null)
            {
                string? encryptedIsbnNumber = isbnNumberProperty.GetValue(book) as string;
                if (!string.IsNullOrEmpty(encryptedIsbnNumber))
                {
                    string decryptedIsbnNumber = Decrypt(encryptedIsbnNumber);

                    string maskedIsbnNumber = MaskIsbnNumber(decryptedIsbnNumber);
                    isbnNumberProperty.SetValue(book, maskedIsbnNumber);

                }
            }
        }

        /// <summary>
        /// The function masks all characters in an ISBN number except for the last four with asterisks.
        /// </summary>
        /// <param name="isbnNumber">A string representing an ISBN number.</param>
        /// <returns>
        /// The method returns a string that is the same as the input `isbnNumber`, except that all
        /// characters except the last four are replaced with asterisks
        /// </returns>
        private string MaskIsbnNumber(string isbnNumber)
        {
            int length = isbnNumber.Length;
            // Replace characters except the last four with asterisks
            return new string('*', length - 4) + isbnNumber.Substring(length - 4);
        }

        /// <summary>
        /// This function encrypts a given plain text using AES encryption with a specified key and initialization vector.
        /// </summary>
        /// <param name="plainText">The text that needs to be encrypted.</param>
        /// <returns>
        /// The method is returning a string that represents the encrypted version of the input plainText. The encrypted version is in Base64 format.
        /// </returns>
        private string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] key = Encoding.UTF8.GetBytes(_configuration["EncryptionKey"]!.Replace("YOUR_ENCRYPTION_KEY", _configuration["EncryptionKey"]));
                byte[] iv = Encoding.UTF8.GetBytes(_configuration["EncryptionIV"]!.Replace("YOUR_ENCRYPTION_IV", _configuration["EncryptionIV"]));

                // Adjust the key size if necessary
                if (key.Length != 16 && key.Length != 24 && key.Length != 32)
                {
                    Array.Resize(ref key, 32); // Set key size to 256 bits (32 bytes) if it's not already
                }

                // Adjust the IV size if necessary
                if (iv.Length != 16)
                {
                    Array.Resize(ref iv, 16); // Set IV size to 128 bits (16 bytes) if it's not already
                }

                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                byte[] encryptedBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                        cs.Write(plainBytes, 0, plainBytes.Length);
                    }

                    encryptedBytes = ms.ToArray();
                }

                return Convert.ToBase64String(encryptedBytes);
            }
        }

        /// <summary>
        /// This function decrypts a given string using AES encryption with a specified key and initialization vector.
        /// </summary>
        /// <param name="encryptedText">The text that has been encrypted and needs to be
        /// decrypted.</param>
        /// <returns>
        /// The method is returning a decrypted string that was originally encrypted using AES encryption.
        /// </returns>
        private string Decrypt(string encryptedText)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] key = Encoding.UTF8.GetBytes(_configuration["EncryptionKey"]!.Replace("YOUR_ENCRYPTION_KEY", _configuration["EncryptionKey"]));
                byte[] iv = Encoding.UTF8.GetBytes(_configuration["EncryptionIV"]!.Replace("YOUR_ENCRYPTION_IV", _configuration["EncryptionIV"]));

                // Adjust the key size if necessary
                if (key.Length != 16 && key.Length != 24 && key.Length != 32)
                {
                    Array.Resize(ref key, 32); // Set key size to 256 bits (32 bytes) if it's not already
                }

                // Adjust the IV size if necessary
                if (iv.Length != 16)
                {
                    Array.Resize(ref iv, 16); // Set IV size to 128 bits (16 bytes) if it's not already
                }

                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                byte[] decryptedBytes;
                using (MemoryStream ms = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        decryptedBytes = new byte[encryptedBytes.Length];
                        cs.Read(decryptedBytes, 0, decryptedBytes.Length);
                    }
                }
                string decryptedText = Encoding.UTF8.GetString(decryptedBytes).TrimEnd('\0');
                return decryptedText;
            }
        }

    }
}