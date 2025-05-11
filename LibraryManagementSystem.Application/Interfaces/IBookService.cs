using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Application.Dtos;

namespace LibraryManagementSystem.Application.Interfaces
{
    public interface IBookService
    {
        Task<BookDto?> GetBookByIdAsync(int id); 
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto> CreateBookAsync(CreateBookDto createBookDto);
        Task UpdateBookAsync(int id, CreateBookDto updateBookDto); 
        Task DeleteBookAsync(int id); 
        Task<BorrowTransactionDto?> BorrowBookAsync(CreateBorrowTransactionDto borrowDto);
        Task ReturnBookAsync(int borrowTransactionId); 
    }
}
