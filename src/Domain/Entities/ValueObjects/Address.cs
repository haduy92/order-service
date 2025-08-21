namespace Domain.Entities.ValueObjects;

public readonly record struct Address(string Street, string City, string Country, string PostCode);

