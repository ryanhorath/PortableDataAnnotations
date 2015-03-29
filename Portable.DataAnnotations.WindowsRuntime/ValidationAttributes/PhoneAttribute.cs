namespace System.ComponentModel.DataAnnotations
{
    using System;
    using System.ComponentModel.DataAnnotations.Resources;
    using System.Text.RegularExpressions;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class PhoneAttribute : DataTypeAttribute
    {
        // see unit tests for examples
        private static Regex _regex = new Regex(@"^(\+\s?)?((?<!\+.*)\(\+?\d+([\s\-\.]?\d+)?\)|\d+)([\s\-\.]?(\(\d+([\s\-\.]?\d+)?\)|\d+))*(\s?(x|ext\.?)\s?\d+)?$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        public PhoneAttribute()
            : base(DataType.PhoneNumber)
        {
            ErrorMessage = DataAnnotationsResources.PhoneAttribute_Invalid;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            string valueAsString = value as string;
            return valueAsString != null && _regex.Match(valueAsString).Length > 0 ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}