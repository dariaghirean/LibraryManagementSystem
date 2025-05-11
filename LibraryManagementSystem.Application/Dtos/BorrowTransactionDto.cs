using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.Dtos
{
    public class BorrowTransactionDto
    {
        public int Id { get; set; }
        public int BookID { get; set; }
        public string BookTitle { get; set; } 
        public string BorrowerInfo { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
