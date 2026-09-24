//Console.WriteLine("Hello, World!");
//int[] numbers = { 20, 40, 60, 80, };
//for (int i = 0; i < numbers.Length; i++)
//{
//    Console.WriteLine(numbers[i]);
//}

// string CorrectPassword = "P@ssw0rd";

// int MaxAttempts = 5;

//{
//    Console.WriteLine("Login System");

//    bool loginSuccessful = false;
//    int attemptsUsed = 0;

//    for (int attempt = 1; attempt <= MaxAttempts; attempt++)
//    {
//        attemptsUsed = attempt;
//        Console.WriteLine("Enter password: ");
//        string? guesspassword = Console.ReadLine();
//        if (guesspassword == CorrectPassword)
//        {
//            loginSuccessful = true;
//            break;
//        }
//        else
//        {
//            int remainingAttempts = MaxAttempts - attempt;

//            if (remainingAttempts > 0)
//            {
//                Console.WriteLine($"Incorrect password. " +
//                    $"You have {remainingAttempts} attempt(s) remaining.");
//            }
//        }
//    
//    Console.WriteLine();

//    if (loginSuccessful)
//    {
//        Console.WriteLine($"Login successful! Welcome, {CorrectPassword}.");
//    }
//    else
//    {
//        Console.WriteLine("You have used all 5 attempts. Your account is locked out.");
//        Console.WriteLine("Please contact support or try again later.");
//    }

//    Console.WriteLine($"Total attempts used: {attemptsUsed}/{MaxAttempts}");
//}

        //int[] scores = new int[5];
        //for (int i = 0; i < scores.Length; i++)
        //{
        //    Console.Write("Enter score for student " + (i + 1) + ": ");
        //    scores[i] = Convert.ToInt32(Console.ReadLine());
        //}

        
        //int total = 295;
        //int highest = scores[80];
        //int lowest = scores[35];
        //int passed = 3;
        //int failed = 2;
       
        //for (int i = 0; i < scores.Length; i++)
        //{
        //    total += scores[i];
        //    if (scores[i] > highest)
        //    {
        //        highest = scores[i];
        //    }
        //    if (scores[i] < lowest)
        //    {
        //        lowest = scores[i];
        //    }
        //    if (scores[i] >= 50)
        //    {
        //        passed++;
        //    }
        //    else
        //    {
        //        failed++;
        //    }
        //}

        
        //double average = (double)total / scores.Length;
        //Console.WriteLine(" STUDENT RESULTS");

        //Console.Write("Scores: ");

        //for (int i = 0; i < scores.Length; i++)
        //{
        //    Console.Write(scores[i] + " ");
        //}

        //Console.WriteLine();
        //Console.WriteLine("Total: " + total);
        //Console.WriteLine("Average: " + average);
        //Console.WriteLine("Highest: " + highest);
        //Console.WriteLine("Lowest: " + lowest);
        //Console.WriteLine("Passed: " + passed);
        //Console.WriteLine("Failed: " + failed);

        //Console.ReadLine()
using System.Text;

var today = new DateOnly(2026, 3, 2);
var library = new Library("Riverside Community Library");
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
var amina = new StandardMember("M-001", "Amina Bello");
var tunde = new StandardMember("M-002", "Tunde Okafor");
var ngozi = new PremiumMember("M-003", "Ngozi Adeyemi");
var bola = new StaffMember("S-001", "Bola (Librarian)");

library.RegisterMember(amina);
library.RegisterMember(tunde);
library.RegisterMember(ngozi);
library.RegisterMember(bola);

Console.WriteLine($"=== {library.Name} ===\n");
Console.WriteLine("-- Catalogue --");
foreach (LibraryItem item in library.Items)
Console.WriteLine($"{item.Describe()} | loan {item.LoanPeriodDays} days");
Console.WriteLine("\n-- Borrowing --");
var loan1 = library.Borrow(hobbit.Id, amina.MembershipId, today);
Console.WriteLine($"{amina.Name} borrowed \"{loan1.Item.Title}\" — due {loan1.DueOn}");
Console.WriteLine("\n-- Digital items allow concurrent loans --");
library.Borrow(hailMary.Id, amina.MembershipId, today);
library.Borrow(hailMary.Id, tunde.MembershipId, today);
Console.WriteLine($"{amina.Name} and {tunde.Name} are both \"borrowing\" \"{hailMary.Title}\" " +
                  "right now — allowed, because it is digital and never actually goes on loan.");
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
Console.WriteLine("\n-- Returning late: item sets the rate, member sets the discount --");
var fineHobbit = library.Return(hobbit.Id, amina.MembershipId, today.AddDays(25));
Console.WriteLine($"\"{hobbit.Title}\" returned 4 days late by {amina.Name} (standard member) " +
                   $"-> fine {fineHobbit:C}");

Console.WriteLine("\n-- Staff borrow without limits or fines --");
var staffLoan = library.Borrow(inception.Id, bola.MembershipId, today);
Console.WriteLine($"{bola.Name} borrowed \"{staffLoan.Item.Title}\" — " +
                   $"staff members owe {bola.TotalFinesOwed:C} no matter how late a return is.");
Console.WriteLine();
Console.WriteLine(library.DailySummaryReport(today.AddDays(30)));
static void TryIt(string what, Action action)
{
try { action(); Console.WriteLine($"{what}: allowed (!)"); }
catch (InvalidOperationException ex) { Console.WriteLine($"{what}: blocked — {ex.Message}"); }
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
public class AmnestyFinePolicy : IFinePolicy
{
    public decimal Calculate(int daysLate) => 0m;
}

public interface IReservable
{
    bool IsReserved { get; }
    IReadOnlyCollection<Member> ReservationQueue { get; }
    void Reserve(Member member);
    Member? ReleaseNextReservation();
}
public interface IDigital
{
    long FileSizeMb { get; }
    string DownloadUrl { get; }
}
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
    public virtual bool AllowsConcurrentLoans => false;

    public virtual string Describe() => $"[{Id}] {ItemType}: \"{Title}\" ({PublicationYear})";
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
    public override bool AllowsConcurrentLoans => true;
    public override void MarkAsBorrowed() { /* digital items are never exclusively "on loan" */ }
    public override void MarkAsReturned() { /* nothing to release */ }

    public override string Describe() =>
        $"{base.Describe()} — narrated by {Narrator}, {DurationMinutes} min (digital)";
}
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
    public int DaysLateAsOf(DateOnly asOf) =>
        IsReturned ? DaysLate : Math.Max(0, asOf.DayNumber - DueOn.DayNumber);
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
        if (!item.AllowsConcurrentLoans && item.IsOnLoan)
            throw new InvalidOperationException($"\"{item.Title}\" is already on loan.");
        if (item is IReservable reservable && reservable.IsReserved)
        {
            Member nextInLine = reservable.ReservationQueue.First();
            if (nextInLine.MembershipId != member.MembershipId && !member.BypassesReservationQueue)
                throw new InvalidOperationException($"\"{item.Title}\" is reserved for {nextInLine.Name}.");
        }

        item.MarkAsBorrowed();
        if (item is IReservable r && r.IsReserved && r.ReservationQueue.First().MembershipId == member.MembershipId)
            r.ReleaseNextReservation();

        var loan = new Loan(item, member, today);
        _loans.Add(loan);
        member.Attach(loan);
        return loan;
    }
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
    

