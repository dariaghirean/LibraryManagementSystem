using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.Dtos
{
    public class CreateBorrowTransactionDto
    {
        [Required]
        public int BookID { get; set; }

        [Required]
        [StringLength(200)]
        public string BorrowerInfo { get; set; } = string.Empty;

        [Required]
        public DateTime BorrowDate { get; set; } = DateTime.UtcNow; 

        [Required]
        public DateTime DueDate { get; set; } 
    }
}
