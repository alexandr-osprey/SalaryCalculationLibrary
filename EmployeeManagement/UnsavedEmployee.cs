using System;

namespace EmployeeManagement;

public class UnsavedEmployee
{
    public UnsavedEmployee(
        long bossId,
        EmployeeType type,
        DateTime entryDate,
        DateTime? exitDate,
        decimal baseRate)
    {
        EntryDate = entryDate;
        ExitDate = exitDate;
        BaseRate = baseRate;
        Type = type;
        BossId = bossId;
    }

    public EmployeeType Type { get; }
    
    public long BossId { get; }

    public DateTime EntryDate { get; }

    public DateTime? ExitDate { get; }
    
    public decimal BaseRate { get; }
}
