using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using deniszykov.TypeConversion;
using MessagePack;
using SliccDB.Core;
using SliccDB.Exceptions;
using SliccDB.Fluent;

namespace SliccDB.Serialization
{

    public class DatabaseConnection
    {
        public string FilePath { get; private set; }

        internal Database Database { get; private set; }

        public HashSet<Node> Nodes => Database?.Nodes;
        public HashSet<Relation> Relations => Database?.Relations;
        public List<Schema> Schemas => Database?.Schemas;

        public ConnectionStatus ConnectionStatus { get; set; }
        private readonly bool realtime;

        private TypeConversionProvider typeConversionProvider;

        /// <summary>
        /// Creates new Database Connection Instance
        /// </summary>
        /// <param name="filePath">Path to a database file</param>
        /// <param name="realtime">if true, attempts to save the database on every operation. Disabled by default as it is memory intensive</param>
        public DatabaseConnection(string filePath, bool realtime = false)
        {
            FilePath = filePath;
            if (File.Exists(filePath))
            {
                var bytes = File.ReadAllBytes(filePath);
                try
                {
                    Database = MessagePackSerializer.Deserialize<Database>(bytes);
                    this.ConnectionStatus = ConnectionStatus.Connected;
                }
                catch (Exception exception)
                {
                    Database = new Database();
                    this.ConnectionStatus = ConnectionStatus.Connected;
                }
            }
            else
            {
                Database = new Database();
                this.ConnectionStatus = ConnectionStatus.Connected;
                var bytes = MessagePackSerializer.Serialize(Database);
                File.WriteAllBytes(filePath, bytes);
            }
            this.realtime = realtime;

            //init type conversion provider
            typeConversionProvider = new TypeConversionProvider();
            typeConversionProvider.RegisterConversion<string, string>((s, s1, arg3) => s, ConversionQuality.Constructor);
        }
        /// <summary>
        /// Queries nodes based on generic delegate type
        /// </summary>
        /// <param name="query">generic delegate type</param>
        /// <returns>Queried Nodes</returns>
        public IEnumerable<Node> QueryNodes(Func<HashSet<Node>, IEnumerable<Node>> query)
        {
            return query.Invoke(Nodes);
        }
        /// <summary>
        /// Queries relations based on generic delegate type
        /// </summary>
        /// <param name="query">generic delegate type</param>
        /// <returns>Queried Relations</returns>
        public IEnumerable<Relation> QueryRelations(Func<HashSet<Relation>, IEnumerable<Relation>> query)
        {
            return query.Invoke(Relations);
        }

        /// <summary>
        /// Updates Node or Relation
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="InvalidOperationException">Throws when wrong type of GraphEntity is supplied</exception>
        public void Update(GraphEntity entity)
        {
            ValidateSchema(entity);
            if (entity is Node node)
            {
                Nodes.Replace(node);
            }
            else if (entity is Relation relation)
            {
                Relations.Replace(relation);
            }
            else
            {
                throw new InvalidOperationException("Invalid entity type");
            }
            SaveDatabase();
        }

        /// <summary>
        /// Enforces a schema based on amount of schemas
        /// </summary>
        /// <param name="entity">entity to check schema enforcement on</param>
        /// <exception cref="SchemaValidationException">Throws when schema validation for given label fails</exception>
        private void ValidateSchema(GraphEntity entity)
        {
            var schema = Schemas.FirstOrDefault(s => entity.Labels.Contains(s.Label));

            if (schema == null)
                return;

            Dictionary<string, string> reasons = new Dictionary<string, string>();
            bool result = true;
            foreach (var schemaProperty in schema.Properties)
            {
                if (!entity.Properties.TryGetValue(schemaProperty.Name, out var foundProperty))
                {
                    reasons.Add(schemaProperty.Name, "Not Found");
                    result = false;
                }
               
                if (foundProperty != null && !typeConversionProvider.TryConvert(typeof(string), Type.GetType(schemaProperty.FullTypeName),
                       foundProperty, out var convertResult))
                {
                    reasons.Add(schemaProperty.Name,
                        "Converter not registered, try registering one with RegisterConversion");
                    result = false;
                }
            }

            if (!result)
            {
                throw new SchemaValidationException(schema.Label, reasons);
            }
        }

