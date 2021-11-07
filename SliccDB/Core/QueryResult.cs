using System.Collections.Generic;

namespace SliccDB.Core
{
    public class QueryResult
    {
        public HashSet<Node> Nodes { get; set; } = new HashSet<Node>();
        public HashSet<Relation> Relations { get; set; } = new HashSet<Relation>();
    }
}