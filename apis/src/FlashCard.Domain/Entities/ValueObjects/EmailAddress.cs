using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Serialization;
using FlashCard.Common.Extensions;
using FlashCard.Common.Result;

namespace FlashCard.Domain.Entities
{
    public readonly record struct EmailAddress
    {
        public string Email { get; }

        private EmailAddress(string emailAddress)
        {
            Email = emailAddress;
        }

        public static Result<EmailAddress> Create(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                return Result<EmailAddress>.Failure("EmailAddress cannot be null or whitespace.");
            }

            var valid = true;
            try
            {
                var mailAddress = new MailAddress(emailAddress);
            }
            catch
            {
                valid = false;
            }
            if (!valid)
            {
                return Result<EmailAddress>.Failure("EmailAddress '{0}' is invalid.", emailAddress);
            }

            return Result<EmailAddress>.Success(new(emailAddress));
        }
    }
    public class EmailAddressConverter : JsonConverter<EmailAddress>
    {
        public override EmailAddress Read(ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var address = reader.GetString();
            if (address.IsNullOrEmpty())
            {
                return default;
            }

            return EmailAddress.Create(address!).Value;
        }
        public override void Write(Utf8JsonWriter writer,
            EmailAddress value,
            JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Email);
        }
    }
}