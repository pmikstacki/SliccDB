using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using MessagePack;

namespace SliccDB.Core
{
    /// <summary>
    /// Represents a node in a graph
    /// </summary>
    [MessagePackObject]
    public class Node
    {
        /// <summary>
        /// Hash by which the Nodes are identified
        /// </summary>
        [Key(0)]
        public virtual string Hash { get; set; }

        /// <summary>
        /// Properties contained in a node
        /// </summary>
        [Key(1)]
        public virtual Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Name of a node. Keep it consistent within a database to make querying easier
        /// </summary>
        [Key(2)]
        public virtual HashSet<string> Labels { get; set; }

        public Node()
        {
            Hash = Guid.NewGuid().ToString();
        }

        public Node(Dictionary<string, string> properties, HashSet<string> labels)
        {
            Hash = Guid.NewGuid().ToString();
            Properties = properties;
            Labels = labels;
        }

        public Node(string hash, Dictionary<string, string> properties, HashSet<string> labels)
        {
            Hash = hash;
            Properties = properties;
            Labels = labels;
        }
    }
}
