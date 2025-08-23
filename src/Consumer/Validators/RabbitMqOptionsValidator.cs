using Consumer.Configuration;
using Microsoft.Extensions.Options;

namespace Consumer.Validators;

/// <summary>
/// Validator for RabbitMQ configuration options
/// Follows the SOLID principles by having a single responsibility
/// </summary>
public class RabbitMqOptionsValidator : IValidateOptions<RabbitMqOptions>
{
    public ValidateOptionsResult Validate(string? name, RabbitMqOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.HostName))
        {
            failures.Add("RabbitMQ HostName is required and cannot be empty");
        }

        if (options.Port <= 0 || options.Port > 65535)
        {
            failures.Add("RabbitMQ Port must be between 1 and 65535");
        }

        if (string.IsNullOrWhiteSpace(options.UserName))
        {
            failures.Add("RabbitMQ UserName is required and cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(options.Password))
        {
            failures.Add("RabbitMQ Password is required and cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(options.Exchange))
        {
            failures.Add("RabbitMQ Exchange name is required and cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(options.ExchangeType))
        {
            failures.Add("RabbitMQ ExchangeType is required and cannot be empty");
        }

        if (failures.Any())
        {
            return ValidateOptionsResult.Fail(failures);
        }

        return ValidateOptionsResult.Success;
    }
}
