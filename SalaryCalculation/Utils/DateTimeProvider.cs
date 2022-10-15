using System;

namespace SalaryCalculation.Utils;

public class DateTimeProvider : IDateTimeProvider
{
    public int GetYearsDiff(DateTime start, DateTime end)
    {
        return (end.Year - start.Year - 1) +
            (((end.Month > start.Month) ||
            ((end.Month == start.Month) && (end.Day >= start.Day))) ? 1 : 0);
    }
}