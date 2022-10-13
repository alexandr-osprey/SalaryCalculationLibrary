namespace EmployeeManagement.Tests;

public class EmployeeCreateServiceTests
{
    private readonly Mock<ISynchronizationHelper> _sync;
    private readonly Mock<IEmployeeRepository> _repo;
    private readonly EmployeeCreateService _service;
    public EmployeeCreateServiceTests()
    {
        _sync = new Mock<ISynchronizationHelper>(MockBehavior.Strict);
        _repo = new Mock<IEmployeeRepository>(MockBehavior.Strict);
        _service = new EmployeeCreateService(_repo.Object, _sync.Object);
    }

    [Fact]
    public async void CreateEmployeeAsync__Valid__Saved()
    {
        // Given
        var boss = new Employee(1, 0, EmployeeType.Manager, DateTime.MinValue, null, 1000);
        _repo.Setup(f => f.GetEmployeeAsync(1)).ReturnsAsync(boss);
        _sync.Setup(f => f.StartWrite());
        _sync.Setup(f => f.EndWrite());
        var unsaved = new UnsavedEmployee(1, EmployeeType.Employee, DateTime.MaxValue, null, 1000);
        _repo.Setup(f => f.CreateEmployeeAsync(unsaved)).ReturnsAsync(10);
        var saved = new Employee(10, 1, EmployeeType.Employee, DateTime.MaxValue, null, 1000);
        _repo.Setup(f => f.GetEmployeeAsync(10)).ReturnsAsync(saved);

        // When
        var actual = await _service.CreateEmployeeAsync(unsaved);

        // Then
        Assert.Equal(saved, actual);
        _sync.VerifyAll();
        _repo.VerifyAll();
    }

    [Fact]
    public async void CreateEmployeeAsync__ValidNoBoss__Saved()
    {
        // Given
        _sync.Setup(f => f.StartWrite());
        _sync.Setup(f => f.EndWrite());
        var unsaved = new UnsavedEmployee(0, EmployeeType.Manager, DateTime.MaxValue, null, 1000);
        _repo.Setup(f => f.CreateEmployeeAsync(unsaved)).ReturnsAsync(10);
        var saved = new Employee(10, 0, EmployeeType.Manager, DateTime.MaxValue, null, 1000);
        _repo.Setup(f => f.GetEmployeeAsync(10)).ReturnsAsync(saved);

        // When
        var actual = await _service.CreateEmployeeAsync(unsaved);

        // Then
        Assert.Equal(saved, actual);
        _sync.VerifyAll();
        _repo.VerifyAll();
    }

    [Fact]
    public async void CreateEmployeeAsync__InValidNoBoss__Exception()
    {
        // Given
        var unsaved = new UnsavedEmployee(0, EmployeeType.Employee, DateTime.MaxValue, null, 1000);

        // When
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateEmployeeAsync(unsaved));

        // Then
        _sync.VerifyAll();
        _repo.VerifyAll();
    }

    [Fact]
    public async void CreateEmployeeAsync__EmployeeBoss__Exception()
    {
        // Given
        var boss = new Employee(1, 0, EmployeeType.Employee, DateTime.MinValue, null, 1000);
        _repo.Setup(f => f.GetEmployeeAsync(1)).ReturnsAsync(boss);
        var unsaved = new UnsavedEmployee(1, EmployeeType.Employee, DateTime.MaxValue, null, 1000);

        // When
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateEmployeeAsync(unsaved));

        // Then
        _sync.VerifyAll();
        _repo.VerifyAll();
    }
}