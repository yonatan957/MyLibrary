using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLibrary.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [Display(Name = "שם הספר")]
        public string bookName { get; set; }
        [Display(Name = "רוחב הספר")]
        public int bookWidth { get; set; }
        [Display(Name = "גובה הספר/הסט")]
        public int bookHeight { get; set; }
        public int? GenreId { get; set; }
        public Genre? Genre { get; set; }
        public int? ShelfId { get; set; }
        public Shelf? Shelf { get; set; }
        [Display(Name = "האם זה חלק מסט ?")]
        public bool Set { get; set; }    
    }
}
