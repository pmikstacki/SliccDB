using System;
using SliccDB.Core;

namespace SliccDB.Exceptions;

public class SchemaExistsException : Exception
{
    public override string Message { get; }
    public override string HelpLink { get; set; }

    public SchemaExistsException(string schemaName) : base()
    {
        Message = $"Schema for label {schemaName} already exists!";
    }

}