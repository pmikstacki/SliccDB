using System;

namespace SliccDB.Exceptions
{
    public class TargetOrSourceNotFoundException : Exception
    {
        public override string Message => "<Target> or <Source> node with provided hashes doesn't exist";
    }
}