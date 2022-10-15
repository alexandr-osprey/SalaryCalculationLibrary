using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagement;

internal class EmployeeRepository : IEmployeeRepository
{
    private readonly List<Employee> _employees;
    private long _counter;

    public EmployeeRepository()
    {
        _employees = new List<Employee>();
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

    public async Task<Employee?> GetEmployeeAsync(long id)
    {
        await Task.CompletedTask;
        return _employees.Find(e => e.Id == id);
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
        return _employees.FindAll(e => e.ExitDate is null || e.ExitDate < toDate)
            .Skip(skip)
            .Take(take)
            .ToArray();
    }
}
