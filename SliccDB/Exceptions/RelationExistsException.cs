using System;

namespace SliccDB.Exceptions
{
    public class RelationExistsException : Exception
    {
        public override string Message => "Relation already exists";
    }
}