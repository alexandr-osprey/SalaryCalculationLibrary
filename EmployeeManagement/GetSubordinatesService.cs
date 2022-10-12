using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagement;

public interface IGetSubordinatesService
{
    Task<IReadOnlyCollection<Employee>> GetSubordinatesAsync(IEnumerable<long> ids);
}

internal class GetSubordinatesService : IGetSubordinatesService
{
    private readonly IEmployeeRepository _repo;

    public GetSubordinatesService(IEmployeeRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyCollection<Employee>> GetSubordinatesAsync(IEnumerable<long> ids)
    {
        return await _repo.GetSubordinatesAsync(ids);
    }
}
