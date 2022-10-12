using System.Collections.Generic;
using EmployeeManagement;

namespace Management.SalaryCalculation;

public interface IIncreaseSettings
{
    IReadOnlyDictionary<EmployeeType, IncreasePercent> Increases { get; }
}

public record IncreasePercent(double Yearly, double Max);
