namespace LibraryApplicationApi.models
{
    public class Borrowing
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public int MemberId { get; set; }

        public DateTime BorrowedDate { get; set; } =
            new DateTime(2026, 10, 6);

        public DateTime? ReturnedDate { get; set; } =
            new DateTime(2026, 10, 13);
        public Book? Book { get; set; }

        public Member? Member { get; set; }
    }
}
