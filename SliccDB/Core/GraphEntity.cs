using System.Collections.Generic;
using MessagePack;

namespace SliccDB.Core
{
    [MessagePack.Union(0, typeof(Node))]
    [MessagePack.Union(1, typeof(Relation))]
    public abstract class GraphEntity
    {
        /// <summary>
        /// Properties contained in an entity
        /// </summary>
        [Key(0)]
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// Labels contained in an entity
        /// </summary>
        [Key(1)]

        public HashSet<string> Labels { get; set; } = new HashSet<string>();

        [Key(2)]
        public virtual string Hash { get; set; }
    }
}