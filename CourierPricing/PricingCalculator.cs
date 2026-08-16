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
        var (type, cost) = NormalPrice(parcel);
        var heavy = HeavyPrice(parcel.Weight);
        // Cheaper of normal vs Heavy; a tie keeps the size type
        if (heavy < cost)
            return new Line("Heavy parcel", "Heavy", heavy);
        return new Line($"{type} parcel", type, cost);
    }

    private static (string Type, decimal Cost) NormalPrice(Parcel parcel)
    {
        var (type, cost, weightLimit) = Classify(parcel);
        var over = parcel.Weight - weightLimit;
        // $2 per kg over the size limit; exactly at the limit is free
        if (over > 0)
            cost += over * 2m;
        return (type, cost);
    }

    private static decimal HeavyPrice(decimal weight)
    {
        // $50 up to 50 kg, then $1 per kg over 50
        var over = weight - 50m;
        return over > 0 ? 50m + over : 50m;
    }

    private static (string Type, decimal Cost, decimal WeightLimit) Classify(Parcel parcel)
    {
        // All three sides must be strictly under the band limit
        if (AllUnder(parcel, 10)) return ("Small", 3m, 1m);
        if (AllUnder(parcel, 50)) return ("Medium", 8m, 3m);
        if (AllUnder(parcel, 100)) return ("Large", 15m, 6m);
        return ("XL", 25m, 10m);
    }

    private static bool AllUnder(Parcel parcel, int limit) =>
        parcel.Length < limit && parcel.Width < limit && parcel.Height < limit;
}
