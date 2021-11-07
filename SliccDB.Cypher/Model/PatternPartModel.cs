using System.Collections.Generic;

namespace SliccDB.Cypher.Model
{
    public class PatternPartModel
    {
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public HashSet<string> Labels { get; set; } = new HashSet<string>();
        public string VariableName { get; set; } 
    }
}