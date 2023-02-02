using System;

namespace SliccDB.Exceptions;

public class SliccDbException : Exception
{
    public string Message { get; protected set; }

}