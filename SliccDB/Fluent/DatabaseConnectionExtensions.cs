using System;
using System.Collections.Generic;
using System.Linq;
using SliccDB.Core;
using SliccDB.Serialization;

namespace SliccDB.Fluent;

public static class DatabaseConnectionExtensions
{
    public static HashSet<Node> Nodes(this DatabaseConnection db)
    {
        return db.Nodes;
    }
    /// <summary>
    /// Returns all nodes in given connection <see cref="db"/> that match the structure of <see cref="T"/>  
    /// </summary>
    /// <typeparam name="T"> Type used to match against properties in the <see cref="Node"/></typeparam>
    /// <param name="db"> Database connection which will be queried</param>
    /// <returns> A dictionary with ids and the nodes of type <see cref="T"/> themselves</returns>
    public static Dictionary<string, T> Nodes<T>(this DatabaseConnection db)
    {
        if (!db.Schemas.Exists(s => s.Label == typeof(T).Name))
            return new Dictionary<string, T>();

        var filledObjects = new Dictionary<string, T>();
        foreach (var node in db.Nodes().Where(x => x.Labels.Contains(typeof(T).Name)))
        {
            var newTypeInstance = (T) Activator.CreateInstance(typeof(T));

            db.FillObjectFromProperties(node, newTypeInstance);
            filledObjects.Add(node.Hash, newTypeInstance);
        }

        return filledObjects;
    }
    /// <summary>
    /// Returns all relations in given connection <see cref="db"/> that match the structure of <see cref="T"/>  
    /// </summary>
    /// <typeparam name="T"> Type used to match against properties in the <see cref="Relation"/></typeparam>
    /// <param name="db"> Database connection which will be queried</param>
    /// <returns> A dictionary with ids and the relations of type <see cref="T"/> themselves</returns>
    public static Dictionary<string, T> Relations<T>(this DatabaseConnection db)
    {
        if (!db.Schemas.Exists(s => s.Label == typeof(T).Name))
            return new Dictionary<string, T>();

        var filledObjects = new Dictionary<string, T>();
        foreach (var node in db.Relations().Where(x => x.RelationName == typeof(T).Name))
        {
            var newTypeInstance = (T) Activator.CreateInstance(typeof(T));

            db.FillObjectFromProperties(node, newTypeInstance);
            filledObjects.Add(node.Hash, (T)newTypeInstance);
        }

        return filledObjects;
    }
    /// <summary>
    /// Returns all entities in given connection <see cref="db"/> that match the structure of <see cref="T"/>  
    /// </summary>
    /// <typeparam name="T"> Type used to match against properties in the <see cref="GraphEntity"/></typeparam>
    /// <param name="db"> Database connection which will be queried</param>
    /// <returns> A dictionary with ids and the entities of type <see cref="T"/> themselves</returns>
    public static Dictionary<string, T> Entities<T>(this DatabaseConnection db)
    {
        if (!db.Schemas.Exists(s => s.Label == typeof(T).Name))
            return new Dictionary<string, T>();

        var filledObjects = new Dictionary<string, T>();
        foreach (var node in db.Entities().Where(x => x.Labels.Contains(typeof(T).Name)))
        {
            var newTypeInstance = (T) Activator.CreateInstance(typeof(T));

            db.FillObjectFromProperties(node, newTypeInstance);
            filledObjects.Add(node.Hash, (T)newTypeInstance);
        }

        return filledObjects;
    }

    /// <summary>
    /// Returns all relations in the database <see cref="db"/>
    /// </summary>
    /// <param name="db">Database Connection</param>
    /// <returns>All relations in the database </returns>
    public static HashSet<Relation> Relations(this DatabaseConnection db)
    {
        var edges = new HashSet<Relation>();
        foreach (var edge in db.Relations)
            edges.Add(edge);
        return edges;
    }
    /// <summary>
    /// Returns all relations in the database <see cref="db"/> with <see cref="name"/>
    /// </summary>
    /// <param name="db"></param>
    /// <param name="name"></param>
    /// <returns>All Relations with <see cref="name"/></returns>
    public static HashSet<Relation> Relations(this DatabaseConnection db, string name)
    {
        var edges = new HashSet<Relation>();
        foreach (var edge in db.Relations.Where(x => x.RelationName == name))
            edges.Add(edge);
        return edges;
    }
    /// <summary>
    /// Returns all entities in connection <see cref="db"/>
    /// </summary>
    /// <param name="db">connection</param>
    /// <returns>All Entities</returns>
    public static HashSet<GraphEntity> Entities(this DatabaseConnection db)
    {
        var entities = new HashSet<GraphEntity>();
        foreach (var entity in db.Nodes())
            entities.Add(entity);
        foreach (var entity in db.Relations())
            entities.Add(entity);
        return entities;
    }

