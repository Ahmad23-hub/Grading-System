using System;
using System.Collections.Generic;
using System.Text;
using static library_project.Program;

namespace library_project
{
    internal class IReversable
    {
       
public interface IReservable
    {
        bool IsReserved { get; }
        IReadOnlyCollection<Member> ReservationQueue { get; }
        void Reserve(Member member);
        Member? ReleaseNextReservation();
    }
}
}
