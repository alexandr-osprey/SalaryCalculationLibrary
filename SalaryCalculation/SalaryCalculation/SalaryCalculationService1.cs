using System;
using System.Threading.Tasks;
using EmployeeManagement;

namespace Management.SalaryCalculation;

public class SalaryCalculationService1
{
    private readonly EmployeeRepository _repo;

    public SalaryCalculationService(EmployeeRepository repo)
    {
        _repo = repo;
    }

    public async Task<decimal> CalculateSalaryAsync(long id, DateTime toDate)
    {
        var employee = await _repo.GetEmployeeAsync(id)
            ?? throw new ArgumentException("Employee not found");
        
        if (employee.)
    }
}
