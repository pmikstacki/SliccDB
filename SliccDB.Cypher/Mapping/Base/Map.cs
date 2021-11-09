using System;

namespace SliccDB.Cypher.Mapping.Base
{
    public abstract class Map<SOURCE, TARGET> : IMap where SOURCE : class
    {
        public TARGET this[SOURCE src] => Project(src);

        public abstract TARGET Project(SOURCE src);
        public bool IsMapTarget(Type type)
        {
            return type == typeof(TARGET);
        }
    }
}