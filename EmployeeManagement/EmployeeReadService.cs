using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagement;

public interface IEmployeeReadService
{
    Task<IReadOnlyCollection<Employee>> GetSubordinatesAsync(IEnumerable<long> ids);
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
}
