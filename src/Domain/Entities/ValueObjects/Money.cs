using Shared.Result;

namespace Domain.Entities.ValueObjects;

public readonly record struct Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            return Result<Money>.Failure($"{nameof(currency)} cannot be null or whitespace.");
        if (amount < 0)
            return Result<Money>.Failure($"Amount must be greater than or equal zero");

        return Result<Money>.Success(new(amount, currency));
    }
}

