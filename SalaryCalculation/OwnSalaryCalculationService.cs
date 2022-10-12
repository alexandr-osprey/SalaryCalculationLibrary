using System;
using EmployeeManagement;
using Management.Utils;

namespace SalaryCalculation;

internal interface IOwnSalaryCalculationService
{
    decimal Calculate(Employee employee, DateTime toDate);
}

internal class OwnSalaryCalculationService : IOwnSalaryCalculationService
{
    private readonly IDateTimeProvider _dateProvider;
    private readonly IIncreaseSettings _settings;

    public OwnSalaryCalculationService(IDateTimeProvider dateProvider, IIncreaseSettings settings)
    {
        _dateProvider = dateProvider;
        _settings = settings;
    }

    public decimal Calculate(Employee employee, DateTime toDate)
    {
        var entryDate = employee.EntryDate;
        var baseRate = employee.BaseRate;
        if (toDate <= entryDate)
        {
            return 0;
        }

        if (employee.ExitDate is not null && toDate > employee.ExitDate)
        {
            return 0;
        }

        var increase = _settings.Increases[employee.Type];
        decimal maxSalary = baseRate * (1 + (decimal)increase.Max);
        double years = _dateProvider.GetYearsDiff(entryDate, toDate);
        decimal yearsSalary = baseRate;
        for (int i = 0; i < years; i++)
        {
            yearsSalary += i * (decimal)increase.Yearly * yearsSalary;
        }

        return Math.Min(maxSalary, yearsSalary);
    }
}
