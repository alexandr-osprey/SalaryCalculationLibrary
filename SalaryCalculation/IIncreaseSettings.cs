using System.Collections.Generic;
using EmployeeManagement;

namespace SalaryCalculation;

public interface IIncreaseSettings
{
    IReadOnlyDictionary<EmployeeType, IncreasePercent> Increases { get; }
}

public class IncreaseSettings : IIncreaseSettings
{
    public IncreaseSettings(IReadOnlyDictionary<EmployeeType, IncreasePercent> increases)
    {
        Increases = increases;
    }

    public IReadOnlyDictionary<EmployeeType, IncreasePercent> Increases { get; }
}

public record IncreasePercent(double Yearly, double Max, double Subordinates);
