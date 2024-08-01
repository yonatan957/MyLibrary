using System.ComponentModel.DataAnnotations;

namespace MyLibrary.Models
{
    public class Shelf
    {
        [Key]
        public int ShelfId { get; set; }
        [Display(Name = "מקום פנוי")]
        public int ShelfWidth { get; set; }
        [Display(Name = "גובה")]
        public int ShelfHeight { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public List<Book> Books { get; set; }
    }
}