    /// <summary>
    /// Replaces the node in the database <see cref="db"/>
    /// </summary>
    /// <param name="db">database</param>
    /// <param name="entity">entity to replace</param>
    /// <exception cref="InvalidOperationException">throws if node with given hash was not found</exception>
    public static void Replace(this HashSet<Node> db, Node entity)
    {
        var numRemoved = db.RemoveWhere(x => x.Hash == entity.Hash);
        if (numRemoved == 1)
            db.Add(entity);
        else
        {
            throw new InvalidOperationException("Attempted to replace a node that does not exist");
        }
    }
    /// <summary>
    /// Replaces the relation in the database <see cref="db"/>
    /// </summary>
    /// <param name="db">database</param>
    /// <param name="entity">entity to replace</param>
    /// <exception cref="InvalidOperationException">throws if relation with given hash was not found</exception>
    public static void Replace(this HashSet<Relation> db, Relation entity)
    {
        var numRemoved = db.RemoveWhere(x => x.Hash == entity.Hash);
        if (numRemoved == 1)
            db.Add(entity);
        else
        {
            throw new InvalidOperationException("Attempted to replace a relation that does not exist");
        }
    }
    /// <summary>
    /// Founds an entity with all labels
    /// </summary>
    /// <param name="allEntities">all entities to perform query onto</param>
    /// <param name="labels">all labels to query by</param>
    /// <returns></returns>
    public static HashSet<GraphEntity> Labels(this HashSet<GraphEntity> allEntities, params string[] labels)
    {
        var entities = new HashSet<GraphEntity>();

        foreach (var entity in allEntities)
        {
            var commonLabels = entity.Labels.Count(x => labels.Contains(x));
            if (commonLabels == entity.Labels.Count)
                entities.Add(entity);
        }

        return entities;
    }

    /// <summary>
    /// Finds entity with properties and their values
    /// </summary>
    /// <param name="allEntities">all entities to perform query onto</param>
    /// <param name="properties">all properties to query by</param>
    /// <returns></returns>
    public static HashSet<GraphEntity> Properties(this HashSet<GraphEntity> allEntities, params KeyValuePair<string, string>[] properties)
    {
        var entities = new HashSet<GraphEntity>();
        foreach(var entity in allEntities)
        {
            foreach (var keyValuePair in properties)
            {
                if (!entity.Properties.ContainsKey(keyValuePair.Key)) return null;

                if (entity.Properties[keyValuePair.Key] == keyValuePair.Value)
                {
                    entities.Add(entity);
                }
            }
        };
        return entities;
    }

    /// <summary>
    /// Finds entity with properties and their values
    /// </summary>
    /// <param name="allEntities">all entities to perform query onto</param>
    /// <param name="properties">all properties to query by</param>
    /// <returns></returns>
    public static HashSet<T> Labels<T>(this HashSet<T> allEntities, params string[] labels) where T : GraphEntity
    {
        var entities = new HashSet<T>();

        foreach (var entity in allEntities)
        {

            var commonLabels = entity.Labels.Count(x => labels.Contains(x));
            if (commonLabels == labels.Length)
                entities.Add(entity);

        }

        return entities;
    }

    /// <summary>
    /// Finds entities with properties 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="allEntities">all entities in database</param>
    /// <param name="properties">properties' names and values to match against</param>
    /// <returns></returns>
    public static HashSet<T> Properties<T>(this HashSet<T> allEntities,
        params KeyValuePair<string, string>[] properties) where T : GraphEntity
    {
        var entities = new HashSet<T>();
        foreach (var entity in allEntities)
        {
            foreach (var keyValuePair in properties)
            {
                if (entity.Properties.ContainsKey(keyValuePair.Key) 
                    && entity.Properties[keyValuePair.Key] == keyValuePair.Value)
                {
                    entities.Add(entity);
                }
            }
        }

        return entities;
    }
    /// <summary>
    /// Returns all nodes that have outgoing relation ([N]->) with given name
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="relationName">name of the relation that should be outgoing</param>
    /// <returns></returns>
    public static HashSet<Node> Out(this DatabaseConnection connection, string relationName)
    {
        var allRelationsWithName = connection.Relations(relationName);
        var entities = connection.Nodes().Where(n => allRelationsWithName.ToList().Exists(r => r.SourceHash == n.Hash));
        return new HashSet<Node>(entities);
    }

    /// <summary>
    /// Returns all nodes that have incoming relation (->[N]) with given name
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="relationName">name of the relation that should be incoming</param>
    /// <returns></returns>
    public static HashSet<Node> In(this DatabaseConnection connection, string relationName)
    {
        var allRelationsWithName = connection.Relations(relationName);
        var entities = connection.Nodes().Where(n => allRelationsWithName.ToList().Exists(r => r.TargetHash == n.Hash));
        return new HashSet<Node>(entities);
    }


    public static KeyValuePair<string, string> Value(this string key, string value)
    {
        return new KeyValuePair<string, string>(key, value);
    }


}