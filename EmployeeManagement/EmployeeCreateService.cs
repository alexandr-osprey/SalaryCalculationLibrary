using System;
using System.Threading.Tasks;

namespace EmployeeManagement;

internal class EmployeeCreateService : IEmployeeCreateService
{
    private readonly IEmployeeRepository _repo;
    private readonly ISynchronizationHelper _syncHelper;

    public EmployeeCreateService(IEmployeeRepository repo, ISynchronizationHelper syncHelper)
    {
        _repo = repo;
        _syncHelper = syncHelper;
    }

    public async Task<Employee> CreateEmployeeAsync(UnsavedEmployee unsaved)
    {
        if (unsaved.BossId == 0 && unsaved.Type != EmployeeType.Manager)
        {
            throw new ArgumentException($"{unsaved.Type} should have a boss");
        }

        if (unsaved.BossId != 0)
        {
            var boss = await _repo.GetEmployeeAsync(unsaved.BossId)
                ?? throw new ArgumentException($"Boss {unsaved.BossId} does not exist");
            if (boss.Type == EmployeeType.Employee)
            {
                throw new ArgumentException("Employee can't have subordinates");
            }
        }

        _syncHelper.StartWrite();
        try
        {

            long id = await _repo.CreateEmployeeAsync(unsaved);
            return await _repo.GetEmployeeAsync(id)
                ?? throw new InvalidOperationException("Employee not created");
        }
        finally
        {
            _syncHelper.EndWrite();
        }
    }
}
