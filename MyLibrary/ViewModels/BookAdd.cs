using MyLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace MyLibrary.ViewModels
{
    public class BookAdd
    {
        public List<Book>? Book { get; set; }
        public int id { get; set; }
        [Display(Name = "שם סט")]
        public string? set { get; set; }

    }
}
