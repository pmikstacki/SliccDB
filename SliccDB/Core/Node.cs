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
    public class Node : GraphEntity
    {
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
