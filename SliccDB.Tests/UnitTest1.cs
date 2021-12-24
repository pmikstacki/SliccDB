using NUnit.Framework;
using SliccDB.Core;
using SliccDB.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace SliccDB.Tests
{
    public class Tests
    {        
        public Node NodeOne { get; set; }

        public Node NodeTwo { get; set; }

        public DatabaseConnection Connection { get; set; }

        [SetUp]
        public void SetupDatabase()
        {
            Connection = new DatabaseConnection("filename");
        }

        [Test]
        public void CreateNodeTest()
        {
            CreateNodes();

            Assert.IsTrue(NodeOne.Labels.Where(x => x.Contains("Person")).FirstOrDefault() != null, "Label Person not found.");
            Assert.IsTrue(NodeTwo.Labels.Where(x => x.Contains("Person")).FirstOrDefault() != null, "Label Person not found.");

            Assert.IsTrue(NodeOne.Properties.Where(x => x.Key.Equals("Name")).FirstOrDefault().Value.Equals("Alice"), "Property Name not found.");
            Assert.IsTrue(NodeTwo.Properties.Where(x => x.Key.Equals("Name")).FirstOrDefault().Value.Equals("Steve"), "Property Name not found.");
        }

        [Test]
        public void CreateRelationsBetweenTwoNodesTest()
        {
            CreateNodes();
            CreateRelations();

            Assert.IsTrue(Connection.Relations.Where(x => x.RelationName.Equals("Likes")).Count() > 0, "Relation not found.");
        }

        [Test]
        public void SelectNodeTest()
        {
            CreateNodes();

            var selectedNode = Connection.QueryNodes(x => x.Where(x => x.Properties["Name"] == "Steve").ToList()).First();
            Assert.IsTrue(selectedNode.Labels.Count > 0, "Node not found.");
        }

        [Test]
        public void SelectEdgeTest()
        {
            CreateNodes();
            CreateRelations();

            var selectedEdge = Connection.QueryRelations(x => x.Where(x => x.RelationName == "Likes").ToList()).First();
            Assert.IsTrue(selectedEdge.Labels.Count > 0, "Edge not found");
        }

        private void CreateRelations()
        {
            var properties = new Dictionary<string, string>();
            var labels = new HashSet<string>();
            properties.Add("How Much", "Very Much");
            labels.Add("Label on a node!");
            Connection.CreateRelation("Likes", sn => sn.First(x => x.Hash == NodeOne.Hash), tn => tn.First(a => a.Hash == NodeTwo.Hash), properties, labels);
        }

        private void CreateNodes()
        {
            NodeOne = Connection.CreateNode(
                new Dictionary<string, string>() { { "Name", "Alice" } },
                new HashSet<string>() { "Person" }
                );

            NodeTwo = Connection.CreateNode(
                new Dictionary<string, string>() { { "Name", "Steve" } },
                new HashSet<string>() { "Person" }
                );
        }
    }
}