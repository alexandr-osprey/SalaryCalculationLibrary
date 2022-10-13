using System;

namespace SalaryCalculation.Utils;

public interface IDateTimeProvider
{
    int GetYearsDiff(DateTime start, DateTime end);
}
