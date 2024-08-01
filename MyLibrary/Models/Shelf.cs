using System.ComponentModel.DataAnnotations;

namespace MyLibrary.Models
{
    public class Shelf
    {
        [Key]
        public int ShelfId { get; set; }
        public int ShelfWidth { get; set; }
        public int ShelfHeight { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public List<Book> Books { get; set; }
    }
}
