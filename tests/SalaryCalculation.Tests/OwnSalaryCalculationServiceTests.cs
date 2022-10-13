using EmployeeManagement;
using SalaryCalculation.Utils;

namespace SalaryCalculation.Tests;

public class OwnSalaryCalculationServiceTests
{
    private readonly Mock<IDateTimeProvider> _dateProvider;
    private readonly Mock<IIncreaseSettings> _settings;
    private readonly OwnSalaryCalculationService _service;

    public OwnSalaryCalculationServiceTests()
    {
        _dateProvider = new Mock<IDateTimeProvider>(MockBehavior.Strict);
        _settings = new Mock<IIncreaseSettings>(MockBehavior.Strict);
        _service = new OwnSalaryCalculationService(_dateProvider.Object, _settings.Object);
    }

    [Fact]
    public void Calculate__WorkingEmployeeSalary__Calculated()
    {
        // Arrange
        var toDate = DateTime.Now;
        var employee = new Employee(1, 2, EmployeeType.Employee, toDate.AddYears(-3), null, 1000);
        _dateProvider.Setup(f => f.GetYearsDiff(employee.EntryDate, toDate)).Returns(3);
        var increaes = new Dictionary<EmployeeType, IncreasePercent>();
        increaes.Add(EmployeeType.Employee, new IncreasePercent(0.05, 0.20, 0));
        _settings.Setup(f => f.Increases).Returns(increaes);

        // Act
        decimal salary = _service.Calculate(employee, toDate);
        decimal expected = 1157.625M;

        // Assert
        Assert.Equal(expected, salary);
        _dateProvider.VerifyAll();
        _settings.VerifyAll();
    }

    [Fact]
    public void Calculate__ToDateLessThanEntryDate__Zero()
    {
        // Arrange
        var toDate = DateTime.Now.AddYears(-4);
        var employee = new Employee(1, 2, EmployeeType.Employee, toDate.AddYears(-3), null, 1000);

        // Act
        decimal salary = _service.Calculate(employee, toDate);

        // Assert
        Assert.Equal(0M, salary);
        _dateProvider.VerifyAll();
        _settings.VerifyAll();
    }

    [Fact]
    public void Calculate__WorkingEmployeeSalaryMoreThanMax__MaxReturned()
    {
        // Arrange
        var toDate = DateTime.Now;
        var employee = new Employee(1, 2, EmployeeType.Employee, toDate.AddYears(-3), null, 1000);
        _dateProvider.Setup(f => f.GetYearsDiff(employee.EntryDate, toDate)).Returns(3);
        var increaes = new Dictionary<EmployeeType, IncreasePercent>();
        increaes.Add(EmployeeType.Employee, new IncreasePercent(0.1, 0.20, 0));
        _settings.Setup(f => f.Increases).Returns(increaes);

        // Act
        decimal salary = _service.Calculate(employee, toDate);
        decimal expected = 1200M;
        
        // Assert
        Assert.Equal(expected, salary);
        _dateProvider.VerifyAll();
        _settings.VerifyAll();
    }
}