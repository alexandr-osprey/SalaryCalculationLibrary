using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagement;

internal class EmployeeRepository : IEmployeeRepository
{
    private readonly ConcurrentDictionary<long, Employee> _employees;
    private long _counter;

    public EmployeeRepository()
    {
        _employees = new ConcurrentDictionary<long, Employee>();
    }

    public Task<long> CreateEmployeeAsync(UnsavedEmployee unsaved)
    {
        long id = Interlocked.Increment(ref _counter);
        var e = new Employee(
            id,
            unsaved.BossId,
            unsaved.Type,
            unsaved.EntryDate,
            unsaved.ExitDate,
            unsaved.BaseRate);
        _employees.TryAdd(id, e);
        return Task.FromResult(id);
    }

    public Task UpdateEmployeeAsync(Employee employee)
    {
        // stub method, because the instance is already up to date in the list
        return Task.CompletedTask;
    }

    public async Task<Employee?> GetEmployeeAsync(long id)
    {
        await Task.CompletedTask;
        if (_employees.TryGetValue(id, out Employee? emp))
        {
            return emp;
        }

        return null;
    }

    public async Task<IReadOnlyCollection<Employee>> GetSubordinatesAsync(IEnumerable<long> ids)
    {
        await Task.CompletedTask;
        return _employees.Where(kv => ids.Contains(kv.Value.BossId)).Select(kv => kv.Value)
            .ToArray();
    }
}
