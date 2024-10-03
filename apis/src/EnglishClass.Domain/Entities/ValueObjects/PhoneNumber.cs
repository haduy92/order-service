namespace EnglishClass.Domain.Entities
{
    [Serializable]
    public readonly record struct PhoneNumber
    {
        public string CountryCode { get; }
        public string Number { get; }

        private PhoneNumber(string countryCode, string number)
        {
            CountryCode = countryCode;
            Number = number;
        }

        public static Result<PhoneNumber> Create(string countryCode, string number)
        {
            if (countryCode.All(char.IsDigit))
                return Result<PhoneNumber>.Failure("CountryCode must be digits.");
            if (number.All(char.IsDigit))
                return Result<PhoneNumber>.Failure("Number must be digits.");

            return Result<PhoneNumber>.Success(new(countryCode, number));
        }
    }
}