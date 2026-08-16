# Courier pricing

A C# library that prices an order of parcels. No app or API; tests can call it directly.

Money uses 'decimal' and the tests are xUnit.

Implemented: Step 1 to Step 4 - size bands, weight fees, Heavy and Speedy. Discounts have been excluded (Refer Next steps).

## How to run

Requires the .NET 8 SDK.

```bash
dotnet test
```

`dotnet test` from the repo root restores, builds, and runs the xUnit tests.

## Assumptions

The brief left these open. I chose a behaviour and kept it. 
1. Empty order: no lines, total $0.
2. Empty order with Speedy: no Speedy line, total $0 (nothing to double).
3. A side equal to a band limit is not in that band. 10 cm is Medium, not Small. Same at 50 and 100.
4. Weight may be fractional ('decimal'). Sides are whole centimetres ('int').
5. No rounding. 0.5 kg over a size limit is $1. A third of a kilo over is '$3 + 2/3', not $3.67.
6. Size-band overweight stays on that parcel’s line. If Heavy wins, the Heavy price replaces the whole normal price; the $2/kg fee is not added on top.
7. Parcel lines keep input order. Speedy is calculated and tallied after.
8. No validation. Negative size or weight is not rejected.

## Design decisions

1. Output is lines plus a total, not one number, so Speedy can be its own line without rewriting parcel prices.
2. Size, weight fees, and Heavy change that parcel’s cost. Speedy does not.
3. One public entry point: `PricingCalculator.Price`. Logic stays on that class for this technical task.

## Next steps

Step 5 (discounts) is not in this submission. Parcel pricing, Heavy, and Speedy are done and tested; I would add deals on top of that rather than rework it.

The brief is three offers: 
a. 4 smalls, cheapest of the 4 is free. 
b. 3 mediums, cheapest of the 3 is free. 
c. 5 parcels of any type, cheapest of the 5 is free. 

1. One parcel, one deal. I would keep the parcel lines as they are and add minus-lines for the freebies, then apply Speedy to the discounted subtotal so the bill still doubles after deals.

2. Grouping is the actual problem. A parcel can only sit in one deal, so I would pick the combination that saves the most, not “every 3rd item in the list.” 

3. First test I would write: six mediums, three at $8 and three at $10. Grouping the three $10s together (and the three $8s together) saves $18; mixing $8 and $10 in a group saves less. I would also use the type actually charged (Heavy is not Small).

4. I would start with **brute force**: try legal valid groups, recurse on the rest, keep the max saving. Once that test is green I would replace it with a faster method and keep the $18 case so the answer cannot drift.

Separately I would reject invalid input (negative size or weight). I left that out so the pricing rules stayed the primary focus.

## Self-evaluation

1. Size, weight, Heavy, and Speedy in one calculator is fine for this test. In a real service I would split it.
2. Speedy uses the same kind of bill line as a parcel. It's "type" is "Speedy shipping" instead of something like Small or Medium. In a real API, I would treat the fee lines separately so "type" only means parcel type, not a shipping surcharge.
3. I skipped discounts so the priced rules stayed solid. The cost is there is no automated test for the $18 grouping case.
