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
        var bill = _calculator.Price([new Parcel(length, width, height, 0)]);

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

    [Fact]
    public void Speedy_shipping_adds_a_line_equal_to_the_parcel_subtotal()
    {
        var bill = _calculator.Price([new Parcel(9, 9, 9, 0)], speedyShipping: true);

        Assert.Equal([3, 3], bill.Lines.Select(l => l.Cost));
        Assert.Equal("Speedy shipping", bill.Lines[1].Description);
        Assert.Equal(6, bill.Total);
    }

    [Fact]
    public void Speedy_shipping_does_not_change_parcel_line_costs()
    {
        var bill = _calculator.Price(
        [
            new Parcel(9, 9, 9, 0),
            new Parcel(10, 1, 1, 0),
            new Parcel(50, 1, 1, 0),
            new Parcel(100, 1, 1, 0)
        ], speedyShipping: true);

        Assert.Equal([3, 8, 15, 25, 51], bill.Lines.Select(l => l.Cost));
        Assert.Equal("Speedy shipping", bill.Lines[4].Description);
        Assert.Equal(102, bill.Total);
    }

    [Fact]
    public void Empty_order_with_speedy_on_has_no_lines()
    {
        // No parcels means no Speedy line either
        var bill = _calculator.Price([], speedyShipping: true);

        Assert.Empty(bill.Lines);
        Assert.Equal(0, bill.Total);
    }

    // Exactly at the size weight limit has no extra fee
    [Theory]
    [InlineData(9, 9, 9, 1, 3)]
    [InlineData(10, 1, 1, 3, 8)]
    [InlineData(50, 1, 1, 6, 15)]
    [InlineData(100, 1, 1, 10, 25)]
    public void No_overweight_fee_at_the_weight_limit(
        int length, int width, int height, double weight, int cost)
    {
        var bill = _calculator.Price([new Parcel(length, width, height, (decimal)weight)]);

        Assert.Equal(cost, Assert.Single(bill.Lines).Cost);
        Assert.Equal(cost, bill.Total);
    }

    // $2 per kg over the limit, including a half kg
    [Theory]
    [InlineData(2, 5)]
    [InlineData(1.5, 4)]
    public void Adds_overweight_fee_to_the_parcel_line(double weight, int cost)
    {
        var bill = _calculator.Price([new Parcel(9, 9, 9, (decimal)weight)]);

        Assert.Equal(cost, Assert.Single(bill.Lines).Cost);
        Assert.Equal(cost, bill.Total);
    }

    [Fact]
    public void Speedy_uses_parcel_prices_after_overweight()
    {
        var bill = _calculator.Price([new Parcel(9, 9, 9, 2)], speedyShipping: true);

        Assert.Equal([5, 5], bill.Lines.Select(l => l.Cost));
        Assert.Equal("Speedy shipping", bill.Lines[1].Description);
        Assert.Equal(10, bill.Total);
    }
}
