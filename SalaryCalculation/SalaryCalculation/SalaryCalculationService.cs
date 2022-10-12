using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement;

namespace Management.SalaryCalculation;

internal class SalaryCalculationService
{
    private readonly EmployeeRepository _repo;
    private readonly OwnSalaryCalculationService _ownSalaryService;

    public SalaryCalculationService(EmployeeRepository repo, OwnSalaryCalculationService ownSalaryService)
    {
        _repo = repo;
        _ownSalaryService = ownSalaryService;
    }

    public async Task<decimal> CalculateAsync(Employee employee, DateTime toDate)
    {
        decimal ownSalary = _ownSalaryService.Calculate(employee, toDate);
        if (employee.Type == EmployeeType.Employee)
        {
            return ownSalary;
        }

        if (employee.Type == EmployeeType.Manager)
        {
            var subordinates = await _repo.GetSubordinatesAsync(new [] { employee.Id });
            decimal subordinatesSalarySum = 0;
            foreach (var subordinate in subordinates)
            {
                subordinatesSalarySum += await CalculateAsync(subordinate, toDate);
            }

            return ownSalary + subordinatesSalarySum;
        }

        if (employee.Type == EmployeeType.Sales)
        {
            var employeeIds = new List<long>();
            employeeIds.Add(employee.Id);
            decimal subordinatesSalarySum = 0;
            do
            {
                var subordinates = await _repo.GetSubordinatesAsync(employeeIds);
                foreach (var subordinate in subordinates)
                {
                    subordinatesSalarySum += await CalculateAsync(subordinate, toDate);
                }

                employeeIds.AddRange(subordinates.Select(s => s.Id));
            }
            while (employeeIds.Count != 0);

            return ownSalary + subordinatesSalarySum;
        }

        throw new ArgumentException($"Unsupported employee type {employee.Type}");
    }
}
