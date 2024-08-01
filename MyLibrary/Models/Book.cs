using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLibrary.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [Display(Name = "שם הספר/הסט")]
        public string bookName { get; set; }
        [Display(Name = "רוחב הספר/הסט")]
        public int bookWidth { get; set; }
        [Display(Name = "אורך הספר/הסט")]
        public int bookHeight { get; set; }
        public int? GenreId { get; set; }
        public Genre? Genre { get; set; }
        public int? ShelfId { get; set; }
        public Shelf? Shelf { get; set; }
        [Display(Name = "האם זה סט ?")]
        public bool Set { get; set; }    
    }
}
