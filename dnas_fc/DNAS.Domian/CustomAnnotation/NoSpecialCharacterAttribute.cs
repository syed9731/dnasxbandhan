using DNAS.Domian.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DNAS.Domain.NoSpecialCharacter;

public class NoSpecialCharacterAttribute : ValidationAttribute, IClientModelValidator
{
    // Override the IsValid method to provide custom validation logic
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        //Get the AppConfig settings from DI
        var options = validationContext.GetService<IOptions<AppConfig>>();

        if (value != null)
        {
            string input = value.ToString()!;

            // Use the ContainsHtml method to check if HTML is present
            if (ContainsSpecialCharacter(input, options))
            {
                // Return validation failure with error message
                // var rrr= new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                // return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

                string errorMessage = ErrorMessage ??
                                      $"{validationContext.DisplayName} contains special characters!";

                return new ValidationResult(errorMessage);
            }
        }

        // Return success if no HTML is found
        return ValidationResult.Success!;
    }


    public void AddValidation(ClientModelValidationContext context)
    {
        string propertyName = context.ModelMetadata.DisplayName ?? context.ModelMetadata.Name;
        context.Attributes.Add("data-val", "true");
        context.Attributes.Add("data-val-nospecialcharacter", ErrorMessage ?? $"{propertyName} contains special characters!");
    }



    // Method to check for HTML content using Regex
    private static bool ContainsSpecialCharacter(string input, IOptions<AppConfig> options)
    {
        string specialCharPattern = options?.Value?.AllowedCharacter ?? @"[^a-zA-Z0-9@';.]";
        return Regex.IsMatch(input, specialCharPattern, RegexOptions.IgnoreCase,
            TimeSpan.FromMilliseconds(300));
    }
}