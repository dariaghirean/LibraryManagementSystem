using LibraryManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.Interfaces
{
    public interface IAuthorRepository
    {
        Task<Author?> GetByIdAsync(int id);
        Task<Author?> GetByNameAsync(string name);
        Task<IEnumerable<Author>> GetAllAsync();
        Task<Author> AddAsync(Author author);
    }
}
