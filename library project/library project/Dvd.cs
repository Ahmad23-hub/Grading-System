using System;
using System.Collections.Generic;
using System.Text;

namespace library_project
{
    internal class Dvd
    {
        

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
}
}
