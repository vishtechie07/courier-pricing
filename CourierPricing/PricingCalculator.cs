namespace CourierPricing;

public record Parcel(int Length, int Width, int Height, decimal Weight);

public record Line(string Description, string Type, decimal Cost);

public record Bill(IReadOnlyList<Line> Lines, decimal Total);

public class PricingCalculator
{
    public Bill Price(IReadOnlyList<Parcel> parcels, bool speedyShipping = false)
    {
        var lines = parcels.Select(ToLine).ToList();
        var subtotal = lines.Sum(l => l.Cost);
        // Speedy doubles the order via its own line; skip when there are no parcels
        if (speedyShipping && lines.Count > 0)
            lines.Add(new Line("Speedy shipping", "Speedy shipping", subtotal));
        return new Bill(lines, lines.Sum(l => l.Cost));
    }

    private static Line ToLine(Parcel parcel)
    {
        var (type, cost) = Classify(parcel);
        return new Line($"{type} parcel", type, cost);
    }

    private static (string Type, decimal Cost) Classify(Parcel parcel)
    {
        // All three sides must be strictly under the band limit; weight is unused
        if (AllUnder(parcel, 10)) return ("Small", 3m);
        if (AllUnder(parcel, 50)) return ("Medium", 8m);
        if (AllUnder(parcel, 100)) return ("Large", 15m);
        return ("XL", 25m);
    }

    private static bool AllUnder(Parcel parcel, int limit) =>
        parcel.Length < limit && parcel.Width < limit && parcel.Height < limit;
}
