using System.Collections.Generic;
using Antlr4.Runtime.Atn;

namespace SliccDB.Cypher.Extensions
{
    public static class PropertiesExtensions
    {
        public static bool HasSameKeysValues(this Dictionary<string, string> thisDictionary, Dictionary<string, string> otherDictionary)
        {
            var amountToTest = thisDictionary.Count;
            int amountTestedPositive = 0;
            foreach (var keyValuePair in thisDictionary)
            {
                if(otherDictionary.ContainsKey(keyValuePair.Key) && otherDictionary[keyValuePair.Key] == keyValuePair.Value)
                {
                    amountTestedPositive++;
                }
            }

            if (amountTestedPositive == amountToTest)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}