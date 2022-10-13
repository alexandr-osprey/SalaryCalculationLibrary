using System.Collections.Generic;
using EmployeeManagement;
using SalaryCalculation.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace SalaryCalculation;

public static class DependencyExtension
{
    public static IServiceCollection AddSalaryCalculation(this IServiceCollection sc)
    {
        var increases = new Dictionary<EmployeeType, IncreasePercent>();
        increases.Add(EmployeeType.Employee, new IncreasePercent(0.03, 0.30, 0));
        increases.Add(EmployeeType.Manager, new IncreasePercent(0.05, 0.40, 0.005));
        increases.Add(EmployeeType.Sales, new IncreasePercent(0.01, 0.35, 0.003));
        sc.AddSingleton<IIncreaseSettings>(new IncreaseSettings(increases))
            .AddSingleton<IOwnSalaryCalculationService, OwnSalaryCalculationService>()
            .AddSingleton<ISalaryCalculationService, SalaryCalculationService>()
            .AddSingleton<ICompanySalaryCalculationService, CompanySalaryCalculationService>()
            .AddSingleton<IDateTimeProvider, DateTimeProvider>();
        
        return sc;
    }
}
