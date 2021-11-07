using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace SliccDB.Cypher.Collection
{
    public class VariableBuffer : IEnumerable
    {
        
        public List<Variable> Variables = new List<Variable>();

        public IEnumerator GetEnumerator()
        {
            return Variables.GetEnumerator();
        }

        public IEnumerable<object> Get(string name)
        {
            return Variables.Where(x => x.Name == name).Select(x => x.Value);
        }

        public void Add(string name, object value)
        {
            Variables.Add(new Variable()
            {
                Name = name,
                Value = value
            });
        }

        public void Clear() => Variables.Clear();
    }

    public class Variable
    {
        public string Name;
        public object Value;
        public Type GetVariableType() => Value.GetType();
    }
}