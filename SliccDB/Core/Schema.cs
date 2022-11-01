using System;
using MessagePack;
using System.Collections.Generic;

namespace SliccDB.Core;
/// <summary>
/// Represents schema with needed properties
/// </summary>
[MessagePackObject]
public class Schema
{

    [Key(0)]
    public virtual string Label { get; set; }

    [Key(2)]
    public List<Property> Properties { get; set; } = new List<Property>();
}

[MessagePackObject]
public class Property
{

    [Key(0)]
    public string Name { get; set; }

    [Key(2)]
    public string FullTypeName { get; set; }

    public Property()
    {

    }

    public Property(string name, Type type)
    {
        Name = name;
        FullTypeName = type.FullName;
    }

}
