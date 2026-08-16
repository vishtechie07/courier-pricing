# Courier pricing solution plan
_Exported on 8/16/2026 at 13:00:24 GMT+12 from Cursor (3.16.17)_

---

**User**

Execute a plan only for now. Do not create or edit any files yet.

I want to scaffold an empty courier pricing solution. This is the full requirement for this step:

Goal
Set up the project. No pricing logic yet.

What to create
- A solution callled CourierPricing.sln
- A Class library CourierPricing, target net8.0
- An xUnit project CourierPricing.Tests, target net8.0, referencing CourierPricing
- Add both projects to the solution
- .gitignore via: dotnet new gitignore
- README.md with only these empty headings, in this order:
  How to run
  Assumptions
  Design decisions
  Next steps
  Self-evaluation

What not to create
- No pricing / calculator code
- No CLI, website, or API
- No extra NuGet packages beyond the xUnit template

Done when
- dotnet test from the repo root exits 0 (zero tests is fine)

Please write a short plan for how you will do exactly that. Wait for my approval before implementing any file level changes or you build anything.

---

**Cursor**

I'll inspect the repo as it stands so the scaffold plan matches what's already there.

Repo is empty besides git and Cursor rules. Drafting a scaffold-only plan that matches the requirement exactly.

---

**User**

Implement the plan as specified, it is attached for your reference. Do NOT edit the plan file itself.

To-do's from the plan have already been created. Do not create them again. Mark them as in_progress as you work, starting with the first one. Don't stop until you have completed all the to-dos.

---

**Cursor**

Scaffolding the empty solution as specified. Starting with the solution and projects.

Projects are in place. Adding `.gitignore` and removing the template placeholders.

Placeholders are gone. Writing the heading-only README and verifying `dotnet test`.

Scaffold is in place. `dotnet test` from the repo root exits 0 (zero tests).

- `CourierPricing.sln` with `CourierPricing` (net8.0 classlib) and `CourierPricing.Tests` (net8.0 xUnit, references the library)
- `.gitignore` from `dotnet new gitignore`
- `README.md` with the five empty headings
- Template `Class1.cs` / `UnitTest1.cs` removed; no pricing code

Proposed commit: `Scaffold empty CourierPricing solution`

---

**User**

Goal
Price a list of parcels by their size. Return an itemised bill plus the total.

In scope
1. A parcel has length, width, height in cm, and weight in kg.
  Store the weight for now, but don't use it in the price yet.
2. Size uses all three sides:
  a. all under 10cm: Small, $3
  b. all under 50cm: Medium, $8
  c. all under 100cm: Large, $15
  d. anything else: XL, $25
3. Each parcel becomes one line: description, type, and cost.
4. The result is those lines plus a grand total.
5. More kinds of lines will come in later steps, so keep this shape and don't change it.

Out of scope
1. Speedy shipping
2. Weight fees
3. Heavy parcels
4. Discounts

Acceptance
1. 9x9x9: Small $3
2. 10x1x1: Medium $8 (10 is not "under 10")
3. 50x1x1: Large $15
4. 100x1x1: XL $25
5. one Small, one Medium, one Large, one XL: four lines, total $51
6. empty order: no lines, total $0

Done when
Tests are green. Suggest a short commit message; I will review and commit myself.

---

**Cursor**

Implementing size-based parcel pricing with TDD. Checking the current project, then writing failing tests first.

Tests fail as expected. Adding the calculator and records next.

Tests are green (6 passed).

`PricingCalculator.Price` takes parcels and returns a `Bill` of `Line`s (description, type, cost) plus total. Size is all-sides-under: Small $3, Medium $8, Large $15, XL $25. Weight is stored on `Parcel` and unused.

Proposed commit: `Price parcels by size into an itemised bill`

---

**User**

The Code style section under @.cursor/rules/workflow.mdc has an update to cover short comments. Please update the former code and tests cases where relevant aligned to the updated code style rules. Do not change any code behaviour.

---

**Cursor**

I'll read the updated style rules and the current calculator/tests, then add only the comments those rules call for.

Adding short comments only on the size-band rule and the test boundary/weight assumption. No behaviour changes.

Comments only; behaviour unchanged.

`Classify` — all three sides strictly under the band; weight unused.

Single-parcel tests — a side on the limit is not “under” that band; weight unused.

Proposed commit: `Add short comments for size-band rules`

---

**User**

Context
Speedy shipping is an optional whole-order extra pricing. Parcel line prices must stay
exactly as they are. The doubling shows up as its own line.

Goal
Let the customer opt into speedy shipping as per their wish. Double the order cost as a separate line.

