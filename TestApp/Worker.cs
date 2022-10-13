using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagement;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SalaryCalculation;

namespace TestApp;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IEmployeeCreateService _createService;
    private readonly IEmployeeReadService _readService;
    private readonly ISalaryCalculationService _salaryCalculationService;
    private readonly ICompanySalaryCalculationService _companyCalculationService;
    private readonly DateTime _startDate;

    public Worker(
        ILogger<Worker> logger,
        IEmployeeCreateService createService,
        IEmployeeReadService readService,
        ISalaryCalculationService salaryCalculationService,
        ICompanySalaryCalculationService companyCalculationService)
    {
        _logger = logger;
        _createService = createService;
        _readService = readService;
        _salaryCalculationService = salaryCalculationService;
        _companyCalculationService = companyCalculationService;
        string dateString = "01/01/2015 00:00:00.000000";
        string format = "dd/MM/yyyy HH:mm:ss.ffffff";
        _startDate = DateTime.ParseExact(dateString, format, null);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var lists = await FillDataAsync();
        await TestSingleCalculationAsync(lists);
        await TestMultithreading();
    }

    private async Task<EmployeeLists> FillDataAsync()
    {
        var lists = new EmployeeLists()
        {
            Managers = new List<Employee>(10),
            Sales1 = new List<Employee>(10),
            Sales2 = new List<Employee>(10),
            Employees = new List<Employee>(100),
            FiredEmployees = new List<Employee>(10),
        };

        int employeesCount = lists.Employees.Capacity;
        int managersCount = lists.Managers.Capacity;
        int salesCount = lists.Sales1.Capacity;

        for (int i = 0; i < managersCount; i++)
        {
            var empStartDate = _startDate.AddMonths(i);
            var unsaved = new UnsavedEmployee(0, EmployeeType.Manager, empStartDate, null, 5000);
            lists.Managers.Add(await _createService.CreateEmployeeAsync(unsaved));
        }

        for (int i = 0; i < salesCount; i++)
        {
            int managerIndex = i % managersCount;
            var manager = lists.Managers[managerIndex];
            var empStartDate = _startDate.AddMonths(i);
            var unsaved = new UnsavedEmployee(manager.Id, EmployeeType.Sales, empStartDate, null, 4000);
            lists.Sales1.Add(await _createService.CreateEmployeeAsync(unsaved));
        }

        for (int i = 0; i < salesCount; i++)
        {
            var salesSuperior = lists.Sales1[i];
            var empStartDate = _startDate.AddMonths(i);
            var unsaved = new UnsavedEmployee(salesSuperior.Id, EmployeeType.Sales, empStartDate, null, 3000);
            lists.Sales2.Add(await _createService.CreateEmployeeAsync(unsaved));
        }

        for (int i = 0; i < employeesCount; i++)
        {
            int salesIndex = i % salesCount;
            var superior = lists.Sales2[salesIndex];
            var empStartDate = _startDate.AddMonths(i);
            var unsaved = new UnsavedEmployee(superior.Id, EmployeeType.Employee, empStartDate, null, 2000);
            lists.Employees.Add(await _createService.CreateEmployeeAsync(unsaved));
        }

        for (int i = 0; i < lists.FiredEmployees.Capacity; i++)
        {
            int salesIndex = i % salesCount;
            var superior = lists.Sales1[salesIndex];
            var empEndDate = _startDate.AddMonths(i);
            var unsaved = new UnsavedEmployee(superior.Id, EmployeeType.Employee, _startDate, empEndDate, 2000);
            lists.FiredEmployees.Add(await _createService.CreateEmployeeAsync(unsaved));
        }

        return lists;
    }

    private async Task TestSingleCalculationAsync(EmployeeLists lists)
    {
        var toDate = _startDate.AddYears(3);
        var manager = lists.Managers[0];
        var cache = new Dictionary<long, decimal>();
        decimal managerSalary = await _salaryCalculationService.CalculateAsync(manager, toDate, cache);
        _logger.LogInformation("Manager working from {From} has a salary of {Salary}", manager.EntryDate, managerSalary);

        var sales1 = lists.Sales1[0];
        decimal sales1Salary = await _salaryCalculationService.CalculateAsync(sales1, toDate, cache);
        _logger.LogInformation("Sales 1 level working from {From} has a salary of {Salary}", sales1.EntryDate, sales1Salary);

        var sales2 = lists.Sales2[0];
        decimal sales2Salary = await _salaryCalculationService.CalculateAsync(sales2, toDate, cache);
        _logger.LogInformation("Sales 2 level working from {From} has a salary of {Salary}", sales2.EntryDate, sales2Salary);

        var employee = lists.Employees[0];
        decimal employeeSalary = await _salaryCalculationService.CalculateAsync(employee, toDate, cache);
        _logger.LogInformation("Employee working from {From} has a salary of {Salary}", employee.EntryDate, employeeSalary);

        var firedEmployee = lists.FiredEmployees[0];
        decimal firedEmployeeSalary = await _salaryCalculationService.CalculateAsync(firedEmployee, toDate, cache);
        _logger.LogInformation("FiredEmployee working from {From} has a salary of {Salary}", firedEmployee.EntryDate, firedEmployeeSalary);
    }

    private async Task TestMultithreading()
    {
        var toDate = _startDate.AddYears(3);

        decimal totalSalary = await _companyCalculationService.CalculateForAllAsync(toDate);

        int parallelCount = 100;
        Parallel.For(0, parallelCount, _ => FillDataAsync());

        decimal totalSalaryAfterParallelFill = await _companyCalculationService.CalculateForAllAsync(toDate);
        decimal expectedTotalSalary = totalSalary * (parallelCount + 1);
        if (expectedTotalSalary != totalSalaryAfterParallelFill)
        {
            throw new InvalidOperationException("Parallel tests fail");
        }

        _logger.LogInformation("Multithreading tests passed");
    }

    private class EmployeeLists
    {
        public List<Employee> Managers { get; set; }

        public List<Employee> Sales1 { get; set; }

        public List<Employee> Sales2 { get; set; }

        public List<Employee> Employees { get; set; }

        public List<Employee> FiredEmployees { get; set; }
    }
}
