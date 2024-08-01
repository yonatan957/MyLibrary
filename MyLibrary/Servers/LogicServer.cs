using MyLibrary.Models;

namespace MyLibrary.Servers
{
    public static class LogicServer
    {
        public static Shelf setShlf(List<Shelf> shelves, int width, int height) 
        {
            foreach (Shelf shelf in shelves)
            {
                if (shelf.ShelfWidth >= width && shelf.ShelfHeight >= height)
                {
                    return shelf;
                }
            }
            return null;
        }
    }
}
