namespace CourierPricing.Tests;

public class PricingCalculatorTests
{
    private readonly PricingCalculator _calculator = new();

    // A side equal to a band limit is not "under" that band
    [Theory]
    [InlineData(9, 9, 9, "Small", 3)]
    [InlineData(10, 1, 1, "Medium", 8)]
    [InlineData(50, 1, 1, "Large", 15)]
    [InlineData(100, 1, 1, "XL", 25)]
    public void Prices_a_single_parcel_by_size(
        int length, int width, int height, string type, int cost)
    {
        var bill = _calculator.Price([new Parcel(length, width, height, 0)]); // weight unused

        var line = Assert.Single(bill.Lines);
        Assert.Equal(type, line.Type);
        Assert.Equal(cost, line.Cost);
        Assert.Equal(cost, bill.Total);
    }

    [Fact]
    public void Prices_a_mixed_order()
    {
        var bill = _calculator.Price(
        [
            new Parcel(9, 9, 9, 0),
            new Parcel(10, 1, 1, 0),
            new Parcel(50, 1, 1, 0),
            new Parcel(100, 1, 1, 0)
        ]);

        Assert.Equal(["Small", "Medium", "Large", "XL"], bill.Lines.Select(l => l.Type));
        Assert.Equal([3, 8, 15, 25], bill.Lines.Select(l => l.Cost));
        Assert.Equal(51, bill.Total);
    }

    [Fact]
    public void Empty_order_has_no_lines_and_zero_total()
    {
        var bill = _calculator.Price([]);

        Assert.Empty(bill.Lines);
        Assert.Equal(0, bill.Total);
    }
}
