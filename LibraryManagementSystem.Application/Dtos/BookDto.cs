using LibraryManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.Dtos
{
    public class BookDto
    {
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public string Language { get; set; } = string.Empty;
        public DateTime PublishingDate { get; set; }
        public int TotalQuantity { get; set; }
        public int AvailableQuantity { get; set; }
    }
}
