using System.ComponentModel.DataAnnotations;

namespace Delez.Api.Dto;

[AttributeUsage(AttributeTargets.Property)]
public class VeljavenZnesekAttribute : ValidationAttribute
{
    private const decimal Najvecji = 99_999_999.99m;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not decimal znesek)
        {
            return ValidationResult.Success;
        }

        var sporocilo = znesek switch
        {
            <= 0 => "Znesek mora biti večji od 0,00 €",
            _ when decimal.Round(znesek, 2) != znesek => "Znesek ima lahko največ dve decimalki",
            > Najvecji => "Znesek je lahko največ 99.999.999,99 €",
            _ => null
        };

        return sporocilo is null
            ? ValidationResult.Success
            : new ValidationResult(sporocilo, validationContext.MemberName is { } ime ? [ime] : null);
    }
}
