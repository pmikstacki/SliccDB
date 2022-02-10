using System;
using System.Collections.Generic;
using System.Linq;
using SliccDB.Core;

namespace SliccDB.Fluent
{
    public static class DatabaseExtensions
    {
        public static HashSet<GraphEntity> Nodes(this Database db)
        {
            var nodes = new HashSet<GraphEntity>();
            foreach (var node in db.Nodes)
                nodes.Add(node);
            return nodes;
        }

        public static HashSet<GraphEntity> Edges(this Database db)
        {
            var edges = new HashSet<GraphEntity>();
            foreach (var edge in db.Edges())
                edges.Add(edge);
            return edges;
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

        public static KeyValuePair<string, string> Value(this string key, string value)
        {
            return new KeyValuePair<string, string>(key, value);
        }
    }
}
