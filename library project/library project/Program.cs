using System.Text;

namespace library_project
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var today = new DateOnly(2026, 3, 2);
            var library = new Library("Riverside Community Library");

            // --- Stocking the shelves: four physical types + one digital type. ---
            var hobbit = new Book("The Hobbit", "J.R.R. Tolkien", "978-0261102217", 1937);
            var cleanCode = new Book("Clean Code", "Robert C. Martin", "978-0132350884", 2008);
            var inception = new Dvd("Inception", 148, "PG-13", 2010);
            var newSci = new Magazine("New Scientist", 3521, 2026);
            var hailMary = new AudioBook("Project Hail Mary", "Ray Porter", 963, 2021,
                                           850, "https://library.example/audio/hail-mary");

            library.AddItem(hobbit);
            library.AddItem(cleanCode);
            library.AddItem(inception);
            library.AddItem(newSci);
            library.AddItem(hailMary);

            // --- Registering members of every tier (Level 10.3). ---
            var amina = new StandardMember("M-001", "Amina Bello");
            var tunde = new StandardMember("M-002", "Tunde Okafor");
            var ngozi = new PremiumMember("M-003", "Ngozi Adeyemi");
            var bola = new StaffMember("S-001", "Bola (Librarian)");

            library.RegisterMember(amina);
            library.RegisterMember(tunde);
            library.RegisterMember(ngozi);
            library.RegisterMember(bola);

            Console.WriteLine($"=== {library.Name} ===\n");

            // --- POLYMORPHISM: one loop, one call, five different behaviours. ---
            Console.WriteLine("-- Catalogue --");
            foreach (LibraryItem item in library.Items)
                Console.WriteLine($"{item.Describe()} | loan {item.LoanPeriodDays} days");

            // --- Borrowing a physical item ---
            Console.WriteLine("\n-- Borrowing --");
            var loan1 = library.Borrow(hobbit.Id, amina.MembershipId, today);
            Console.WriteLine($"{amina.Name} borrowed \"{loan1.Item.Title}\" — due {loan1.DueOn}");

            // --- IDigital: an audiobook can be "borrowed" by many members at once (Level 10.2) ---
            Console.WriteLine("\n-- Digital items allow concurrent loans --");
            library.Borrow(hailMary.Id, amina.MembershipId, today);
            library.Borrow(hailMary.Id, tunde.MembershipId, today);
            Console.WriteLine($"{amina.Name} and {tunde.Name} are both \"borrowing\" \"{hailMary.Title}\" " +
                              "right now — allowed, because it is digital and never actually goes on loan.");

            // --- Reservation queue: more than one member can wait in line (Level 10.5) ---
            Console.WriteLine("\n-- Reservation queue --");
            Console.WriteLine($"Can a magazine be reserved? {newSci is IReservable}");
            cleanCode.Reserve(tunde);
            cleanCode.Reserve(ngozi);
            Console.WriteLine($"Queue for \"{cleanCode.Title}\": " +
                               string.Join(" -> ", cleanCode.ReservationQueue.Select(m => m.Name)));

            TryIt("Amina (not in the queue) tries to borrow the reserved book",
                  () => library.Borrow(cleanCode.Id, amina.MembershipId, today));

            Console.WriteLine("Ngozi is Premium, so she can bypass the queue without taking the reservation:");
            var bypassLoan = library.Borrow(cleanCode.Id, ngozi.MembershipId, today);
            Console.WriteLine($"{ngozi.Name} borrowed \"{bypassLoan.Item.Title}\" — queue is still " +
                               string.Join(" -> ", cleanCode.ReservationQueue.Select(m => m.Name)));
            library.Return(cleanCode.Id, ngozi.MembershipId, today.AddDays(2));

            var tundeLoan = library.Borrow(cleanCode.Id, tunde.MembershipId, today.AddDays(2));
            Console.WriteLine($"{tunde.Name}, first in line, borrowed \"{tundeLoan.Item.Title}\". " +
                               $"Queue is now: {string.Join(" -> ", cleanCode.ReservationQueue.Select(m => m.Name))}");

            // --- Returning late, with a fine rate on the item and a discount on the member (Level 10.3+4) ---
            Console.WriteLine("\n-- Returning late: item sets the rate, member sets the discount --");
            var fineHobbit = library.Return(hobbit.Id, amina.MembershipId, today.AddDays(25)); // 21-day loan, 4 late
            Console.WriteLine($"\"{hobbit.Title}\" returned 4 days late by {amina.Name} (standard member) " +
                               $"-> fine {fineHobbit:C}");

            Console.WriteLine("\n-- Staff borrow without limits or fines --");
            var staffLoan = library.Borrow(inception.Id, bola.MembershipId, today);
            Console.WriteLine($"{bola.Name} borrowed \"{staffLoan.Item.Title}\" — " +
                               $"staff members owe {bola.TotalFinesOwed:C} no matter how late a return is.");

            // --- Daily summary report (Level 10.6) ---
            Console.WriteLine();
            Console.WriteLine(library.DailySummaryReport(today.AddDays(30)));

            // Small helper so a deliberate failure prints tidily instead of crashing the demo.
            static void TryIt(string what, Action action)
            {
                try { action(); Console.WriteLine($"{what}: allowed (!)"); }
                catch (InvalidOperationException ex) { Console.WriteLine($"{what}: blocked — {ex.Message}"); }
            }
        }
