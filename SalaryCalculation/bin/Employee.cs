namespace EmployeeManagement;

public class Employee
{
    public Employee(
        ulong id,
        EmployeeType type,
        DateTime entryDate,
        DateTime? exitDate,
        decimal baseRate)
    {
        Id = id;
        EntryDate = entryDate;
        ExitDate = exitDate;
        BaseRate = baseRate;
        Type = type;
    }

    public ulong Id { get; }
    public EmployeeType Type { get; }
    public DateTime EntryDate { get; }
    public DateTime? ExitDate { get; }
    public decimal BaseRate { get; }
}
