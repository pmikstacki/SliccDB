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
    public class Database : IDisposable
    {
        [Key(0)]
        public HashSet<Node> Nodes { get; set; } = new HashSet<Node>();
        [Key(1)]
        public HashSet<Relation> Relations { get; set; }= new HashSet<Relation>();
        [Key(2)]
        public List<Schema> Schemas { get; set; } = new List<Schema>();
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                Nodes.Clear();
                Relations.Clear();
                Schemas.Clear();
            }
            // free native resources if there are any.
        }
    }
}
