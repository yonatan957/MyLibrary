using MyLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace MyLibrary.ViewModels
{
    public class GenreAddShelf
    {
        public Genre Genre { get; set; }
        [Display(Name = "רוחב")]
        public int width { get; set; }
        [Display(Name = "גובה")]
        public int height { get; set; }
    }
}
