using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement;

namespace SalaryCalculation;

public interface ISalaryCalculationService
{
    Task<decimal> CalculateAsync(Employee employee, DateTime toDate, IDictionary<long, decimal>? cache = null);
}

internal class SalaryCalculationService : ISalaryCalculationService
{
    private readonly IEmployeeReadService _subordinatesService;
    private readonly IOwnSalaryCalculationService _ownSalaryService;
    private readonly IIncreaseSettings _increaseSettings;

    public SalaryCalculationService(
        IEmployeeReadService repo,
        IOwnSalaryCalculationService ownSalaryService,
        IIncreaseSettings increaseSettings)
    {
        _subordinatesService = repo;
        _ownSalaryService = ownSalaryService;
        _increaseSettings = increaseSettings;
    }

    public async Task<decimal> CalculateAsync(Employee employee, DateTime toDate, IDictionary<long, decimal>? cache = null)
    {
        long id = employee.Id;
        if (cache?.TryGetValue(id, out decimal cached) ?? false)
        {
            return cached;
        }

        if (employee.ExitDate is not null && toDate > employee.ExitDate)
        {
            cache?.Add(id, 0);
            return 0;
        }

        decimal ownSalary = _ownSalaryService.Calculate(employee, toDate);
        if (employee.Type == EmployeeType.Employee)
        {
            cache?.Add(id, ownSalary);
            return ownSalary;
        }

        if (employee.Type == EmployeeType.Manager)
        {
            var subordinates = await _subordinatesService.GetSubordinatesAsync(new[] { employee.Id });
            decimal subordinatesSalarySum = 0;
            foreach (var subordinate in subordinates)
            {
                subordinatesSalarySum += await CalculateAsync(subordinate, toDate, cache);
            }

            decimal subPercent = (decimal)_increaseSettings.Increases[EmployeeType.Manager].Subordinates;
            decimal totalSalary = ownSalary + (subordinatesSalarySum * subPercent);
            cache?.Add(id, totalSalary);
            return totalSalary;
        }

        if (employee.Type == EmployeeType.Sales)
        {
            var employeeIds = new List<long>();
            employeeIds.Add(employee.Id);
            decimal subordinatesSalarySum = 0;
            do
            {
                var subordinates = await _subordinatesService.GetSubordinatesAsync(employeeIds);
                foreach (var subordinate in subordinates)
                {
                    subordinatesSalarySum += await CalculateAsync(subordinate, toDate, cache);
                }

                employeeIds.Clear();
                employeeIds.AddRange(subordinates.Select(s => s.Id));
            }
            while (employeeIds.Count != 0);

            decimal subPercent = (decimal)_increaseSettings.Increases[EmployeeType.Sales].Subordinates;
            decimal totalSalary = ownSalary + (subordinatesSalarySum * subPercent);
            cache?.Add(id, totalSalary);
            return totalSalary;
        }

        throw new ArgumentException($"Unsupported employee type {employee.Type}");
    }
}
