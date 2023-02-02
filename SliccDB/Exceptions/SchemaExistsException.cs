using System;
using SliccDB.Core;

namespace SliccDB.Exceptions;

public class SchemaExistsException : SliccDbException
{
    public override string HelpLink { get; set; }

    public SchemaExistsException(string schemaName) : base()
    {
        Message = $"Schema for label {schemaName} already exists!";
    }

}