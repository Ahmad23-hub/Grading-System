using System;
using System.Collections.Generic;
using System.Text;
using static library_project.Program;

namespace library_project
{
    internal class ReversableLibraryItem
    {
        

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
}
}
