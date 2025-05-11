using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace LibraryManagementSystem.Application.Dtos
{
    public class CreateBookDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        public List<string> AuthorNames { get; set; } = new List<string>();
        
        public List<string> CategoryNames { get; set; } = new List<string>();

        [StringLength(50)]
        public string Language { get; set; } = string.Empty;

        [Required]
        public DateTime PublishingDate { get; set; }

        [Range(0, 10000)]
        public int TotalQuantity { get; set; }
    }
}
