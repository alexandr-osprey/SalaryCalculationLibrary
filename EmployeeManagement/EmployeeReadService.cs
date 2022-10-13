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
    private readonly ISynchronizationHelper _syncHelper;

    public EmployeeReadService(
        IEmployeeRepository repo,
        ISynchronizationHelper syncHelper)
    {
        _repo = repo;
        _syncHelper = syncHelper;
    }

    public async Task<IReadOnlyCollection<Employee>> GetSubordinatesAsync(IEnumerable<long> ids)
    {
        _syncHelper.StartRead();
        try
        {
            return await _repo.GetSubordinatesAsync(ids);

        }
        finally
        {
            _syncHelper.EndRead();
        }
    }

    public async Task<IReadOnlyCollection<Employee>> GetAllEmployeesAsync(DateTime toDate, int skip, int take)
    {
        _syncHelper.StartRead();
        try
        {
            return await _repo.GetAllEmployeesAsync(toDate, skip, take);

        }
        finally
        {
            _syncHelper.EndRead();
        }
    }
}
