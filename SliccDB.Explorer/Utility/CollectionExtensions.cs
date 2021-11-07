using System.Collections.Generic;
using System.Collections.ObjectModel;
using SliccDB.Explorer.Model;
using Syncfusion.Linq;

namespace SliccDB.Explorer.Utility
{
    public static class CollectionExtensions
    {
        public static ObservableCollection<string> ToObservableCollection(this HashSet<string> targetHashSet)
        {
            var obscol = new ObservableCollection<string>();
            targetHashSet.ForEach(x => obscol.Add(x));
            return obscol;
        } 
        public static ObservableCollection<Property> DictionaryToObservableCollection(this Dictionary<string, string> targetHashSet)
        {
            var obscol = new ObservableCollection<Property>();
            targetHashSet.ForEach(x => obscol.Add(new Property()
            {
                Name = x.Key,
                Value = x.Value
            }));
            return obscol;
        }
    }
}