using System;
using System.Collections.Generic;
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
    private readonly ISynchronizationHelper _syncHelper;

    public CompanySalaryCalculationService(
        ISalaryCalculationService calcService,
        IEmployeeReadService employeeReadService,
        ISynchronizationHelper syncHelper)
    {
        _calcService = calcService;
        _employeeReadService = employeeReadService;
        _syncHelper = syncHelper;
    }

    public async Task<decimal> CalculateForAllAsync(DateTime toDate)
    {
        int totalTaken = 0;
        const int take = 100;
        decimal totalSalary = 0;
        var cache = new Dictionary<long, decimal>();
        _syncHelper.StartRead();
        try
        {
            int taken = 0;
            do
            {
                var employees = await _employeeReadService.GetAllEmployeesAsync(toDate, totalTaken, take);
                foreach (var e in employees)
                {
                    totalSalary += await _calcService.CalculateAsync(e, toDate, cache);
                }

                taken = employees.Count;
                totalTaken += taken;
            }
            while (taken > 0);
            return totalSalary;
        }
        finally
        {
            _syncHelper.EndRead();
        }
    }
}
