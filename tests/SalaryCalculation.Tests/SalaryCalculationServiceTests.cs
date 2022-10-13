using EmployeeManagement;

namespace SalaryCalculation.Tests;

public class SalaryCalculationServiceTests
{
    private readonly Mock<IEmployeeReadService> _readService;
    private readonly Mock<IOwnSalaryCalculationService> _ownSalaryService;
    private readonly Mock<IIncreaseSettings> _settings;
    private readonly SalaryCalculationService _service;

    public SalaryCalculationServiceTests()
    {
        _readService = new Mock<IEmployeeReadService>(MockBehavior.Strict);
        _ownSalaryService = new Mock<IOwnSalaryCalculationService>(MockBehavior.Strict);
        _settings = new Mock<IIncreaseSettings>(MockBehavior.Strict);
        _service = new SalaryCalculationService(_readService.Object, _ownSalaryService.Object, _settings.Object);
    }

    [Fact]
    public async Task CalculateAsync__Employee__Calculated()
    {
        // Given
        var toDate = DateTime.Now;
        var employee = new Employee(1, 10, EmployeeType.Employee, DateTime.MinValue, null, 1000);
        _ownSalaryService.Setup(f => f.Calculate(employee, toDate)).Returns(2000);

        // When
        decimal salary = await _service.CalculateAsync(employee, toDate);

        // Then
        Assert.Equal(2000, salary);
        _ownSalaryService.VerifyAll();
        _readService.VerifyAll();
        _settings.VerifyAll();
    }

    [Fact]
    public async Task CalculateAsync__Manager__Calculated()
    {
        // Given
        var toDate = DateTime.Now;
        var manager = new Employee(1, 0, EmployeeType.Manager, DateTime.MinValue, null, 3000);
        _ownSalaryService.Setup(f => f.Calculate(manager, toDate)).Returns(4000);
        var employee = new Employee(2, 1, EmployeeType.Employee, DateTime.MinValue, null, 2000);
        _ownSalaryService.Setup(f => f.Calculate(employee, toDate)).Returns(3000);
        _readService.Setup(f => f.GetSubordinatesAsync(new List<long> { 1 })).ReturnsAsync(new List<Employee> { employee });
        var increaes = new Dictionary<EmployeeType, IncreasePercent>();
        increaes.Add(EmployeeType.Manager, new IncreasePercent(0.00, 0.0, 0.01));
        _settings.Setup(f => f.Increases).Returns(increaes);

        // When
        decimal salary = await _service.CalculateAsync(manager, toDate);

        // Then
        decimal expected = 4030M;
        Assert.Equal(expected, salary);
        _ownSalaryService.VerifyAll();
        _readService.VerifyAll();
        _settings.VerifyAll();
    }

    [Fact]
    public async Task CalculateAsync__Sales__Calculated()
    {
        // Given
        var toDate = DateTime.Now;
        var sales = new Employee(1, 0, EmployeeType.Sales, DateTime.MinValue, null, 4000);
        _ownSalaryService.Setup(f => f.Calculate(sales, toDate)).Returns(5000);
        var manager = new Employee(2, 1, EmployeeType.Manager, DateTime.MinValue, null, 3000);
        _ownSalaryService.Setup(f => f.Calculate(manager, toDate)).Returns(4000);
        var employee = new Employee(3, 2, EmployeeType.Employee, DateTime.MinValue, null, 2000);
        _ownSalaryService.Setup(f => f.Calculate(employee, toDate)).Returns(3000);
        _readService.Setup(f => f.GetSubordinatesAsync(new List<long> { 1 })).ReturnsAsync(new List<Employee> { manager });
        _readService.Setup(f => f.GetSubordinatesAsync(new List<long> { 2 })).ReturnsAsync(new List<Employee> { employee });
        _readService.Setup(f => f.GetSubordinatesAsync(new List<long> { 3 })).ReturnsAsync(new List<Employee> ());
        var increaes = new Dictionary<EmployeeType, IncreasePercent>();
        increaes.Add(EmployeeType.Manager, new IncreasePercent(0.00, 0.0, 0.01));
        increaes.Add(EmployeeType.Sales, new IncreasePercent(0.00, 0.0, 0.02));
        _settings.Setup(f => f.Increases).Returns(increaes);

        // When
        decimal salary = await _service.CalculateAsync(sales, toDate);

        // Then
        decimal expected = 5140.6M;
        Assert.Equal(expected, salary);
        _ownSalaryService.VerifyAll();
        _readService.VerifyAll();
        _settings.VerifyAll();
    }
}