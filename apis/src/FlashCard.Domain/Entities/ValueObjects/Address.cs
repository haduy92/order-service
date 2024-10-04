namespace FlashCard.Domain.Entities
{
    public readonly record struct Address(string Street, string City, string Country, string PostCode);
}