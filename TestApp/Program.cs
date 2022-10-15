using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EmployeeManagement;
using SalaryCalculation;

namespace TestApp;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddHostedService<Worker>()
                    .AddEmployeeManagement()
                    .AddSalaryCalculation();
            })
            .Build();

        await host.RunAsync();
    }
}