using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Entities;
using System.Text.Json; 
using System.IO;      
using System.Linq;
using System.Text.Json.Serialization;

namespace LibraryManagementSystem.Infrastructure.Persistence.Repositories;

public class BookRepository : IBookRepository
{
    private readonly string _filePath;
    private List<Book> _books;
    private int _nextId = 1; 

    public BookRepository(string? filePath = null)
    {
        _filePath = filePath ?? Path.Combine(AppContext.BaseDirectory, "books.json");
        LoadBooks();
    }

    private void LoadBooks()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                _books = new List<Book>();
                SaveChanges();
                return;
            }
            var jsonData = File.ReadAllText(_filePath);
            _books = string.IsNullOrWhiteSpace(jsonData) ? new List<Book>() : JsonSerializer.Deserialize<List<Book>>(jsonData) ?? new List<Book>();
            if (_books.Any())
            {
                _nextId = _books.Max(b => b.Id) + 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading books: {ex.Message}");
            _books = new List<Book>();
        }
    }

    private void SaveChanges()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve }; // Preserve references for collections
            var jsonData = JsonSerializer.Serialize(_books, options);
            File.WriteAllText(_filePath, jsonData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving books: {ex.Message}"); 
        }
    }

    public async Task<Book> AddAsync(Book book)
    {
        book.Id = _nextId++;
        _books.Add(book);
        await Task.Run(() => SaveChanges());
        return book;
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await Task.FromResult(_books.AsEnumerable());
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await Task.FromResult(_books.FirstOrDefault(b => b.Id == id));
    }

    public async Task UpdateAsync(Book book)
    {
        var existingBookIndex = _books.FindIndex(b => b.Id == book.Id);
        if (existingBookIndex != -1)
        {
            _books[existingBookIndex] = book;
            await Task.Run(() => SaveChanges());
        }
    }

    public async Task DeleteAsync(int id)
    {
        var bookToRemove = _books.FirstOrDefault(b => b.Id == id);
        if (bookToRemove != null)
        {
            _books.Remove(bookToRemove);
            await Task.Run(() => SaveChanges());
        }
    }
}