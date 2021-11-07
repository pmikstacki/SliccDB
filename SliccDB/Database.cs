using System;
using System.Collections.Generic;
using System.IO;
using MessagePack;
using SliccDB.Core;
using MessagePack;

namespace SliccDB
{
    /// <summary>
    /// A graph Database
    /// </summary>
    [MessagePackObject]
    public class Database
    {
        [Key(0)]
        public HashSet<Node> Nodes { get; set; } = new HashSet<Node>();
        [Key(1)]
        public HashSet<Relation> Relations { get; set; }= new HashSet<Relation>();

      
    }
}
