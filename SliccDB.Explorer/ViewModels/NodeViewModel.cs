using System.Collections.Generic;
using System.Collections.ObjectModel;
using SliccDB.Core;
using SliccDB.Explorer.Model;

namespace SliccDB.Explorer.ViewModels
{
    public class NodeViewModel
    {
        public ObservableCollection<Property> Properties { get; set; } =
            new ObservableCollection<Property>();
        public ObservableCollection<LabelModel> Labels { get; set; } =
            new ObservableCollection<LabelModel>();

        public Node ReturnNode()
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

            return new Node(dict, labels);
        }
    }
}