using System;
using System.Collections.Generic;
using System.Text;

namespace SliccDB.Exceptions;

public class SchemaValidationException : SliccDbException
{
    public SchemaValidationException(string label, Dictionary<string, string> properties)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Schema validation for {label} failed. Problematic fields: ");
        foreach (var property in properties)
        {
            sb.AppendLine($"Property: {property.Key}, Reason: {property.Value}");
        }

        Message = sb.ToString();
    }
}