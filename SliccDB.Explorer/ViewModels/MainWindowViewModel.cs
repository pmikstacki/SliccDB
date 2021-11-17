using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Caliburn.Micro;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Msagl.Drawing;
using Microsoft.Win32;
using SliccDB.Core;
using SliccDB.Cypher;
using SliccDB.Explorer.Model;
using SliccDB.Explorer.Utility;
using SliccDB.Explorer.Views;
using SliccDB.Serialization;
using Syncfusion.Linq;
using WpfGraphControl;
using CollectionExtensions = SliccDB.Explorer.Utility.CollectionExtensions;

namespace SliccDB.Explorer.ViewModels
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        #region Properties

        bool debugTree;

        public bool DebugTree
        {
            get { return debugTree; }
            set
            {
                debugTree = value;
                NotifyOfPropertyChange(() => DebugTree);
            }

        }

        ObservableCollection<QueryResultsViewModel> queryResults = new ObservableCollection<QueryResultsViewModel>();

        public ObservableCollection<QueryResultsViewModel> QueryResults
        {
            get { return queryResults; }
            set
            {
                queryResults = value;
                NotifyOfPropertyChange(() => QueryResults);
            }
        }

        IHighlightingDefinition highlightForCypher;

        public IHighlightingDefinition HighlightForCypher
        {
            get { return highlightForCypher; }
            set
            {
                highlightForCypher = value;
                NotifyOfPropertyChange(() => HighlightForCypher);
            }
        } 


        string cypherQuery;

        public string CypherQuery
        {
            get { return cypherQuery; }
            set
            {
                cypherQuery = value;
                NotifyOfPropertyChange(() => CypherQuery);
            }
        }

        public static Random random = new Random();

        public System.Windows.Media.Color RandomColor = System.Windows.Media.Color.FromRgb(
            (byte)random.Next(255),
            (byte)random.Next(255),
            (byte)random.Next(255)
        );

        ObservableCollection<Property> properties = new ObservableCollection<Property>();

        public ObservableCollection<Property> Properties
        {
            get { return properties; }
            set
            {
                properties = value;
                NotifyOfPropertyChange(() => Properties);
            }
        }
        
        bool showInfoPanel;

        public bool ShowInfoPanel
        {
            get { return showInfoPanel; }
            set
            {
                showInfoPanel = value;
                NotifyOfPropertyChange(() => ShowInfoPanel);
            }
        }

        string selectedType;

        public string SelectedType
        {
            get { return selectedType; }
            set
            {
                selectedType = value;
                NotifyOfPropertyChange(() => SelectedType);
            }
        }

        string selectedHash;

        public string SelectedHash
        {
            get { return selectedHash; }
            set
            {
                selectedHash = value;
                NotifyOfPropertyChange(() => SelectedHash);
            }
        }

        ObservableCollection<string> selectedTags;

        public ObservableCollection<string> SelectedTags
        {
            get { return selectedTags; }
            set
            {
                selectedTags = value;
                NotifyOfPropertyChange(() => SelectedTags);
            }
        }

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

        private NodeViewModel node = new NodeViewModel();

        public NodeViewModel Node
        {
            get => node;
            set
            {
                node = value;
                NotifyOfPropertyChange(() => Node);
            }

        }

        private RelationViewModel relation = new RelationViewModel();

        public RelationViewModel Relation
        {
            get => relation;
            set
            {
                relation = value;
                NotifyOfPropertyChange(() => Relation);
            }

        }



        bool isDatabaseConnected;

        public bool IsDatabaseConnected
        {
            get { return isDatabaseConnected; }
            set
            {
                isDatabaseConnected = value;
                NotifyOfPropertyChange(() => IsDatabaseConnected);
            }
        }
        private DatabaseConnection _databaseConnection;

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            using (Stream s = typeof(MainWindowView).Assembly.GetManifestResourceStream("Cypher"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    HighlightForCypher = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }

                HighlightingManager.Instance.RegisterHighlighting("OpenCypher", new string[] { ".cyp" }, HighlightForCypher);
            }
        }

        #endregion

        #region Public Methods

        public void ExecuteCypher()
        {
            CypherInterpreter interpreter = new CypherInterpreter(_databaseConnection);
            Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\temp\\");
            File.WriteAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\temp\\cypherstring.txt", CypherQuery);
            var result = interpreter.Interpret(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\temp\\cypherstring.txt");
            QueryResults.Add(new QueryResultsViewModel(result));
            if (DebugTree)
            {
                new DebugParseTreeVisual(interpreter.debugTreeInfo).Show();
            }
            GenerateGraph();
        }

        public void OpenDatabase()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                _databaseConnection = new DatabaseConnection(ofd.FileName);
            }

            GenerateGraph();

            if (_databaseConnection.ConnectionStatus == ConnectionStatus.Connected) IsDatabaseConnected = true;
        }

        public void SaveDatabase()
        {
            _databaseConnection.SaveDatabase();
        }

        public void GenerateGraph()
        {
            var _graph = new
                Microsoft.Msagl.Drawing.Graph("graph");
            _graph.Attr.BackgroundColor = new Color(59, 58, 57);
            _graph.Attr.Color = new Color(255, 255, 255);
            _databaseConnection.Nodes.ForEach(x =>
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
            _databaseConnection.Relations.ForEach(r =>
            {
                _graph.AddEdge(r.SourceHash,r.RelationName, r.TargetHash);
            });
            Graph = _graph;

            GraphSelection.OnGraphElementSelection += (s, b) =>
            {
                ShowInfoPanel = true;
                SelectedType = b ? "Node" : "Relation";
                SelectedHash = s;
                if (b)
                {
                    var selectedNode = _databaseConnection.QueryNodes(x => x.Where(x => x.Hash == s).ToList()).FirstOrDefault();
                    if (selectedNode != null)
                    {
                        SelectedTags = FunctionalExtensions.ToObservableCollection(selectedNode.Labels);

                        Properties = CollectionExtensions.DictionaryToObservableCollection(selectedNode.Properties);
                    }


                }

                else
                {
                    var selectedEdge = _databaseConnection.QueryRelations(x => x.Where(x => x.Hash == s).ToList()).First();


                    SelectedTags = FunctionalExtensions.ToObservableCollection(_databaseConnection.QueryRelations(x => x.Where(x => x.Hash == s).ToList()).First()
                            .Labels);

                    Properties = CollectionExtensions.DictionaryToObservableCollection(selectedEdge.Properties);

                }


            };

        }

        public void CloseEntityInfoPanel()
        {
            ShowInfoPanel = false;
        }

        public void AddNode()
        {
            var newNode = this.node.ReturnNode();
            _databaseConnection.CreateNode(newNode.Properties, newNode.Labels);
            GenerateGraph();
        }

        public void AddRelation()
        {
            var newRelation = this.Relation.ReturnRelation();
            _databaseConnection.CreateRelation(newRelation.RelationName, sn => sn.First(x => x.Hash == newRelation.SourceHash), tn => tn.First(a => a.Hash==newRelation.TargetHash), newRelation.Properties, newRelation.Labels);
            GenerateGraph();
        }

        public void ClearDatabase()
        {
            _databaseConnection.ClearDatabase();
            GenerateGraph();
        }
        #endregion
    }
}
