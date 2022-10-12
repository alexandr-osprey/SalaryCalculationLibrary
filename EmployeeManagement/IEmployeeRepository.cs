using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagement;

internal interface IEmployeeRepository
{
    Task<long> CreateEmployeeAsync(UnsavedEmployee unsaved);
    Task<Employee?> GetEmployeeAsync(long id);
    Task<IReadOnlyCollection<Employee>> GetSubordinatesAsync(IEnumerable<long> ids);
    Task UpdateEmployeeAsync(Employee employee);
    Task<IReadOnlyCollection<Employee>> GetAllEmployeesAsync(DateTime toDate, int skip, int take);
}
