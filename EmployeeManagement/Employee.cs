using System;

namespace EmployeeManagement;

public class Employee
{
    internal Employee(
        long id,
        long bossId,
        EmployeeType type,
        DateTime entryDate,
        DateTime? exitDate,
        decimal baseRate)
    {
        Id = id;
        BossId = bossId;
        EntryDate = entryDate;
        ExitDate = exitDate;
        BaseRate = baseRate;
        Type = type;
    }

    public long Id { get; }

    public long BossId { get; }

    public EmployeeType Type { get; }

    public DateTime EntryDate { get; }

    public DateTime? ExitDate { get; }
    
    public decimal BaseRate { get; }
}
