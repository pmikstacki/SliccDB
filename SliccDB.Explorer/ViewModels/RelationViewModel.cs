using System.Collections.Generic;
using System.Collections.ObjectModel;
using SliccDB.Core;
using SliccDB.Explorer.Model;

namespace SliccDB.Explorer.ViewModels
{
    public class RelationViewModel
    {

        public string RelationName { get; set; }
        public ObservableCollection<Property> Properties { get; set; } =
            new ObservableCollection<Property>();
        public ObservableCollection<LabelModel> Labels { get; set; } =
            new ObservableCollection<LabelModel>();
        public virtual string SourceHash { get; set; }
        public virtual string TargetHash { get; set; }
        public override string ToString()
        {
            return RelationName;
        }

        public Relation ReturnRelation()
        {
            var dict = new Dictionary<string, string>();
            var labels = new HashSet<string>();

            foreach (var keyValuePair in Properties)
            {
                dict.Add(keyValuePair.Name, keyValuePair.Value);
            }

            foreach (var label in Labels)
            {
                labels.Add(label.Label);
            }

            return new Relation(RelationName, dict, labels, SourceHash, TargetHash);
        }
    }
}
