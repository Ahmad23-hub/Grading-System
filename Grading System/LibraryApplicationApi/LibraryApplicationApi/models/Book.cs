namespace LibraryApplicationApi.models
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; } = "LIBRARY";

        public string Author { get; set; } = "JHON DOE";

        public string ISBN { get; set; } = "978-1234567890";

        public int PublishedYear { get; set; } = 2026;

        public bool IsAvailable { get; set; } = true;
    }
}