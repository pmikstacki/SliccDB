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
        var nodes = new HashSet<Node>();
        foreach (var node in db.Nodes)
            nodes.Add(node);
        return nodes;
    }

    public static HashSet<Relation> Relations(this DatabaseConnection db)
    {
        var edges = new HashSet<Relation>();
        foreach (var edge in db.Relations)
            edges.Add(edge);
        return edges;
    }

    public static HashSet<Relation> Relations(this DatabaseConnection db, string name)
    {
        var edges = new HashSet<Relation>();
        foreach (var edge in db.Relations.Where(x => x.RelationName == name))
            edges.Add(edge);
        return edges;
    }

    public static HashSet<GraphEntity> Entities(this DatabaseConnection db)
    {
        var entities = new HashSet<GraphEntity>();
        foreach (var entity in db.Nodes())
            entities.Add(entity);
        foreach (var entity in db.Relations())
            entities.Add(entity);
        return entities;
    }

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

    public static HashSet<GraphEntity> Labels(this HashSet<GraphEntity> allEntities, params string[] labels)
    {
        var entities = new HashSet<GraphEntity>();

        allEntities.AsParallel().ForAll(node =>
        {
            var commonLabels = node.Labels.Count(x => labels.Contains(x));
            if (commonLabels == node.Labels.Count)
                entities.Add(node);
        });

        return entities;
    }

    public static HashSet<GraphEntity> Properties(this HashSet<GraphEntity> allEntities, params KeyValuePair<string, string>[] properties)
    {
        var entities = new HashSet<GraphEntity>();
        allEntities.AsParallel().ForAll(entity =>
        {
            foreach (var keyValuePair in properties)
            {
                if (!entity.Properties.ContainsKey(keyValuePair.Key)) return;

                if (entity.Properties[keyValuePair.Key] == keyValuePair.Value)
                {
                    entities.Add(entity);
                }
            }
        });
        return entities;
    }

    public static HashSet<Node> Labels(this HashSet<Node> allEntities, params string[] labels)
    {
        var entities = new HashSet<Node>();

        allEntities.AsParallel().ForAll(node =>
        {
            var commonLabels = node.Labels.Count(x => labels.Contains(x));
            if (commonLabels == node.Labels.Count)
                entities.Add(node);
        });

        return entities;
    }

    public static HashSet<Node> Properties(this HashSet<Node> allEntities, params KeyValuePair<string, string>[] properties)
    {
        var entities = new HashSet<Node>();
        allEntities.AsParallel().ForAll(entity =>
        {
            foreach (var keyValuePair in properties)
            {
                if (!entity.Properties.ContainsKey(keyValuePair.Key)) return;

                if (entity.Properties[keyValuePair.Key] == keyValuePair.Value)
                {
                    entities.Add(entity);
                }
            }
        });
        return entities;
    }


    public static HashSet<Relation> Labels(this HashSet<Relation> allEntities, params string[] labels)
    {
        var entities = new HashSet<Relation>();

        allEntities.AsParallel().ForAll(node =>
        {
            var commonLabels = node.Labels.Count(x => labels.Contains(x));
            if (commonLabels == node.Labels.Count)
                entities.Add(node);
        });

        return entities;
    }

    public static HashSet<Relation> Properties(this HashSet<Relation> allEntities, params KeyValuePair<string, string>[] properties)
    {
        var entities = new HashSet<Relation>();
        allEntities.AsParallel().ForAll(entity =>
        {
            foreach (var keyValuePair in properties)
            {
                if (!entity.Properties.ContainsKey(keyValuePair.Key)) return;

                if (entity.Properties[keyValuePair.Key] == keyValuePair.Value)
                {
                    entities.Add(entity);
                }
            }
        });
        return entities;
    }

    public static KeyValuePair<string, string> Value(this string key, string value)
    {
        return new KeyValuePair<string, string>(key, value);
    }
}