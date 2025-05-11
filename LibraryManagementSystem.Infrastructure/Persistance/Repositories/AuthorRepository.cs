using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Infrastructure.Persistance.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly string _filePath;
        private List<Author> _authors;
        private int _nextId = 1;

        public AuthorRepository(string? filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppContext.BaseDirectory, "authors.json");
            LoadAuthors();
        }

        private void LoadAuthors()
        {
            if (!File.Exists(_filePath)) _authors = new List<Author>();
            else
            {
                var jsonData = File.ReadAllText(_filePath);
                _authors = string.IsNullOrWhiteSpace(jsonData) ? new List<Author>() : JsonSerializer.Deserialize<List<Author>>(jsonData) ?? new List<Author>();
            }
            if (_authors.Any()) _nextId = _authors.Max(a => a.Id) + 1; else _nextId = 1;
        }

        private void SaveChanges()
        {
            var options = new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve };
            File.WriteAllText(_filePath, JsonSerializer.Serialize(_authors, options));
        }

        public async Task<Author> AddAsync(Author author)
        {
            author.Id = _nextId++;
            _authors.Add(author);
            await Task.Run(() => SaveChanges());
            return author;
        }

        public async Task<Author?> GetByIdAsync(int id)
        {
            return await Task.FromResult(_authors.FirstOrDefault(a => a.Id == id));
        }

        public async Task<Author?> GetByNameAsync(string name)
        {
            return await Task.FromResult(_authors.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            return await Task.FromResult(_authors.AsEnumerable());
        }
    }
}