In scope
1. Speedy shipping is optional per order (off until the customer chooses it).
2. Add a line called "Speedy shipping" whose cost equals the parcel subtotal before speedy.
3. Grand total becomes 2 times that subtotal.
4. Do not change the cost on existing parcel lines.

Out of scope
1. Weight fees
2. Heavy parcels
3. Discounts

Acceptance
1. Speedy off: same results as before (size pricing only, no Speedy line)
2. One Small with speedy on: parcel $3, Speedy shipping $3, total $6
3. Mixed order that was $51 with speedy on: parcel lines still $3 / $8 / $15 / $25,
     Speedy shipping $51, total $102
4. Empty order with speedy on: Ask me before deciding

Done when
Tests are green. Suggest a short commit message; I will review and commit myself.

---

**Cursor**

Empty order with speedy on is unspecified — asking that before implementing.

**Empty order + speedy shipping**

> **Empty order with speedy on: what should the bill be?**
> - Same as speedy off: no lines, total $0 (no Speedy line) ✔
> - One Speedy shipping line at $0, total $0
> - Reject / throw — speedy is not valid on an empty order

Empty + speedy matches speedy off: no lines, total $0. Writing the failing tests first.

Tests are green (9 passed).

`Price(..., speedyShipping: false)` stays the default. When on, a **Speedy shipping** line is added equal to the parcel subtotal; parcel line costs are unchanged. Empty + speedy: no lines, total $0.

Proposed commit: `Add optional speedy shipping as a separate line`

---

**User**

Context
People Are stuffing heavy goods into cheap small parcels. Each size now has a
weight limit. Overweight raises that parcel's price ($2 per kg over). Speedy still doubles the order after those fees.

Goal
Add overweight charges: $2 per kg over the limit for that size, on the parcel line.

In scope
1. Weight limits:
   a. Small: 1 kg
   b. Medium: 3 kg
   c. Large: 6 kg
   d. XL: 10 kg
2. Over the limit: add $2 times the kg over, onto that parcel's cost.
3. Exactly at the limit: no extra fee.
4. Speedy still doubles the total using the new parcel prices.
5. Do not change parcel lines when applying Speedy.

Out of scope
1. Heavy parcels
2. Discounts

Acceptance
1. Small at 1 kg: still $3
2. Small at 2 kg: $5
3. Small at 1.5 kg: $4 (half kg over = $1; ask me if you happen to read this differently)
4. That $5 Small with Speedy on: Speedy shipping $5, total $10

Done when
Tests are green. Suggest a short commit message; I will review and commit myself.

---

**Cursor**

Acceptance is clear on 1.5 kg ($1 over). Writing failing overweight tests first, then the smallest change.

Overweight tests fail as expected. Adding the $2/kg-over fee onto the parcel line.

Tests are green (16 passed).

Overweight is $2 per kg over the size limit (Small 1 / Medium 3 / Large 6 / XL 10), added on that parcel line. At the limit: no fee. Speedy still doubles the updated parcel subtotal.

Proposed commit: `Add overweight fees onto parcel line costs`

---

**User**

Context
Some parcels tend to cost too much with weight fees. Heavy is a simpler price. For every parcel, pick the lower price: normal or Heavy. Put that type on the bill. We pick Heavy for the customer; they do not choose it.

Goal
Charge each parcel the cheaper of normal vs Heavy.

In scope
1. Heavy: $50 up to 50 kg, then $1 per kg over 50.
2. Normal price means size price plus any weight fees from before.
3. Use the cheaper price. The bill line shows the type you charged.
4. If the two prices are the same, keep the size type (not Heavy).
5. Speedy still doubles using the new parcel totals.

Out of scope
1. Discounts

Acceptance
1. 10x10x10 at 30 kg: normal $62, so Heavy $50
2. Light Small 1x1x1: still Small $3
3. 51 kg: Heavy $51
4. Medium at 24 kg: both $50, keep Medium $50

Done when
Tests are green. Suggest a short commit message; I will review and commit myself.

---

**Cursor**

Writing failing Heavy-vs-normal tests first, then the cheapest-of-the-two pick.

Tests are green (20 passed).

Each parcel is charged the cheaper of normal (size + overweight) vs Heavy ($50 up to 50 kg, then $1/kg). A tie keeps the size type. Speedy still doubles the chosen parcel totals.

Proposed commit: `Charge each parcel the cheaper of normal vs Heavy`

---

**User**

Fill only the "How to run" section in the README with how to restore/test this repo (.NET 8, dotnet test). Leave the rest of the sections, I will draft the content; Avoid any code or test case changes.

---

**Cursor**

Filled **How to run** only: .NET 8, `dotnet restore`, `dotnet test` from the repo root. Other headings are unchanged.

