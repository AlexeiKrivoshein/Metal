using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalDAL.Helpers
{
    internal static class Helper
    {
        public static Exception GetValidationException(IEnumerable<DbEntityValidationResult> errors)
        {
            var error = new StringBuilder();

            foreach (var exception in errors)
            {
                foreach (var validation in exception.ValidationErrors)
                {
                    error.AppendLine($"{validation.PropertyName} {validation.ErrorMessage}");
                }
            }

            return new Exception(error.ToString());
        }
    }
}
