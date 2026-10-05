using System;
using System.Collections.Generic;
using System.Text;

namespace library_project
{
public class Book : ReservableLibraryItem
    {
        public string Author { get; }
        public string Isbn { get; }

        public Book(string title, string author, string isbn, int publicationYear)
            : base(title, publicationYear, new DailyRateFinePolicy(0.50m))
        {
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("A book must have an author.", nameof(author));
            Author = author.Trim();
            Isbn = isbn;
        }

        public override int LoanPeriodDays => 21;
        public override string ItemType => "Book";
        public override string Describe() => $"{base.Describe()} by {Author} — ISBN {Isbn}";
    }
}
