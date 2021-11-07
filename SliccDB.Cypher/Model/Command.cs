using System.Collections.Generic;
using System.Linq;

namespace SliccDB.Cypher.Model
{
    public abstract class Command
    {
        public Dictionary<string, object> Parameters = new Dictionary<string, object>();
        public abstract void Execute();

        public override string ToString()
        {
            return
                $"Command: {GetType().Name} \n Parameters: {string.Join("\n", Parameters.Select(a => $"Name: {a.Key}, Value: {a.Value.ToString()}"))}";
        }
    }
}