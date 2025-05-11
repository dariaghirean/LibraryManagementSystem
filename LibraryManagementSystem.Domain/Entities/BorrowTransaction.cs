using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Domain.Entities
{
    public class BorrowTransaction
    {
        public int Id { get; set; } 
        public int BookID { get; set; } 
        public Book Book { get; set; } 
        public string BorrowerInfo { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; } 
    }
}
