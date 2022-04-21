using MessagePack;
using System;
using System.Collections.Generic;

namespace SliccDB.Core
{
    /// <summary>
    /// Edge of the graph
    /// </summary>
    [MessagePackObject]
    public class Relation : GraphEntity, IComparable<Relation>, IEquatable<Relation>
    {
        [Key(3)]
        public virtual string RelationName { get; set; }
        [Key(4)]
        public virtual string SourceHash { get; set; }
        [Key(5)]
        public virtual string TargetHash { get; set; }


        public Relation()
        {
            Hash = Guid.NewGuid().ToString();
        }

        public Relation(string relationName, Dictionary<string, string> properties, HashSet<string> labels, string sourceHash, string targetHash)
        {
            Hash = Guid.NewGuid().ToString();
            RelationName = relationName;
            Properties = properties;
            Labels = labels;
            SourceHash = sourceHash;
            TargetHash = targetHash;
        }

        public int CompareTo(Relation other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var relationNameComparison = string.Compare(RelationName, other.RelationName, StringComparison.Ordinal);
            if (relationNameComparison != 0) return relationNameComparison;
            var sourceHashComparison = string.Compare(SourceHash, other.SourceHash, StringComparison.Ordinal);
            if (sourceHashComparison != 0) return sourceHashComparison;
            return string.Compare(TargetHash, other.TargetHash, StringComparison.Ordinal);
        }

        public bool Equals(Relation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return RelationName == other.RelationName && Equals(Properties, other.Properties) && Equals(Labels, other.Labels) && SourceHash == other.SourceHash && TargetHash == other.TargetHash;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Relation) obj);
        }

    }
}