using System.Threading;

namespace EmployeeManagement;

public interface ISynchronizationHelper
{
    void EndRead();
    void EndWrite();
    void StartRead();
    void StartWrite();
}

public class SynchronizationHelper : ISynchronizationHelper
{
    private readonly ReaderWriterLockSlim _rwl;

    public SynchronizationHelper()
    {
        _rwl = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
    }

    public void StartRead()
    {
        _rwl.EnterReadLock();
    }

    public void EndRead()
    {
        _rwl.ExitReadLock();
    }

    public void StartWrite()
    {
        _rwl.EnterWriteLock();
    }

    public void EndWrite()
    {
        _rwl.ExitWriteLock();
    }
}
