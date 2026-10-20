using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LibraryApplicationApi.Dtos
{
    public class BorrowingRequest : Controller
    { }
namespace LibraryApi.DTOs
    {
        public class CreateBorrowingRequest
        {
            [Range(1, int.MaxValue)]
            public int BookId { get; set; }

            [Range(1, int.MaxValue)]
            public int MemberId { get; set; }

            [Required]
            public DateTime DueDate { get; set; }
        }
    }
        }
    