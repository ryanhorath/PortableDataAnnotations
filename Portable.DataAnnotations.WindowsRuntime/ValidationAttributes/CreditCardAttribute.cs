using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Resources;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class CreditCardAttribute : DataTypeAttribute
    {
        public CreditCardAttribute()
            : base(DataType.Custom)
        {
            var att = new RangeAttribute(0, 100);
            ErrorMessage = DataAnnotationsResources.CreditCardAttribute_Invalid;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            string ccValue = value as string;
            if (ccValue == null)
            {
                return new ValidationResult(ErrorMessage);
            }
            ccValue = ccValue.Replace("-", "");
            ccValue = ccValue.Replace(" ", "");

            int checksum = 0;
            bool evenDigit = false;

            // http://www.beachnet.com/~hstiles/cardtype.html
            foreach (char digit in Reverse(ccValue))
            {
                if (digit < '0' || digit > '9')
                {
                    return new ValidationResult(ErrorMessage);
                }

                int digitValue = (digit - '0') * (evenDigit ? 2 : 1);
                evenDigit = !evenDigit;

                while (digitValue > 0)
                {
                    checksum += digitValue % 10;
                    digitValue /= 10;
                }
            }

            return (checksum % 10) == 0 ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }

        private static string Reverse(string input)
        {
            StringBuilder sb = new StringBuilder(input.Length);
            for (int i = input.Length - 1; i >= 0; i--)
            {
                sb.Append(input[i]);
            }
            return sb.ToString();
        }
    }
}