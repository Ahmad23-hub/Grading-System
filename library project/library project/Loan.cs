using System;
using System.Collections.Generic;
using System.Text;
using static library_project.Program;

namespace library_project
{
    internal class Loan
    {
        using System;

// =============================================================================
// LEVEL 9 — COMPOSITION / AGGREGATION
// =============================================================================
public class Loan
    {
        public LibraryItem Item { get; }
        public Member Borrower { get; }
        public DateOnly BorrowedOn { get; }
        public DateOnly DueOn { get; }
        public DateOnly? ReturnedOn { get; private set; }

        public Loan(LibraryItem item, Member borrower, DateOnly borrowedOn)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
            Borrower = borrower ?? throw new ArgumentNullException(nameof(borrower));
            BorrowedOn = borrowedOn;
            DueOn = borrowedOn.AddDays(item.LoanPeriodDays); // the item decides; the loan just asks
        }

        public bool IsReturned => ReturnedOn.HasValue;

        public int DaysLate => ReturnedOn is null
            ? 0
            : Math.Max(0, ReturnedOn.Value.DayNumber - DueOn.DayNumber);

        // Days late as of any date, even before the item is returned — used for
        // the "overdue and accruing" line in the daily report.
        public int DaysLateAsOf(DateOnly asOf) =>
            IsReturned ? DaysLate : Math.Max(0, asOf.DayNumber - DueOn.DayNumber);

        // Delegation: the loan asks the item for the full fine; the member (not
        // shown here) is the one who applies any discount on top of it.
        public decimal Fine => Item.CalculateFine(DaysLate);

        public void Complete(DateOnly returnedOn)
        {
            if (IsReturned)
                throw new InvalidOperationException("This loan has already been closed.");
            if (returnedOn < BorrowedOn)
                throw new ArgumentException("An item cannot be returned before it was borrowed.",
                    nameof(returnedOn));
            ReturnedOn = returnedOn;
        }
    }
}
}
