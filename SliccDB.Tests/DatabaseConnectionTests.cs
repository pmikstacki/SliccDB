using NUnit.Framework;
using NUnit.Framework;
using SliccDB.Core;
using SliccDB.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SliccDB.Exceptions;
using SliccDB.Fluent;

namespace SliccDB.Tests
{
    public class Tests
    {
        private const string fileName = "sample.siccdb";

        public Node NodeOne { get; set; }

        public Node NodeTwo { get; set; }

        public DatabaseConnection Connection { get; set; }

        [SetUp]
        public void SetupDatabase()
        {
            Setup();
        }

        [Test]
        public void CreateSchemaTest()
        {
            try
            {
                Connection.CreateSchema("Person",
                    new List<Property>() { new() { FullTypeName = "System.String", Name = "Name" } });
            }
            catch (SchemaExistsException ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsTrue(Connection.Schemas.Exists(x => x.Label == "Person"));
        }

      
        [Test]
        public void CreateNodeTest()
        {
            CreateNodes();

            Assert.IsTrue(NodeOne.Labels.FirstOrDefault(x => x.Contains("Person")) != null, "Label Person not found.");
            Assert.IsTrue(NodeTwo.Labels.FirstOrDefault(x => x.Contains("Person")) != null, "Label Person not found.");

            Assert.IsTrue(NodeOne.Properties.FirstOrDefault(x => x.Key.Equals("Name")).Value.Equals("Alice"), "Property Name not found.");
            Assert.IsTrue(NodeTwo.Properties.FirstOrDefault(x => x.Key.Equals("Name")).Value.Equals("Steve"), "Property Name not found.");
        }

        [Test]
        public void CreateRelationsBetweenTwoNodesTest()
        {
            CreateNodes();
            CreateRelations();

            Assert.IsTrue(Connection.Relations.Any(x => x.RelationName.Equals("Likes")), "Relation not found.");
        }

        [Test]
        public void SelectNodeTest()
        {
            CreateNodes();

            var selectedNode = Connection.QueryNodes(x => x.Where(xa =>
            {
                xa.Properties.TryGetValue("Name", out var name);
                return name == "Steve";
            }).ToList()).First();
            Assert.IsTrue(selectedNode.Labels.Count > 0, "Node not found.");
        }

        [Test]
        public void SelectEdgeTest()
        {
            CreateNodes();
            CreateRelations();

            var selectedEdge = Connection.QueryRelations(x => x.Where(xa => xa.RelationName == "Likes").ToList()).First();
            Assert.IsTrue(selectedEdge.Labels.Count > 0, "Edge not found");
        }

        [Test]
        public void HaveNodesSchemaEnforced()
        {

            CreateNodes();
            CreateRelations();
            UpdateSchema();
            var nodesWithLabel = Connection.Nodes().Where(x => x.Labels.Contains("Person"));
            Assert.IsTrue(nodesWithLabel.ToList().Exists(x => x.Properties.ContainsKey("Age")));
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
                new Dictionary<string, string>() { { "Name", "Alice"}, {"Age", "18"} },
                new HashSet<string>() { "Person" }
                );

            NodeTwo = Connection.CreateNode(
                new Dictionary<string, string>() { { "Name", "Steve" }, { "Age", "20" } },
                new HashSet<string>() { "Person" }
                );
        }

        [Test]
        public void MapperGetNode()
        {
            MapperCreateNodes();
            var foundTrain = Connection.Nodes<Train>();
            Assert.IsTrue(foundTrain.Values.ToList().Exists(x => x.SerialNumber == "CE2356") && foundTrain.Values.ToList().Exists(x => x.SerialNumber == "CE2332"), foundTrain.Count.ToString());
        }


        [Test]
        public void MapperGetRelation()
        {
            Connection.ClearDatabase();
            MapperCreateNodes();
            MapperCreateRelation();
            var foundTrain = Connection.Relations<RidesTo>();
            Assert.IsTrue(foundTrain.Values.ToList().Exists(x => x.Platform == 2));
        }

