# SliccDB
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white) ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white) ![Visual Studio](https://img.shields.io/badge/Visual%20Studio-5C2D91.svg?style=for-the-badge&logo=visual-studio&logoColor=white)
![Version](https://img.shields.io/badge/Version-0.1.1-brightgreen?style=for-the-badge)

Light Embedded Graph Database for .net
<img src="https://raw.githubusercontent.com/pmikstacki/SliccDB/master/SliccDB/SLICC_128.png" width="128" height="128" align="right">

## What is it?
Think of it like SQLite for graph databases. There were some efforts to build something like this (such as Graph Engine) but Microsoft decided to abandon all of their graph database related projects. Be aware that I have no intention to make this some sort of Neo4J .net Clone. It will be not fully compliant with features of mentioned database.

## Installation
You have three options to get this package.
### Nuget Package
You can download nuget package from [here](https://www.nuget.org/packages/SliccDB/) or by using this command
```
NuGet\Install-Package SliccDB
```

### Unstable Builds
If you feel adventurous, you can download SliccDB as a build artifact from github actions. Note that these are not official releases, so they may contain bugs. 
But, they can also contain features not yet found on nuget... you never know ¯\_(ツ)_/¯

### Compiling from source
You can always just clone this repo, open the solution with visual studio and compile it yourself. The build will produce both the dll and nuget package. 
You can also just build it with dotnet tool thingy. You're the boss here!


### Examples
Here are some simple examples:

#### Basic Operations

##### Create Node Programatically 

```Csharp
DatabaseConnection connection = new DatabaseConnection("filename");
var properties = new Dictionary<string, string>();
var labels = new HashSet<string>();
properties.Add("Name", "Alice");
labels.Add("Person");

var nodeOne = connection.CreateNode(properties, labels);
properties.Clear();
labels.Clear();
properties.Add("Name", "Steve");
labels.Add("Person");

var nodeTwo = connection.CreateNode(properties, labels);
```

##### Create Relations between two nodes
Note that relations also have ability to hold data like Properties and Labels
```Csharp
var properties = new Dictionary<string, string>();
var labels = new HashSet<string>();
properties.Add("How Much", "Very Much");
labels.Add("Label on a node!");
connection.CreateRelation(
"Likes", 
sn => sn.First(x => x.Hash == nodeOne.Hash), tn => tn.First(a => a.Hash == nodeTwo.Hash), properties, labels);
```

#### Queries

You can query nodes and relations as you would any collection (With Linq). 

###### Query Nodes

```Csharp
var selectedNode = Connection.Nodes().Properties("Name".Value("Tom")).Labels("Person").FirstOrDefault();
```

###### Query Relations

```Csharp
var queriedRelation = Connection.Relations().Properties("Property".Value("PropertyValue"))
                .Labels("RelationLabel");
```


#### Mutations

##### Updates

```Csharp
 var relation = Connection.Relations("Likes").ToList().First();
            relation.Properties["How Much"] = "Not So Much";
            relation.Labels.Add("Love Hate Relationship");

Connection.Update(relation);
            
...

 var node = Connection.Nodes().Properties("Name".Value("Steve")).First();
            node.Properties["Name"] = "Steve2";

Connection.Update(node);
```

###### Deletions

```CSharp
var toDelete = Connection.CreateNode(
                new Dictionary<string, string>() { { "Name", "Tom" } },
                new HashSet<string>() { "Person" }
            );

var queryToDelete = Connection.Nodes().Properties("Name".Value("Tom")).Labels("Person").FirstOrDefault();

Connection.Delete(queryToDelete);
```
```CSharp
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
```

#### Schemas and Mappings
Since 0.1.1 SliccDb features object - graph entity mapping, allowing to treat Relations and Nodes like regular poco classes.
To ensure correct mapping for both types and property names it was neccessary to implement some sort of **schema enforcement**.Therefore, schemas were born.

##### Schemas
Schemas are like recipes for your graph entities, they enforce the existence of certain fields and allow to specify to which type these fields should be mapped. 
Schemas are strongly related to labels. Schemas are defined per label. 

Example: 
```Csharp
schema = Connection.CreateSchema("Person",
                    new List<Property>() { new() { FullTypeName = "System.String", Name = "Name" }, new() { FullTypeName = "System.Double", Name = "Age" } });
```

Now when we label any entity with "Person", missing fields will be automatically added to it. In this example, when we add a label "Person" to a node or relation it will create Fields "Name" and "Age" if those do not exist.

##### Mapper
Mapper allows us to manipulate graph by manipulating regular POCOs. 
Let's say we have two classes, representing Trains, Stations and a class RidesTo which represents relation between train and station: 
```Csharp
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
```

By passing an object as an argument to method CreateNode we can create node with properties equal to the properties of an object:
```Csharp
  Connection.CreateNode(new Train() { SerialNumber = "CE2332" });
  Connection.CreateNode(new Train() { SerialNumber = "CE2356" });
  Connection.CreateNode(new Station() { City = "Katowice" });
  Connection.CreateNode(new Station() { City = "Warsaw" });
```
We can also create a relation connecting two stations (by a query) with a train: 

```Csharp
Connection.CreateRelation(n =>
                Connection.Nodes().Properties("SerialNumber".Value("CE2332")).FirstOrDefault(),
                a => Connection.Nodes().Properties("SerialNumber".Value("CE2356")).FirstOrDefault(),
                new RidesTo(){ Platform = 2 });
```

### Why it exists?
I just need embedded Graph Database Solution that supports Cypher Language.
Graph DBMS is a perfect solution for some gamedev ai-related stuff (Procedural Behaviour Design, Perception and context awareness storage), but every solution needs a server. Imagine that in order to play your game you must install Neo4J's server in the first place, or every time your npc makes a decision it queries Azure Cloud to check how much he likes pizza. It's unreliable and forces you to host a game server for the entirety of a game's lifetime even though your game is singleplayer. Sounds stupid? Well, ask EA about Simcity 2013 - they thought it's a great idea! But really, Graph DBMS has other use cases. [Watch this video to know more about Graph DBMS](https://www.youtube.com/watch?v=GekQqFZm7mA)

### SliccDB Studio
You can download SliccDB Studio [here](https://github.com/pmikstacki/SliccDBStudio).

SliccDB Studio is a companion app currently used for interaction with database. It allows for CRUD operations on the database to test its' functionalities. 

![SliccDB Studio Screenshot](https://raw.githubusercontent.com/pmikstacki/SliccDBStudio/main/Screenshot.png)



**DB Explorer is now considered obsolete, but you can still get the source code [here](https://github.com/pmikstacki/SliccDB.Explorer/)**

### Progress
Below you'll find a list of features that will be included in the 1st milestone release of SliccDB:
- [x] Basic Structures (Relations, Nodes)
- [x] Serialization (Reading and Writing)
- [x] Queries
- [x] Basic Debugger App (Ability to add, remove, edit nodes and relations)
- [x] Schemas for Nodes and Relations
- [x] ORM (Object Relational Mapper) for Nodes and Relations
- [x] New Companion App
### Can I help?
Of course! Any help with the SliccDB will be appreciated. 
#### Special Thanks!
- [Oleitao](https://github.com/oleitao) He wrote first unit tests of the database!
- [HoLLy-Hacker (cool name tho) ](https://github.com/HoLLy-HaCKeR) - He performance tested key functionalities of SliccDB and found a few ~~errors~~ bugs. 
