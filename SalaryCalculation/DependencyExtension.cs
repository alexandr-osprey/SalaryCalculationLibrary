using System.Collections.Generic;
using EmployeeManagement;
using Microsoft.Extensions.DependencyInjection;

namespace SalaryCalculation;

public static class DependencyExtension
{
    public static IServiceCollection AddSalaryCalculation(this IServiceCollection sc)
    {
        var increases = new Dictionary<EmployeeType, IncreasePercent>();
        increases.Add(EmployeeType.Employee, new IncreasePercent(0.03, 0.30));
        increases.Add(EmployeeType.Manager, new IncreasePercent(0.05, 0.40));
        increases.Add(EmployeeType.Employee, new IncreasePercent(0.01, 0.35));
        sc.AddSingleton<IIncreaseSettings>(new IncreaseSettings(increases))
            .AddSingleton<IOwnSalaryCalculationService, OwnSalaryCalculationService>()
            .AddSingleton<ISalaryCalculationService, SalaryCalculationService>();
        
        return sc;
    }
}
