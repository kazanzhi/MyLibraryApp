using System.Text.Json.Serialization;

namespace MyLibraryApp.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string Content { get; set; }
        public int PublishedYear { get; set; }

        [JsonIgnore]
        public ICollection<UserBook> UserBooks{ get; set; } = new List<UserBook>();
    }
}
