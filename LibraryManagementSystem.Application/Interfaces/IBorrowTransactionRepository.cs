using LibraryManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.Interfaces
{
    public interface IBorrowTransactionRepository
    {
        Task<BorrowTransaction?> GetByIdAsync(int id);
        Task<IEnumerable<BorrowTransaction>> GetAllAsync();
        Task<IEnumerable<BorrowTransaction>> GetByBookIdAsync(int bookId);
        Task<IEnumerable<BorrowTransaction>> GetActiveBorrowsByBookIdAsync(int bookId);
        Task<BorrowTransaction> AddAsync(BorrowTransaction transaction);
        Task UpdateAsync(BorrowTransaction transaction);
    }
}
