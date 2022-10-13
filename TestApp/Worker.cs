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

    private readonly List<Employee> _managers;
    private readonly List<Employee> _sales1;
    private readonly List<Employee> _sales2;
    private readonly List<Employee> _employees;
    private readonly List<Employee> _firedEmployees;
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
        _managers = new List<Employee>(10);
        _sales1 = new List<Employee>(10);
        _sales2 = new List<Employee>(10);
        _employees = new List<Employee>(100);
        _firedEmployees = new List<Employee>(10);
        string dateString = "01/01/2015 00:00:00.000000";
        string format = "dd/MM/yyyy HH:mm:ss.ffffff";
        _startDate = DateTime.ParseExact(dateString, format, null);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await FillDataAsync();
        await TestSingleCalculationAsync();
        await TestCompanyCalculationAsync();
    }

    private async Task FillDataAsync()
    {
        int employeesCount = _employees.Capacity;
        int managersCount = _managers.Capacity;
        int salesCount = _sales1.Capacity;

        for (int i = 0; i < managersCount; i++)
        {
            var empStartDate = _startDate.AddMonths(i);
            var unsaved = new UnsavedEmployee(0, EmployeeType.Manager, empStartDate, null, 5000);
            _managers.Add(await _createService.CreateEmployeeAsync(unsaved));
        }

        for (int i = 0; i < salesCount; i++)
        {
            int managerIndex = i % managersCount;
            var manager = _managers[managerIndex];
            var empStartDate = _startDate.AddMonths(i);
            var unsaved = new UnsavedEmployee(manager.Id, EmployeeType.Sales, empStartDate, null, 4000);
            _sales1.Add(await _createService.CreateEmployeeAsync(unsaved));
        }

        for (int i = 0; i < salesCount; i++)
        {
            var salesSuperior = _sales1[i];
            var empStartDate = _startDate.AddMonths(i);
            var unsaved = new UnsavedEmployee(salesSuperior.Id, EmployeeType.Sales, empStartDate, null, 3000);
            _sales2.Add(await _createService.CreateEmployeeAsync(unsaved));
        }

        for (int i = 0; i < employeesCount; i++)
        {
            int salesIndex = i % salesCount;
            var superior = _sales2[salesIndex];
            var empStartDate = _startDate.AddMonths(i);
            var unsaved = new UnsavedEmployee(superior.Id, EmployeeType.Employee, empStartDate, null, 2000);
            _employees.Add(await _createService.CreateEmployeeAsync(unsaved));
        }

        for (int i = 0; i < _firedEmployees.Capacity; i++)
        {
            int salesIndex = i % salesCount;
            var superior = _sales1[salesIndex];
            var empEndDate = _startDate.AddMonths(i);
            var unsaved = new UnsavedEmployee(superior.Id, EmployeeType.Employee, _startDate, empEndDate, 2000);
            _firedEmployees.Add(await _createService.CreateEmployeeAsync(unsaved));
        }
    }

    private async Task TestSingleCalculationAsync()
    {
        var toDate = _startDate.AddYears(3);
        var manager = _managers[0];
        var cache = new Dictionary<long, decimal>();
        decimal managerSalary = await _salaryCalculationService.CalculateAsync(manager, toDate, cache);
        _logger.LogInformation("Manager working from {From} has a salary of {Salary}", manager.EntryDate, managerSalary);

        var sales1 = _sales1[0];
        decimal sales1Salary = await _salaryCalculationService.CalculateAsync(sales1, toDate, cache);
        _logger.LogInformation("Sales 1 level working from {From} has a salary of {Salary}", sales1.EntryDate, sales1Salary);

        var sales2 = _sales2[0];
        decimal sales2Salary = await _salaryCalculationService.CalculateAsync(sales2, toDate, cache);
        _logger.LogInformation("Sales 2 level working from {From} has a salary of {Salary}", sales2.EntryDate, sales2Salary);

        var employee = _employees[0];
        decimal employeeSalary = await _salaryCalculationService.CalculateAsync(employee, toDate, cache);
        _logger.LogInformation("Employee working from {From} has a salary of {Salary}", employee.EntryDate, employeeSalary);

        var firedEmployee = _firedEmployees[0];
        decimal firedEmployeeSalary = await _salaryCalculationService.CalculateAsync(firedEmployee, toDate, cache);
        _logger.LogInformation("FiredEmployee working from {From} has a salary of {Salary}", firedEmployee.EntryDate, firedEmployeeSalary);
    }

    private async Task TestCompanyCalculationAsync()
    {
        var toDate = _startDate.AddYears(3);

        decimal totalSalary = await _companyCalculationService.CalculateForAllAsync(toDate);
        _logger.LogInformation("Total comany salary to date {Date}: {Total}", toDate, totalSalary);
    }
}
