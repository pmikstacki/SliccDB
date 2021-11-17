using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Antlr4.Runtime.Tree;
using Microsoft.Msagl.Drawing;

namespace SliccDB.Explorer.Utility
{
    /// <summary>
    /// Interaction logic for DebugParseTreeVisual.xaml
    /// </summary>
    public partial class DebugParseTreeVisual : Window
    {
        private Graph graph = new Graph();
        public DebugParseTreeVisual(IParseTree tree)
        {
            InitializeComponent();
            if (GraphControl.Graph == null) GraphControl.Graph = new Graph("Parse Tree");
            GraphControl.Graph.AddNode("parent");
            GenerateGraph(tree, "parent");
        }

        public void GenerateGraph(IParseTree tree, string parentId)
        {
            if (GraphControl.Graph == null) GraphControl.Graph = new Graph("Parse Tree");
            for (int i = 0; i < tree.ChildCount; i++)
            {
                var child = tree.GetChild(i);
                var text = $"{child.GetType().Name} \n {child.GetText()}";
                GraphControl.Graph.AddNode(text);
                GraphControl.Graph.AddEdge(parentId, text);
                GenerateGraph(child, text);
            }
        }
    }
}
