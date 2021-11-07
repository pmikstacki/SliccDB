using System.Linq;
using GraphX.Common.Models;
using Syncfusion.UI.Xaml.Diagram;
using Node = SliccDB.Core.Node;

namespace SliccDebugger.ViewModels
{
    public class NodeVisualViewModel : VertexBase
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