using System;

namespace Management.Utils;

public interface IDateTimeProvider
{
    DateTime GetNow();

    int GetYearsDiff(DateTime start, DateTime end);
}
