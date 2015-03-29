using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    public interface IValidatableObject
    {
        IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
    }
}
