using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement;

public static class EmployeeManagement
{
    public static IServiceCollection AddEmployeeManagement(this IServiceCollection sc)
    {
        sc.AddSingleton<IEmployeeRepository, EmployeeRepository>()
            .AddSingleton<IEmployeeCreateService, EmployeeCreateService>()
            .AddSingleton<IEmployeeReadService, EmployeeReadService>();

        return sc;
    }
}
