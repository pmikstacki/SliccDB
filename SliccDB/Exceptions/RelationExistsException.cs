using System;

namespace SliccDB.Exceptions
{
    public class RelationExistsException : Exception
    {
        public override string Message {
            get;
        }

        public RelationExistsException(string targetHash, string sourceHash) : base()
        {
            Message = $"Relation with Target Hash {targetHash} and Source Hash {sourceHash} already exists!";
        }
    }
}