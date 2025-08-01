using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DNAS.Domain.HtmlRestrict
{

    public class HtmlRestrictAttribute : ValidationAttribute
    {
        // Constructor to set the error message (optional)
        public HtmlRestrictAttribute() : base("HTML content is not allowed in the Input field.")
        {
        }

        // Override the IsValid method to provide custom validation logic
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string input = value.ToString();

                // Use the ContainsHtml method to check if HTML is present
                if (ContainsHtml(input))
                {
                    // Return validation failure with error message
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }

            // Return success if no HTML is found
            return ValidationResult.Success;
        }

        // Method to check for HTML content using Regex
        private bool ContainsHtml(string input)
        {           
            return Regex.IsMatch(input, @"<[^>]+>", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(300));
        }
    }
}