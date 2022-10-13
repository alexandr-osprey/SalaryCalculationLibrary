using SalaryCalculation.Utils;

namespace SalaryCalculation.Tests;

public class DateTimeProviderTests
{
    [Fact]
    public void GetYearsDiff__MoreThan2Years_2Years()
    {
        // Arrange
        var provider = new DateTimeProvider();
        var end = DateTime.Now;
        var start = end.AddYears(-2).AddDays(-1);

        // Act
        int years = provider.GetYearsDiff(start, end);

        // Assert
        Assert.Equal(2, years);
    }

    [Fact]
    public void GetYearsDiff__Exactly2Years_2Years()
    {
        // Arrange
        var provider = new DateTimeProvider();
        var end = DateTime.Now;
        var start = end.AddYears(-2);

        // Act
        int years = provider.GetYearsDiff(start, end);

        // Assert
        Assert.Equal(2, years);
    }

    [Fact]
    public void GetYearsDiff__LessThan3Years_2Years()
    {
        // Arrange
        var provider = new DateTimeProvider();
        var end = DateTime.Now;
        var start = end.AddYears(-3).AddDays(1);

        // Act
        int years = provider.GetYearsDiff(start, end);

        // Assert
        Assert.Equal(2, years);
    }
}