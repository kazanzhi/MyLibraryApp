namespace MyLibraryApp.Models
{
    public class UserBook
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public string UserId { get; set; }
        public AppUser AppUser { get; set; }
        public DateTime AddedAt { get; set; }
    }
}