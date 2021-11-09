using System;

namespace SliccDB.Cypher.Mapping.Base
{
    public interface IMap
    {
        bool IsMapTarget(Type type);
    }
}