using System.Text.Json.Serialization;

namespace MyLibraryApp.Models
{
        public class Category
        {
            public int Id { get; set; }
            public string Name { get; set; }
            [JsonIgnore]
            public ICollection<Book> Books { get; set; } = new List<Book>();
        }
}
/*
Technology
Science
Education
Health & Wellness
Business & Finance
Entertainment
Sports
Travel
Food & Cooking
Lifestyle
Gaming
Music
Movies & TV Shows
Art & Design
Books & Literature
Photography
Fitness & Exercise
Personal Development
Fashion & Beauty
History & Culture
*/