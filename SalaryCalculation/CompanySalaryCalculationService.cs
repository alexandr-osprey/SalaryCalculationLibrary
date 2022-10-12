using System;
using System.Threading.Tasks;
using EmployeeManagement;

namespace SalaryCalculation;

public interface ICompanySalaryCalculationService
{
    Task<decimal> CalculateForAllAsync(DateTime toDate);
}

internal class CompanySalaryCalculationService : ICompanySalaryCalculationService
{
    private readonly ISalaryCalculationService _calcService;
    private readonly IEmployeeReadService _employeeReadService;

    public CompanySalaryCalculationService(
        ISalaryCalculationService calcService,
        IEmployeeReadService employeeReadService)
    {
        _calcService = calcService;
        _employeeReadService = employeeReadService;
    }

    public async Task<decimal> CalculateForAllAsync(DateTime toDate)
    {
        int taken = 0;
        const int take = 100;
        decimal totalSalary = 0;
        do
        {
            var employees = await _employeeReadService.GetAllEmployeesAsync(toDate, taken, take);
            foreach (var e in employees)
            {
                totalSalary += await _calcService.CalculateAsync(e, toDate);
            }

            taken += employees.Count;
        }
        while (taken > 0);

        return totalSalary;
    }
}
