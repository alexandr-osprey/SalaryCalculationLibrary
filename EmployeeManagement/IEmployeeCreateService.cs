using System.Threading.Tasks;

namespace EmployeeManagement;

public interface IEmployeeCreateService
{
    Task<Employee> CreateEmployeeAsync(UnsavedEmployee unsaved);
}
