using System;
using System.Linq;
using SliccDB.Cypher.Mapping.Base;

namespace SliccDB.Cypher.Mapping.Maps
{
    public class BooleanMap : Map<string, bool>
    {
        public override bool Project(string src)
        {
            if (new string[] {"true", "TRUE", "True"}.Contains(src))
                return true;
            if (new string[] {"false", "False", "FALSE"}.Contains(src))
                return false;

            else throw new Exception("Boolean Literal is not recognized: " + src);

        }
    }
}