        private void MapperCreateNodes()
        {
            Connection.CreateNode(new Train() { SerialNumber = "CE2332" });
            Connection.CreateNode(new Train() { SerialNumber = "CE2356" });
            Connection.CreateNode(new Station() { City = "Katowice" });
            Connection.CreateNode(new Station() { City = "Warsaw" });
        }

        private void MapperCreateRelation()
        {
            Connection.CreateRelation(n =>
                Connection.Nodes().Properties("SerialNumber".Value("CE2332")).FirstOrDefault(),
                a => Connection.Nodes().Properties("SerialNumber".Value("CE2356")).FirstOrDefault(),
                new RidesTo(){ Platform = 2 });
        }

        [Test]
        public void TestNodeUpdateTest()
        {
            Connection.ClearDatabase();
            CreateNodes();
            CreateRelations();
            var node = Connection.Nodes().Properties("Name".Value("Steve")).First();
            node.Properties["Name"] = "Steve2";
            Connection.Update(node);

            var selectedNode = Connection.QueryNodes(Nodes => Nodes.Where(foundNode =>
            {
                foundNode.Properties.TryGetValue("Name", out var newName);
                return newName == "Steve2";
            }).ToList()).First();
            Assert.IsTrue(selectedNode != null, "Node not found.");
        }


        [Test]
        public void RelationUpdateTest()
        {
            Connection.ClearDatabase();
            CreateNodes();
            CreateRelations();
            UpdateSchema();
            var relation = Connection.Relations("Likes").ToList().First();
            relation.Properties["How Much"] = "Not So Much";
            relation.Labels.Add("Love Hate Relationship");

            Connection.Update(relation);

            var selectedRelation = Connection.QueryRelations(relations => relations.Where(foundRelation => foundRelation.RelationName == "Likes").ToList()).First();
            Assert.IsTrue(selectedRelation != null && selectedRelation.Properties["How Much"] == "Not So Much" && selectedRelation.Labels.Contains("Love Hate Relationship"), "Relation not found or properties weren't changed.");
        }

        [Test]
        public void NodeDeleteTest()
        {
            Connection.ClearDatabase();
            var toDelete = Connection.CreateNode(
                new Dictionary<string, string>() { { "Name", "Tom" } , {"Age", "0"}},
                new HashSet<string>() { "Person" }
            );

            var queryToDelete = Connection.Nodes()
                .Properties("Name".Value("Tom"))?
                .Labels("Person")
                .FirstOrDefault();
            Connection.Delete(queryToDelete);

            var queryToCheck = Connection.Nodes().Properties("Name".Value("Tom"))?.Labels("Person").FirstOrDefault();

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

            var selectedRelation = Connection.QueryRelations(relations=> relations.Where(rel => rel.RelationName == "Test").ToList()).FirstOrDefault();
            Assert.IsTrue(selectedRelation == null, "Relation still exists");
        }

        public void UpdateSchema()
        {
            var schema = Connection.Schemas.FirstOrDefault(s => s.Label == "Person");
            if (schema == null)
            {
                schema = Connection.CreateSchema("Person",
                    new List<Property>() { new() { FullTypeName = "System.String", Name = "Name" } });
            }
            schema.Properties.Add(new() { FullTypeName = "System.Double", Name = "Age" });
            Connection.UpdateSchema(schema);
        }


        private void Setup()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, fileName);
            Connection = new DatabaseConnection(filePath);
        }


        [OneTimeTearDown]
        public void TearDown()
        {
            Connection.ClearDatabase();
        }
    }

    public class Train
    {
        public string SerialNumber { get; set; }
    }

    public class Station
    {
        public string City { get; set; }
    }
    public class RidesTo
    { 
        public int Platform { get; set; }
    }
}