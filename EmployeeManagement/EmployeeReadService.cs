using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagement;

public interface IEmployeeReadService
{
    Task<IReadOnlyCollection<Employee>> GetSubordinatesAsync(IEnumerable<long> ids);

    Task<IReadOnlyCollection<Employee>> GetAllEmployeesAsync(DateTime toDate, int skip, int take);
}

internal class EmployeeReadService : IEmployeeReadService
{
    private readonly IEmployeeRepository _repo;

    public EmployeeReadService(IEmployeeRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyCollection<Employee>> GetSubordinatesAsync(IEnumerable<long> ids)
    {
        return await _repo.GetSubordinatesAsync(ids);
    }

    public async Task<IReadOnlyCollection<Employee>> GetAllEmployeesAsync(DateTime toDate, int skip, int take)
    {
        return await _repo.GetAllEmployeesAsync(toDate, skip, take);
    }
}
