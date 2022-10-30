using System.Linq;
using Node = SliccDB.Core.Node;

namespace SliccDB.Explorer.ViewModels
{
    public class NodeVisualViewModel
    {
        public string Hash { get; set; }
        public string Labels { get; set; }

        public NodeVisualViewModel(Node node)
        {
            this.Hash = node.Hash;
            Labels = string.Join(",", node.Labels.ToArray());
        }

        public override string ToString()
        {
            return Labels;
        }
    }
}