public interface IFinePolicy
        {
            decimal Calculate(int daysLate);
        }

        public class DailyRateFinePolicy : IFinePolicy
        {
            private readonly decimal _dailyRate;
            public DailyRateFinePolicy(decimal dailyRate) => _dailyRate = dailyRate;
            public decimal Calculate(int daysLate) => daysLate <= 0 ? 0m : daysLate * _dailyRate;
        }

        // Example of the payoff: swap this policy in when constructing items during an
        // amnesty week. No other class in the file needs to change.
        public class AmnestyFinePolicy : IFinePolicy
        {
            public decimal Calculate(int daysLate) => 0m;
        }

        // =============================================================================
        // LEVEL 8 — IReservable, now backed by a queue (Level 10.5) instead of a
        // single name, so more than one member can wait for the same item.
        // =============================================================================
        public interface IReservable
        {
            bool IsReserved { get; }
            IReadOnlyCollection<Member> ReservationQueue { get; }
            void Reserve(Member member);
            Member? ReleaseNextReservation();
        }

        // =============================================================================
        // LEVEL 10.2 — IDigital: a capability, not an identity. Any item that
        // implements it can be "borrowed" by any number of members at once.
        // =============================================================================
        public interface IDigital
        {
            long FileSizeMb { get; }
            string DownloadUrl { get; }
        }

        // =============================================================================
        // LEVEL 7 — ABSTRACTION. LibraryItem says WHAT every item must do, not HOW.
        // =============================================================================
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

        // A reusable base for every item that can be reserved, so Book/Dvd/AudioBook
        // don't each duplicate the same queue logic.
        public abstract class ReservableLibraryItem : LibraryItem, IReservable
        {
            private readonly Queue<Member> _reservationQueue = new();

            protected ReservableLibraryItem(string title, int publicationYear, IFinePolicy finePolicy)
                : base(title, publicationYear, finePolicy) { }

            public bool IsReserved => _reservationQueue.Count > 0;
            public IReadOnlyCollection<Member> ReservationQueue => _reservationQueue;

            public void Reserve(Member member)
            {
                if (_reservationQueue.Any(m => m.MembershipId == member.MembershipId))
                    throw new InvalidOperationException($"{member.Name} has already reserved \"{Title}\".");
                _reservationQueue.Enqueue(member);
            }

            public Member? ReleaseNextReservation() =>
                _reservationQueue.Count > 0 ? _reservationQueue.Dequeue() : null;
        }

        // =============================================================================
        // LEVEL 5/6 — INHERITANCE + POLYMORPHISM, plus the Level 10.1 addition (AudioBook)
        // =============================================================================
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

        public class Dvd : ReservableLibraryItem
        {
            public int RuntimeMinutes { get; }
            public string AgeRating { get; }

            public Dvd(string title, int runtimeMinutes, string ageRating, int publicationYear)
                : base(title, publicationYear, new DailyRateFinePolicy(1.00m))
            {
                if (runtimeMinutes <= 0)
                    throw new ArgumentOutOfRangeException(nameof(runtimeMinutes), "Runtime must be positive.");
                RuntimeMinutes = runtimeMinutes;
                AgeRating = ageRating;
            }

            public override int LoanPeriodDays => 7;
            public override string ItemType => "DVD";
            public override string Describe() => $"{base.Describe()} — {RuntimeMinutes} min, rated {AgeRating}";
        }

        // Magazine stays a plain LibraryItem — it is deliberately NOT reservable.
        public class Magazine : LibraryItem
        {
            public int IssueNumber { get; }

            public Magazine(string title, int issueNumber, int publicationYear)
                : base(title, publicationYear, new DailyRateFinePolicy(0.25m))
            {
                IssueNumber = issueNumber;
            }

            public override int LoanPeriodDays => 3;
            public override string ItemType => "Magazine";
            public override string Describe() => $"{base.Describe()} — issue #{IssueNumber}";
        }

        // LEVEL 10.1 + 10.2 — a fourth item type, added without touching the printing
        // loop, Borrow, or Return. It is reservable AND digital.
        public class AudioBook : ReservableLibraryItem, IDigital
        {
            public string Narrator { get; }
            public int DurationMinutes { get; }
            public long FileSizeMb { get; }
            public string DownloadUrl { get; }

            public AudioBook(string title, string narrator, int durationMinutes, int publicationYear,
                              long fileSizeMb, string downloadUrl)
                : base(title, publicationYear, new DailyRateFinePolicy(0.75m))
            {
                if (string.IsNullOrWhiteSpace(narrator))
                    throw new ArgumentException("An audiobook must have a narrator.", nameof(narrator));
                if (durationMinutes <= 0)
                    throw new ArgumentOutOfRangeException(nameof(durationMinutes), "Duration must be positive.");

                Narrator = narrator.Trim();
                DurationMinutes = durationMinutes;
                FileSizeMb = fileSizeMb;
                DownloadUrl = downloadUrl;
            }

            public override int LoanPeriodDays => 14;
            public override string ItemType => "AudioBook";

            // The one override that makes concurrent digital borrowing work — no type
            // checks needed anywhere else in the program.
            public override bool AllowsConcurrentLoans => true;
            public override void MarkAsBorrowed() { /* digital items are never exclusively "on loan" */ }
            public override void MarkAsReturned() { /* nothing to release */ }

            public override string Describe() =>
                $"{base.Describe()} — narrated by {Narrator}, {DurationMinutes} min (digital)";
        }

        // =============================================================================
        // LEVEL 10.3 — MEMBERSHIP TIERS. The item decides the fine rate; the member
        // decides the discount. Neither one needs to know how the other works.
        // =============================================================================
        public abstract class Member
        {
            private readonly List<Loan> _loans = new();

            public string MembershipId { get; }
            public string Name { get; }
            public IReadOnlyList<Loan> Loans => _loans;

            protected Member(string membershipId, string name)
            {
                if (string.IsNullOrWhiteSpace(membershipId))
                    throw new ArgumentException("Membership id is required.", nameof(membershipId));
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Name is required.", nameof(name));

                MembershipId = membershipId;
                Name = name.Trim();
            }

            public abstract int MaxActiveLoans { get; }
            public abstract decimal ApplyFineDiscount(decimal fullFine);
            public virtual bool BypassesReservationQueue => false;

            public int ActiveLoanCount => _loans.Count(l => !l.IsReturned);
            public bool CanBorrow => ActiveLoanCount < MaxActiveLoans;
            public decimal TotalFinesOwed => _loans.Sum(l => ApplyFineDiscount(l.Fine));

            internal void Attach(Loan loan) => _loans.Add(loan);
        }

        public class StandardMember : Member
        {
            public StandardMember(string membershipId, string name) : base(membershipId, name) { }
            public override int MaxActiveLoans => 3;
            public override decimal ApplyFineDiscount(decimal fullFine) => fullFine;
        }

        public class PremiumMember : Member
        {
            public PremiumMember(string membershipId, string name) : base(membershipId, name) { }
            public override int MaxActiveLoans => 10;
            public override decimal ApplyFineDiscount(decimal fullFine) => fullFine * 0.5m;
            public override bool BypassesReservationQueue => true;
        }

        public class StaffMember : Member
        {
            public StaffMember(string membershipId, string name) : base(membershipId, name) { }
            public override int MaxActiveLoans => int.MaxValue;
            public override decimal ApplyFineDiscount(decimal fullFine) => 0m;
            public override bool BypassesReservationQueue => true;
        }

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

        // =============================================================================
        // THE COORDINATOR — owns the collections, hosts the workflow, leaves every
        // rule inside the object that owns it.
        // =============================================================================
        public class Library
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

            /// <summary>Convenience overload for items that can only ever have one open loan at a time.</summary>
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
    }

    