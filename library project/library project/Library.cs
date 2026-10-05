using System;
using System.Collections.Generic;
using System.Text;
using static library_project.Program;

namespace library_project
{
    internal class Library
    {
        private readonly List<LibraryItem> _items = new();
        private readonly List<Member> _members = new();
        private readonly List<Loan> _loans = new();

        public string Name { get; }

        public Library(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("A library needs a name.", nameof(name));
            Name = name;
        }

        public IReadOnlyList<LibraryItem> Items => _items;
        public IReadOnlyList<Member> Members => _members;
        public IReadOnlyList<Loan> Loans => _loans;

        public void AddItem(LibraryItem item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            _items.Add(item);
        }

        public void RegisterMember(Member member)
        {
            if (member is null) throw new ArgumentNullException(nameof(member));
            if (_members.Any(m => m.MembershipId == member.MembershipId))
                throw new InvalidOperationException($"Membership id {member.MembershipId} is already taken.");
            _members.Add(member);
        }

        public Loan Borrow(int itemId, string membershipId, DateOnly today)
        {
            LibraryItem item = FindItem(itemId);
            Member member = FindMember(membershipId);

            if (!member.CanBorrow)
                throw new InvalidOperationException(
                    $"{member.Name} already has the maximum of {member.MaxActiveLoans} items on loan.");

            // Level 10.2 — a single polymorphic check, not a chain of type checks.
            if (!item.AllowsConcurrentLoans && item.IsOnLoan)
                throw new InvalidOperationException($"\"{item.Title}\" is already on loan.");

            // Level 10.5 — reservation queue, with Premium/Staff able to bypass it.
            if (item is IReservable reservable && reservable.IsReserved)
            {
                Member nextInLine = reservable.ReservationQueue.First();
                if (nextInLine.MembershipId != member.MembershipId && !member.BypassesReservationQueue)
                    throw new InvalidOperationException($"\"{item.Title}\" is reserved for {nextInLine.Name}.");
            }

            item.MarkAsBorrowed();

            // Only consume the reservation if the borrower actually was the one waiting.
            if (item is IReservable r && r.IsReserved && r.ReservationQueue.First().MembershipId == member.MembershipId)
                r.ReleaseNextReservation();

            var loan = new Loan(item, member, today);
            _loans.Add(loan);
            member.Attach(loan);
            return loan;
        }

        /// <summary>Closes the open loan for an item/member pair and returns the discounted fine.</summary>
        public decimal Return(int itemId, string membershipId, DateOnly today)
        {
            Loan loan = _loans.FirstOrDefault(l => l.Item.Id == itemId
                                                 && l.Borrower.MembershipId == membershipId
                                                 && !l.IsReturned)
                ?? throw new InvalidOperationException(
                    $"There is no open loan for item {itemId} and member {membershipId}.");
            return CloseLoan(loan, today);
        }

        public decimal Return(int itemId, DateOnly today)
        {
            Loan loan = _loans.FirstOrDefault(l => l.Item.Id == itemId && !l.IsReturned)
                ?? throw new InvalidOperationException($"There is no open loan for item {itemId}.");
            return CloseLoan(loan, today);
        }

        private decimal CloseLoan(Loan loan, DateOnly today)
        {
            loan.Complete(today);
            loan.Item.MarkAsReturned();
            return loan.Borrower.ApplyFineDiscount(loan.Fine);
        }

        public IEnumerable<LibraryItem> Search(string term) =>
            _items.Where(i => i.Describe().Contains(term, StringComparison.OrdinalIgnoreCase));

        public IEnumerable<LibraryItem> AvailableItems() => _items.Where(i => !i.IsOnLoan);

        public IEnumerable<Loan> OverdueLoans(DateOnly today) =>
            _loans.Where(l => !l.IsReturned && l.DueOn < today);

        // Level 10.6 — the librarian's daily summary.
        public string DailySummaryReport(DateOnly today)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"=== {Name} — Daily Summary ({today}) ===");

            var activeLoans = _loans.Where(l => !l.IsReturned).ToList();
            sb.AppendLine($"Items out: {activeLoans.Count}");

            var overdue = OverdueLoans(today).ToList();
            if (overdue.Count == 0)
            {
                sb.AppendLine("Overdue items: none");
            }
            else
            {
                sb.AppendLine("Overdue items:");
                foreach (var loan in overdue)
                {
                    int daysLate = loan.DaysLateAsOf(today);
                    decimal accruing = loan.Borrower.ApplyFineDiscount(loan.Item.CalculateFine(daysLate));
                    sb.AppendLine($"  \"{loan.Item.Title}\" — {daysLate} day(s) late, " +
                                   $"borrowed by {loan.Borrower.Name}, fine accruing {accruing:C}");
                }
            }

            var atLimit = _members.Where(m => !m.CanBorrow).ToList();
            sb.AppendLine(atLimit.Count == 0
                ? "Members at their loan limit: none"
                : $"Members at their loan limit: {string.Join(", ", atLimit.Select(m => m.Name))}");

            decimal totalOwed = _members.Sum(m => m.TotalFinesOwed);
            sb.AppendLine($"Total outstanding fines: {totalOwed:C}");

            return sb.ToString();
        }

        private LibraryItem FindItem(int id) =>
            _items.FirstOrDefault(i => i.Id == id)
            ?? throw new KeyNotFoundException($"No item with id {id}.");

        private Member FindMember(string membershipId) =>
            _members.FirstOrDefault(m => m.MembershipId == membershipId)
            ?? throw new KeyNotFoundException($"No member with id {membershipId}.");
    }
}
