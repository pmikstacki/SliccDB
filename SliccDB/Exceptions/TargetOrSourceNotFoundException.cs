using System;

namespace SliccDB.Exceptions
{
    public class TargetOrSourceNotFoundException : SliccDbException
    {
        public TargetOrSourceNotFoundException()
        {
            Message = "<Target> or <Source> node with provided hashes doesn't exist";
        }
    }
}