using System;

namespace SliccDB.Exceptions
{
    public class RelationExistsException : SliccDbException
    {
        public RelationExistsException(string targetHash, string sourceHash) : base()
        {
            Message = $"Relation with Target Hash {targetHash} and Source Hash {sourceHash} already exists!";
        }
    }
}