using System.ComponentModel.DataAnnotations;

namespace MyLibrary.Models
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }
        public List<Book>? books { get; set; } = new List<Book>();
        public List<Shelf>? shelves { get; set; } = new List<Shelf>();
        public string Name { get; set; }
    }
}
