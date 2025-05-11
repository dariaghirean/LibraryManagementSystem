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
    public class CategoryRepository : ICategoryRepository 
    {
        private readonly string _filePath;
        private List<Category> _categories;
        private int _nextId = 1;

        public CategoryRepository(string? filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppContext.BaseDirectory, "categories.json");
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    _categories = new List<Category>();
                    SaveChanges(); 
                    return;
                }

                var jsonData = File.ReadAllText(_filePath);
                _categories = string.IsNullOrWhiteSpace(jsonData) ? new List<Category>() : JsonSerializer.Deserialize<List<Category>>(jsonData) ?? new List<Category>();

                if (_categories.Any())
                {
                    _nextId = _categories.Max(c => c.Id) + 1;
                }
                else
                {
                    _nextId = 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading categories: {ex.Message}"); 
                _categories = new List<Category>();
                _nextId = 1;
            }
        }

        private void SaveChanges()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve };
                var jsonData = JsonSerializer.Serialize(_categories, options);
                File.WriteAllText(_filePath, jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving categories: {ex.Message}"); 
            }
        }

        public async Task<Category> AddAsync(Category category)
        {
            category.Id = _nextId++;
            _categories.Add(category);
            await Task.Run(() => SaveChanges());
            return category;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await Task.FromResult(_categories.AsEnumerable());
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await Task.FromResult(_categories.FirstOrDefault(c => c.Id == id));
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await Task.FromResult(_categories.FirstOrDefault(c => c.CategoryName.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
