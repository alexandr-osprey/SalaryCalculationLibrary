using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagement;

internal class EmployeeRepository : IEmployeeRepository
{
    private readonly BlockingCollection<Employee> _employees;
    private long _counter;

    public EmployeeRepository()
    {
        _employees = new BlockingCollection<Employee>();
    }

    public async Task<long> CreateEmployeeAsync(UnsavedEmployee unsaved)
    {
        await Task.CompletedTask;
        long id = Interlocked.Increment(ref _counter);
        var e = new Employee(
            id,
            unsaved.BossId,
            unsaved.Type,
            unsaved.EntryDate,
            unsaved.ExitDate,
            unsaved.BaseRate);
        _employees.Add(e);
        return id;
    }

    public Task UpdateEmployeeAsync(Employee employee)
    {
        // stub method, because the instance is already up to date in the list
        return Task.CompletedTask;
    }

    public async Task<Employee?> GetEmployeeAsync(long id)
    {
        await Task.CompletedTask;
        return _employees.FirstOrDefault(e => e.Id == id);
    }

    public async Task<IReadOnlyCollection<Employee>> GetSubordinatesAsync(IEnumerable<long> ids)
    {
        await Task.CompletedTask;
        return _employees.Where(e => ids.Contains(e.BossId))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<Employee>> GetAllEmployeesAsync(DateTime toDate, int skip, int take)
    {
        await Task.CompletedTask;
        return _employees.Where(e => e.ExitDate is null || e.ExitDate < toDate)
            .Skip(skip)
            .Take(take)
            .ToArray();
    }
}
