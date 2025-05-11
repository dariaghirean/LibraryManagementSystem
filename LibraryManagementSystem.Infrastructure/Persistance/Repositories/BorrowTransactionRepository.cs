using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Entities;
using System.IO;
using System.Text.Json;


namespace LibraryManagementSystem.Infrastructure.Persistance.Repositories
{
    public class BorrowTransactionRepository : IBorrowTransactionRepository 
    {
        private readonly string _filePath;
        private List<BorrowTransaction> _transactions;
        private int _nextId = 1;

        public BorrowTransactionRepository(string? filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppContext.BaseDirectory, "borrowtransactions.json");
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    _transactions = new List<BorrowTransaction>();
                    SaveChanges();
                    return;
                }

                var jsonData = File.ReadAllText(_filePath);
                _transactions = string.IsNullOrWhiteSpace(jsonData) ? new List<BorrowTransaction>() : JsonSerializer.Deserialize<List<BorrowTransaction>>(jsonData) ?? new List<BorrowTransaction>();

                if (_transactions.Any())
                {
                    _nextId = _transactions.Max(t => t.Id) + 1;
                }
                else
                {
                    _nextId = 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading borrow transactions: {ex.Message}");
                _transactions = new List<BorrowTransaction>();
                _nextId = 1;
            }
        }

        private void SaveChanges()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true }; 
                var jsonData = JsonSerializer.Serialize(_transactions, options);
                File.WriteAllText(_filePath, jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving borrow transactions: {ex.Message}");
            }
        }

        public async Task<BorrowTransaction> AddAsync(BorrowTransaction transaction)
        {
            transaction.Id = _nextId++;
            _transactions.Add(transaction);
            await Task.Run(() => SaveChanges());
            return transaction;
        }

        public async Task<IEnumerable<BorrowTransaction>> GetAllAsync()
        {
            return await Task.FromResult(_transactions.AsEnumerable());
        }

        public async Task<BorrowTransaction?> GetByIdAsync(int id)
        {
            return await Task.FromResult(_transactions.FirstOrDefault(t => t.Id == id));
        }

        public async Task<IEnumerable<BorrowTransaction>> GetByBookIdAsync(int bookId)
        {
            return await Task.FromResult(_transactions.Where(t => t.BookID == bookId).ToList());
        }

        public async Task<IEnumerable<BorrowTransaction>> GetActiveBorrowsByBookIdAsync(int bookId)
        {
            return await Task.FromResult(_transactions.Where(t => t.BookID == bookId && !t.ReturnDate.HasValue).ToList());
        }

        public async Task UpdateAsync(BorrowTransaction transaction)
        {
            var existingTransactionIndex = _transactions.FindIndex(t => t.Id == transaction.Id);
            if (existingTransactionIndex != -1)
            {
                _transactions[existingTransactionIndex] = transaction;
                await Task.Run(() => SaveChanges());
            }
        }
    }
}
