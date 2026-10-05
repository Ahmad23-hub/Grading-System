using System;
using System.Collections.Generic;
using System.Text;

namespace library_project
{


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
    }
