using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using LibraryManagementSystem.Application.Dtos;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBorrowTransactionRepository _borrowTransactionRepository;

        public BookService(
            IBookRepository bookRepository,
            IAuthorRepository authorRepository,
            ICategoryRepository categoryRepository,
            IBorrowTransactionRepository borrowTransactionRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _borrowTransactionRepository = borrowTransactionRepository;
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
        {
            var book = new Book
            {
                Title = createBookDto.Title,
                Language = createBookDto.Language,
                PublishingDate = createBookDto.PublishingDate,
                TotalQuantity = createBookDto.TotalQuantity,
                AvailableQuantity = createBookDto.TotalQuantity 
            };

            foreach (var authorName in createBookDto.AuthorNames)
            {
                var author = await _authorRepository.GetByNameAsync(authorName)
                             ?? await _authorRepository.AddAsync(new Author { Name = authorName });
                book.Authors.Add(author);
            }

            foreach (var categoryName in createBookDto.CategoryNames)
            {
                var category = await _categoryRepository.GetByNameAsync(categoryName)
                               ?? await _categoryRepository.AddAsync(new Category { CategoryName = categoryName });
                book.Categories.Add(category);
            }

            var addedBook = await _bookRepository.AddAsync(book);
            return MapBookToDto(addedBook); 
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            return books.Select(MapBookToDto).ToList();
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            return book == null ? null : MapBookToDto(book);
        }

        // Implement UpdateBookAsync and DeleteBookAsync similarly

        public async Task<BorrowTransactionDto?> BorrowBookAsync(CreateBorrowTransactionDto borrowDto)
        {
            var book = await _bookRepository.GetByIdAsync(borrowDto.BookID);
            if (book == null || book.AvailableQuantity <= 0)
            {
                return null; 
            }

            book.AvailableQuantity--;
            await _bookRepository.UpdateAsync(book);

            var transaction = new BorrowTransaction
            {
                BookID = borrowDto.BookID,
                BorrowerInfo = borrowDto.BorrowerInfo,
                BorrowDate = borrowDto.BorrowDate,
                DueDate = borrowDto.DueDate, 
            };

            var addedTransaction = await _borrowTransactionRepository.AddAsync(transaction);
            return MapTransactionToDto(addedTransaction, book.Title);
        }

        public async Task ReturnBookAsync(int borrowTransactionId)
        {
            var transaction = await _borrowTransactionRepository.GetByIdAsync(borrowTransactionId);
            if (transaction == null || transaction.ReturnDate.HasValue)
            {
                           return;
            }

            transaction.ReturnDate = DateTime.UtcNow;
            await _borrowTransactionRepository.UpdateAsync(transaction);

            var book = await _bookRepository.GetByIdAsync(transaction.BookID);
            if (book != null)
            {
                book.AvailableQuantity++;
                await _bookRepository.UpdateAsync(book);
            }
        }


        // --- MAPPING HELPERS ---
        private BookDto MapBookToDto(Book book)
        {
            return new BookDto
            {
                ID = book.Id,
                Title = book.Title,
                Language = book.Language,
                PublishingDate = book.PublishingDate,
                TotalQuantity = book.TotalQuantity,
                AvailableQuantity = book.AvailableQuantity,
                Authors = book.Authors.Select(a => new AuthorDto { Id = a.Id, Name = a.Name }).ToList(),
                Categories = book.Categories.Select(c => new CategoryDto { Id = c.Id, CategoryName = c.CategoryName }).ToList()
            };
        }

        private BorrowTransactionDto MapTransactionToDto(BorrowTransaction transaction, string bookTitle = "")
        {
            return new BorrowTransactionDto
            {
                Id = transaction.Id,
                BookID = transaction.BookID,
                BookTitle = bookTitle, 
                BorrowerInfo = transaction.BorrowerInfo,
                BorrowDate = transaction.BorrowDate,
                DueDate = transaction.DueDate,
                ReturnDate = transaction.ReturnDate
            };
        }
        
        public async Task UpdateBookAsync(int id, CreateBookDto updateBookDto)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                return;
            }

            book.Title = updateBookDto.Title;
            book.Language = updateBookDto.Language;
            book.PublishingDate = updateBookDto.PublishingDate;
            int borrowedCount = book.TotalQuantity - book.AvailableQuantity;
            book.TotalQuantity = updateBookDto.TotalQuantity;
            book.AvailableQuantity = book.TotalQuantity - borrowedCount;
            if (book.AvailableQuantity < 0) book.AvailableQuantity = 0; 

            book.Authors.Clear();
            foreach (var authorName in updateBookDto.AuthorNames)
            {
                var author = await _authorRepository.GetByNameAsync(authorName)
                             ?? await _authorRepository.AddAsync(new Author { Name = authorName });
                book.Authors.Add(author);
            }

            book.Categories.Clear();
            foreach (var categoryName in updateBookDto.CategoryNames)
            {
                var category = await _categoryRepository.GetByNameAsync(categoryName)
                               ?? await _categoryRepository.AddAsync(new Category { CategoryName = categoryName });
                book.Categories.Add(category);
            }

            await _bookRepository.UpdateAsync(book);
        }


              public async Task DeleteBookAsync(int id)
        {
            var activeBorrows = await _borrowTransactionRepository.GetActiveBorrowsByBookIdAsync(id);
            if (activeBorrows.Any())
            {
                throw new InvalidOperationException("Cannot delete book with active borrow transactions.");
                           }
            await _bookRepository.DeleteAsync(id);
        }

    }
}