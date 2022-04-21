using NUnit.Framework;
using SliccDB.Core;
using SliccDB.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SliccDB.Fluent;

namespace SliccDB.Tests
{
    public class Tests
    {
        private const string fileName = "sample";

        public Node NodeOne { get; set; }

        public Node NodeTwo { get; set; }

        public DatabaseConnection Connection { get; set; }

        [SetUp]
        public void SetupDatabase()
        {
            Setup();
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

        [Test]
        public void SaveDatabase()
        {
            int nodes = 0;
            int relations = 0;

            CreateNodes();
            CreateRelations();

            nodes = Connection.Nodes.Count;
            relations = Connection.Relations.Count;

            Connection.SaveDatabase();
            Connection.CloseDatabase();

            Setup();

            Assert.IsTrue(Connection.Nodes.Count.Equals(nodes), "Saved nodes are different.");
            Assert.IsTrue(Connection.Relations.Count.Equals(relations), "Saved relations are different.");
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
        
        [Test]
        public void TestNodeUpdateTest()
        {

            var node = Connection.QueryNodes(x => x.Where(x => x.Properties["Name"] == "Steve").ToList()).First();
            node.Properties["Name"] = "Steve2";

            Connection.Update(node);

            var selectedNode = Connection.QueryNodes(x => x.Where(x => x.Properties["Name"] == "Steve2").ToList()).First();
            Assert.IsTrue(selectedNode != null, "Node not found.");
        }


        [Test]
        public void RelationUpdateTest()
        {

            var relation = Connection.QueryRelations(x => x.Where(x => x.RelationName == "Likes").ToList()).First();
            relation.Properties["How Much"] = "Not So Much";
            relation.Labels.Add("Love Hate Relationship");

            Connection.Update(relation);

            var selectedRelation = Connection.QueryRelations(x => x.Where(x => x.RelationName == "Likes").ToList()).First();
            Assert.IsTrue(selectedRelation != null && selectedRelation.Properties["How Much"] == "Not So Much" && selectedRelation.Labels.Contains("Love Hate Relationship"), "Relation not found or properties weren't changed.");
        }

        [Test]
        public void NodeDeleteTest()
        {

            var toDelete = Connection.CreateNode(
                new Dictionary<string, string>() { { "Name", "Tom" } },
                new HashSet<string>() { "Person" }
            );

            var queryToDelete = Connection.Nodes().Properties("Name".Value("Tom")).Labels("Person").FirstOrDefault();

            Connection.Delete(queryToDelete);

            var queryToCheck = Connection.Nodes().Properties("Name".Value("Tom")).Labels("Person").FirstOrDefault();

            Assert.IsTrue(queryToCheck == null, "Node was not removed");

        }


        [Test]
        public void RelationDeleteTest()
        {
            var testNode1 = Connection.CreateNode(
                new Dictionary<string, string>() { { "Name", "C" } },
                new HashSet<string>() { "S" }
            );

            var testNode2 = Connection.CreateNode(
                new Dictionary<string, string>() { { "Name", "A" } },
                new HashSet<string>() { "S" }
            );            

            var properties = new Dictionary<string, string>();
            var labels = new HashSet<string>();
            properties.Add("Test", "Test");
            labels.Add("Test on a node!");
            Connection.CreateRelation("Test", sn => sn.First(x => x.Hash == testNode1.Hash), tn => tn.First(a => a.Hash == testNode2.Hash), properties, labels);

            var queriedRelation = Connection.Relations("Test").FirstOrDefault();

            Connection.Delete(queriedRelation);

            var selectedRelation = Connection.QueryRelations(x => x.Where(x => x.RelationName == "Test").ToList()).FirstOrDefault();
            Assert.IsTrue(selectedRelation == null, "Relation still exists");
        }

        private void Setup()
        {
            string folderPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\data\"));
            string filePath = Path.Combine(folderPath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                Connection = new DatabaseConnection(filePath);
            }
        }
    }
}