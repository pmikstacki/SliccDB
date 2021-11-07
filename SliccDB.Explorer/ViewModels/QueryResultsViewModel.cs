using System.Linq;
using Caliburn.Micro;
using Microsoft.Msagl.Drawing;
using Newtonsoft.Json;
using SliccDB.Core;
using Syncfusion.Linq;

namespace SliccDB.Explorer.ViewModels
{
    public class QueryResultsViewModel : PropertyChangedBase
    {
        private Graph graph;

        public Graph Graph
        {
            get => graph;
            set
            {
                graph = value;
                NotifyOfPropertyChange(() => Graph);
            }

        }

        string jsonQueryResult;

        public string JsonQueryResult
        {
            get { return jsonQueryResult; }
            set
            {
                jsonQueryResult = value;
                NotifyOfPropertyChange(() => JsonQueryResult);
            }
        }

        private QueryResult _result;

        public QueryResultsViewModel(QueryResult result)
        {
            _result = result;
            jsonQueryResult = JsonConvert.SerializeObject(result, Formatting.Indented);
            GenerateGraph();
        }
        public void GenerateGraph()
        {
            var _graph = new
                Microsoft.Msagl.Drawing.Graph("graph");
            _graph.Attr.BackgroundColor = new Color(59, 58, 57);
            _graph.Attr.Color = new Color(255, 255, 255);
            _result.Nodes.ForEach(x =>
            {
                var labels = string.Join(", ", x.Labels.Select(x => $":{x}"));
                var props = string.Join("\n", x.Properties.Select(x => $"{x.Key} : {x.Value}"));
                var labelsAndProps = labels + "\n" + props;
                _graph.AddNode(new Microsoft.Msagl.Drawing.Node(x.Hash)
                {
                    LabelText = labelsAndProps,
                });
                _graph.FindNode(x.Hash).Attr.FillColor = Color.LightBlue;
            });
            _result.Relations.ForEach(r =>
            {
                _graph.AddEdge(r.SourceHash, r.RelationName, r.TargetHash);
            });
            Graph = _graph;
        }
    }
}