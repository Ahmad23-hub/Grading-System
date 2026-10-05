using System;
using System.Collections.Generic;
using System.Text;

namespace library_project
{
 
        public abstract class LibraryItem
        {
            private static int _nextId = 1;

            public int Id { get; }
            public string Title { get; }
            public int PublicationYear { get; }
            public bool IsOnLoan { get; private set; }

            private readonly IFinePolicy _finePolicy;

            protected LibraryItem(string title, int publicationYear, IFinePolicy finePolicy)
            {
                if (string.IsNullOrWhiteSpace(title))
                    throw new ArgumentException("An item must have a title.", nameof(title));
                if (publicationYear < 1450 || publicationYear > DateTime.Now.Year + 1)
                    throw new ArgumentOutOfRangeException(nameof(publicationYear),
                        "Publication year is outside the plausible range.");

                Id = _nextId++;
                Title = title.Trim();
                PublicationYear = publicationYear;
                _finePolicy = finePolicy ?? throw new ArgumentNullException(nameof(finePolicy));
            }

            public abstract int LoanPeriodDays { get; }
            public abstract string ItemType { get; }

            // Level 10.2 — one virtual switch instead of a chain of "is this a DVD /
            // AudioBook / ..." checks anywhere that borrowing happens.
            public virtual bool AllowsConcurrentLoans => false;

            public virtual string Describe() => $"[{Id}] {ItemType}: \"{Title}\" ({PublicationYear})";

            // The fine rate is delegated to the injected policy instead of being a
            // hard-coded virtual property on every subclass.
            public decimal CalculateFine(int daysLate) => _finePolicy.Calculate(daysLate);

            public virtual void MarkAsBorrowed()
            {
                if (IsOnLoan)
                    throw new InvalidOperationException($"\"{Title}\" is already on loan.");
                IsOnLoan = true;
            }

            public virtual void MarkAsReturned()
            {
                if (!IsOnLoan)
                    throw new InvalidOperationException($"\"{Title}\" is not currently on loan.");
                IsOnLoan = false;
            }
        }
    }
