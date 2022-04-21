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
    public class DatabaseConnectionTests
    {        
        private const string fileName = "sample";

        public Node NodeOne { get; set; }

        public Node NodeTwo { get; set; }

        public DatabaseConnection Connection { get; set; }

        [Test]
        public void CreateNodeTest()
        {

            Assert.IsTrue(NodeOne.Labels.Where(x => x.Contains("Person")).FirstOrDefault() != null, "Label Person not found.");
            Assert.IsTrue(NodeTwo.Labels.Where(x => x.Contains("Person")).FirstOrDefault() != null, "Label Person not found.");

            Assert.IsTrue(NodeOne.Properties.Where(x => x.Key.Equals("Name")).FirstOrDefault().Value.Equals("Alice"), "Property Name not found.");
            Assert.IsTrue(NodeTwo.Properties.Where(x => x.Key.Equals("Name")).FirstOrDefault().Value.Equals("Steve"), "Property Name not found.");
        }

        [Test]
        public void CreateRelationsBetweenTwoNodesTest()
        {
           
            Assert.IsTrue(Connection.Relations.Where(x => x.RelationName.Equals("Likes")).Count() > 0, "Relation not found.");
        }

        [Test]
        public void SelectNodeTest()
        {

            var selectedNode = Connection.QueryNodes(x => x.Where(x => x.Properties["Name"] == "Steve").ToList()).First();
            Assert.IsTrue(selectedNode.Labels.Count > 0, "Node not found.");
        }

        [Test]
        public void SelectEdgeTest()
        {

            var selectedEdge = Connection.QueryRelations(x => x.Where(x => x.RelationName == "Likes").ToList()).First();
            Assert.IsTrue(selectedEdge.Labels.Count > 0, "Edge not found");
        }

        [Test]
        public void SaveDatabase()
        {
            int nodes = 0;
            int relations = 0;

            nodes = Connection.Nodes.Count;
            relations = Connection.Relations.Count;

            Connection.SaveDatabase();
            Connection.CloseDatabase();

            Setup();

            Assert.IsTrue(Connection.Nodes.Count.Equals(nodes), "Saved nodes are different.");
            Assert.IsTrue(Connection.Relations.Count.Equals(relations), "Saved relations are different.");
        }

        [Test]
        public void TestNodeUpdate()
        {

            var node = Connection.QueryNodes(x => x.Where(x => x.Properties["Name"] == "Steve").ToList()).First();
            node.Properties["Name"] = "Steve2";

            Connection.Update(node);

            var selectedNode = Connection.QueryNodes(x => x.Where(x => x.Properties["Name"] == "Steve2").ToList()).First();
            Assert.IsTrue(selectedNode != null, "Node not found.");
        }


        [Test]
        public void TestRelationUpdate()
        {

            var relation = Connection.QueryRelations(x => x.Where(x => x.RelationName == "Likes").ToList()).First();
            relation.Properties["How Much"] = "Not So Much";
            relation.Labels.Add("Love Hate Relationship");

            Connection.Update(relation);

            var selectedRelation = Connection.QueryRelations(x => x.Where(x => x.RelationName == "Likes").ToList()).First();
            Assert.IsTrue(selectedRelation != null && selectedRelation.Properties["How Much"] == "Not So Much" && selectedRelation.Labels.Contains("Love Hate Relationship"), "Relation not found or properties weren't changed.");
        }

        [Test]
        public void TestRelationRemove()
        {
            var relation = Connection.Relations("Likes").FirstOrDefault();
            var numRemoved = Connection.Remove(relation);

            Assert.IsTrue(numRemoved > 0, "Relation was not removed");
        }


        [Test]
        public void TestNodeRemove()
        {
            var node = Connection.Nodes().Labels("Person").Properties("Name".Value("Alice")).FirstOrDefault();
            var numRemoved = Connection.Remove(node);

            Assert.IsTrue(numRemoved > 0, "Node was not removed");
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
        
        [SetUp]
        public void Setup()
        {
            string folderPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\data\"));
            string filePath = Path.Combine(folderPath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                Connection = new DatabaseConnection(filePath);
            }
            else
            {
                filePath = Path.Combine(folderPath, GenerateName(8) + ".sliccdb");
                Connection = new DatabaseConnection(filePath);
            }

            CreateNodes();
            CreateRelations();
        }

        public static string GenerateName(int len)
        {
            Random r = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2;
            while (b < len)
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }

            return Name;
        }
    }
}