        /// <summary>
        /// Bulk updates Node or Relation
        /// </summary>
        /// <param name="entities">collection of entities to update</param>
        /// <exception cref="InvalidOperationException">Throws when wrong type of GraphEntity is supplied</exception>
        public void BulkUpdate(List<GraphEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity is Node node)
                {
                    Nodes.Replace(node);
                }
                else if (entity is Relation relation)
                {
                    Relations.Replace(relation);
                }
                else
                {
                    throw new InvalidOperationException("Invalid entity type");
                }
            }
            SaveDatabase();
        }
        /// <summary>
        /// Deletes any entity passed and related nodes
        /// </summary>
        /// <param name="entity">entity to delete</param>
        /// <returns>all deleted entities</returns>
        public int Delete(GraphEntity entity)
        {
            var nodes = Nodes.RemoveWhere(n => n.Hash == entity.Hash);
            var relationsLinkedWithNodes = Relations.RemoveWhere(r => r.SourceHash == entity.Hash || r.TargetHash == entity.Hash);
            var relations = Relations.RemoveWhere(r => r.Hash == entity.Hash);

            if (realtime) SaveDatabase();

            return nodes + relationsLinkedWithNodes + relations;
        }
        /// <summary>
        /// Find nodes with relation with name as it's source (node->relation)
        /// </summary>
        /// <param name="relationName">name of the relation</param>
        /// <returns>all nodes with relation as their source</returns>
        public IEnumerable<Node> FindNodesWithRelationSource(string relationName)
        {
            var relationsSources = Relations.AsParallel().Where(x => x.RelationName == relationName).Select(x => x.SourceHash);
            return Nodes.AsParallel().Where(x => relationsSources.Contains(x.Hash));
        }
        /// <summary>
        /// Find nodes with relation with name as it's target (relation->node)
        /// </summary>
        /// <param name="relationName">name of the relation</param>
        /// <returns>all nodes with relation as their target</returns>
        public IEnumerable<Node> FindNodesWithRelationTarget(string relationName)
        {
            var relationsSources = Relations.AsParallel().Where(x => x.RelationName == relationName).Select(x => x.TargetHash);
            return Nodes.AsParallel().Where(x => relationsSources.Contains(x.Hash));
        }
        /// <summary>
        /// Saves a database
        /// </summary>
        public void SaveDatabase()
        {
            if (File.Exists(FilePath))
            {
                var bytes = MessagePackSerializer.Serialize(Database);
                File.WriteAllBytes(FilePath, bytes);
            }
        }
        /// <summary>
        /// Closes database
        /// </summary>
        public void CloseDatabase()
        {
            if (Database != null && ConnectionStatus == ConnectionStatus.Connected)
            {
                this.ConnectionStatus = ConnectionStatus.NotConnected;
                Database.Dispose();
            }
        }
        /// <summary>
        /// Clears database
        /// </summary>
        public void ClearDatabase()
        {
            Nodes.Clear();
            Relations.Clear();
            Schemas.Clear();
            SaveDatabase();
        }
        /// <summary>
        /// Creates a new node with given properties and labels
        /// </summary>
        /// <param name="properties">properties</param>
        /// <param name="labels">labels</param>
        /// <returns></returns>
        public Node CreateNode(Dictionary<string, string> properties = null, HashSet<string> labels = null)
        {
            Dictionary<string, string> props = new Dictionary<string, string>();
            HashSet<string> lablSet = new HashSet<string>();
            var node = new Node(properties ?? props, labels ?? lablSet);
            ValidateSchema(node);
            Database.Nodes.Add(node);
            if(realtime) SaveDatabase();
            return node;
        }
        /// <summary>
        /// Creates a new schema
        /// </summary>
        /// <param name="label"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        /// <exception cref="SchemaExistsException"></exception>
        public Schema CreateSchema(string label, List<Property> properties)
        {
            if (Schemas.Exists(x => x.Label == label))
            {
                throw new SchemaExistsException(label);
            }

            Schema schema = new Core.Schema();
            schema.Label = label;
            schema.Properties = properties;
            Schemas.Add(schema);
            UpdateEntitiesForSchema(schema);
            if (realtime) SaveDatabase();
            return schema;
        }
        /// <summary>
        /// Creates a new schema
        /// </summary>
        /// <param name="schema">Schema to create</param>
        /// <returns>Schema added to database</returns>
        /// <exception cref="SchemaExistsException">Throws if schema exists</exception>
        public Schema CreateSchema(Schema schema)
        {
            if (Schemas.Exists(x => x.Label == schema.Label))
            {
                throw new SchemaExistsException(schema.Label);
            }
            Schemas.Add(schema);
            UpdateEntitiesForSchema(schema);
            if (realtime) SaveDatabase();
            return schema;
        }
        /// <summary>
        /// Deletes Schema while retaining entity values
        /// </summary>
        /// <param name="schema">schema to delete</param>
        public void DeleteSchema(Schema schema)
        {
            Schemas.Remove(schema);
        }

        /// <summary>
        /// Deletes schema using label 
        /// </summary>
        /// <param name="schemaLabel">label for the schema</param>
        /// <exception cref="InvalidOperationException">throws if schema with given label was not found</exception>
        public void DeleteSchemaByLabel(string schemaLabel)
        {
            var schema = Schemas.FirstOrDefault(s => s.Label == schemaLabel);
            if (schema is null)
                throw new InvalidOperationException($"Schema with label {schemaLabel} does not exist!");
            Schemas.Remove(schema);

        }

        /// <summary>
        /// Updates Schema that already exists
        /// </summary>
        /// <param name="schema">Schema to update</param>
        /// <returns>Updated schema</returns>
        /// <exception cref="InvalidOperationException">Throws if schema for this label doesn't exist</exception>
        public Schema UpdateSchema(Schema schema)
        {
            var foundSchema = Schemas.FirstOrDefault(x => x.Label == schema.Label);
            if (foundSchema == null)
                throw new InvalidOperationException("Schema was not found. Use CreateSchema instead");
            
            Schemas.Remove(foundSchema);

            Schemas.Add(schema);
            UpdateEntitiesForSchema(schema);
            if (realtime) SaveDatabase();
            return schema;
        }
        /// <summary>
        /// Creates relation entity
        /// </summary>
        /// <param name="relationName">relation name</param>
        /// <param name="sourceNode">source node search func</param>
        /// <param name="targetNode">target node search func</param>
        /// <param name="properties">properties in relation</param>
        /// <param name="labels">labels in relation</param>
        /// <exception cref="RelationExistsException">Throws if relation already exists</exception>
        public void CreateRelation(string relationName, Func<HashSet<Node>, Node> sourceNode, Func<HashSet<Node>, Node> targetNode, Dictionary<string, string> properties = null, HashSet<string> labels = null)
        {
            var sourceNodeObject = sourceNode.Invoke(Database.Nodes);
            var targetNodeObject = targetNode.Invoke(Database.Nodes);
            if (sourceNodeObject is null || targetNodeObject is null)
            {

            }
            bool exists = Relations.AsParallel().ToList().Exists(x =>
                x.TargetHash == targetNodeObject.Hash && x.SourceHash == sourceNodeObject.Hash);
            if (exists)
                throw new RelationExistsException(targetNodeObject.Hash, targetNodeObject.Hash);
            Dictionary<string, string> props = new Dictionary<string, string>();
            HashSet<string> lablSet = new HashSet<string>();
            var relation = new Relation(relationName, properties ?? props, labels ?? lablSet, sourceNodeObject.Hash,
                targetNodeObject.Hash);
            ValidateSchema(relation);
            Database.Relations.Add(relation);
            if (realtime) SaveDatabase();

        }

        /// <summary>
        /// Registers custom converter using TypeConversion library. <see cref="https://github.com/deniszykov/TypeConversion">More Info</see> />
        /// </summary>
        /// <typeparam name="FromTypeT">Type from</typeparam>
        /// <typeparam name="ToTypeT">Type to</typeparam>
        /// <param name="conversionFunc">conversion function</param>
        /// <param name="quality">quality of the conversion. Click more info in summary</param>
        public void RegisterConversion<FromTypeT, ToTypeT>(
            Func<FromTypeT, string, IFormatProvider, ToTypeT> conversionFunc,
            ConversionQuality quality)
        {
            typeConversionProvider.RegisterConversion(conversionFunc, quality);

        }
        /// <summary>
        /// Updates entities by inserting properties enforced by a given schema
        /// </summary>
        /// <param name="schema">Schema to enforce</param>
        private void UpdateEntitiesForSchema(Schema schema)
        {
            var nodesForSchema = Nodes.Where(x => x.Labels.Contains(schema.Label)) as IEnumerable<GraphEntity>;
            var relationsForSchema = Relations.Where(x => x.Labels.Contains(schema.Label)) as IEnumerable<GraphEntity>;
            var entities = new List<GraphEntity>();
            entities.AddRange(nodesForSchema);
            entities.AddRange(relationsForSchema);
            foreach (var entity in entities)
            {
                UpdateMissingFieldsForEntity(entity, schema);
            }

            BulkUpdate(entities);
        }
        /// <summary>
        /// Updates entity by filling in missing fields from schemas
        /// </summary>
        /// <param name="entity">entity to update</param>
        private void UpdateSchemasForEntity(GraphEntity entity)
        {
            var SchemasForEntity = Schemas.Where(s => entity.Labels.Contains(s.Label));
            foreach (var schema in SchemasForEntity)
            {
                UpdateMissingFieldsForEntity(entity, schema);
            }
        }
        /// <summary>
        /// enforces missing fields for entity given schema
        /// </summary>
        /// <param name="entity">entity to update</param>
        /// <param name="schema">schema to enforce</param>
        private void UpdateMissingFieldsForEntity(GraphEntity entity, Schema schema)
        {
            var missingFields = schema.Properties.Where(prop => !entity.Properties.ContainsKey(prop.Name));
            foreach (var missingField in missingFields)
            {
                entity.Properties.Add(missingField.Name, "");
            }
        }
        /// <summary>
        /// Attempts to fill object from properties
        /// </summary>
        /// <typeparam name="T">Type of the object to fill</typeparam>
        /// <param name="entity">entity, in which property values are stored</param>
        /// <param name="objectToFill">object that will be filled with property values</param>
        public void FillObjectFromProperties<T>(GraphEntity entity, T objectToFill)
        {
            var objectProperties = typeof(T).GetProperties();
            foreach (var property in objectProperties)
            {
                
                entity.Properties.TryGetValue(property.Name, out var propertyValue);
                if(typeConversionProvider.TryConvert(typeof(string), property.PropertyType, propertyValue, out var convertedValue))
                    property.SetValue(objectToFill, convertedValue);
            }
        }

        /// <summary>
        /// Attempts to fill object from properties
        /// </summary>
        /// <typeparam name="T">Type of the object to fill</typeparam>
        /// <param name="entity">entity, in which property values are stored</param>
        /// <param name="objectToFill">object that will be filled with property values</param>
        public Dictionary<string, string> GetPropertiesFromObject<T>(T objectToExtractValuesFrom)
        {
            var objectProperties = typeof(T).GetProperties();
            var result = new Dictionary<string, string>();
            foreach (var property in objectProperties)
            {

                result.Add(property.Name, property.GetValue(objectToExtractValuesFrom).ToString());
            }

            return result;
        }
        /// <summary>
        ///  Creates a Relation
        /// </summary>
        /// <typeparam name="T">Type of the inserted node</typeparam>
        /// <param name="value">object properties</param>
        public void CreateNode<T>(T value, params string[] additionalLabels)
        {
            var label = value.GetType().Name;
            if (!Schemas.Exists(s => s.Label == label))
            {
                CreateSchema(label,
                    value.GetType().GetProperties().Select(prop => new Property()
                        { FullTypeName = prop.PropertyType.FullName, Name = prop.Name }).ToList());
            }
            else
            {
                UpdateSchema(new Schema()
                {
                    Label = label,
                    Properties = value.GetType().GetProperties().Select(prop => new Property()
                        { FullTypeName = prop.PropertyType.FullName, Name = prop.Name }).ToList()
                });
            }
            Debug.WriteLine("Node created");
            var labels = new HashSet<string>(additionalLabels);
            labels.Add(label);
            CreateNode(GetPropertiesFromObject(value), labels);
        }

        /// <summary>
        /// Creates a Relation given an object. 
        /// </summary>
        /// <typeparam name="T">Type of the inserted node</typeparam>
        /// <param name="properties">object from which property values will be extracted</param>
        public void CreateRelation<T>(Func<HashSet<Node>, Node> sourceNode, Func<HashSet<Node>, Node> targetNode, T properties, params string[] additionalLabels)
        {
            var label = properties.GetType().Name;
            if (!Schemas.Exists(s => s.Label == label))
            {
                CreateSchema(label,
                    properties.GetType().GetProperties().Select(prop => new Property()
                        { FullTypeName = prop.PropertyType.FullName, Name = prop.Name }).ToList());
            }
            else
            {
                UpdateSchema(new Schema()
                {
                    Label = label,
                    Properties = properties.GetType().GetProperties().Select(prop => new Property()
                        { FullTypeName = prop.PropertyType.FullName, Name = prop.Name }).ToList()
                });
            }

            CreateRelation(label, sourceNode, targetNode, GetPropertiesFromObject(properties), new HashSet<string>(additionalLabels));
        }

        public void Update<T>(string hash, T objectValues)
        {
            var entity = this.Entities().FirstOrDefault(x => x.Hash == hash);
            var propertiesWithValues = GetPropertiesFromObject(objectValues);
            foreach (var propertyWithValue in propertiesWithValues)
            {
                if (entity.Properties.ContainsKey(propertyWithValue.Key))
                    entity.Properties[propertyWithValue.Key] = propertyWithValue.Value;
            }

            Update(entity);
        }

        public int Delete(string hash)
        {
            var nodes = Nodes.RemoveWhere(n => n.Hash == hash);
            var relationsLinkedWithNodes = Relations.RemoveWhere(r => r.SourceHash == hash || r.TargetHash == hash);
            var relations = Relations.RemoveWhere(r => r.Hash == hash);

            if(realtime) SaveDatabase();
            return nodes + relationsLinkedWithNodes + relations;
        }

    }
}