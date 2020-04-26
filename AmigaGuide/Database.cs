using System;
using System.Collections.Generic;
using System.IO;

namespace AmigaGuide
{

  public class Database
  {
    public NodeCollection Nodes { get; private set; }

    public string Author { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public Version Version { get; set; }

    public DateTimeOffset Date { get; set; }


    List<Builders.IBuilder> Builders;

    public Database()
    {
      Builders = new List<Builders.IBuilder>();
      Nodes = new NodeCollection();
      Date = DateTimeOffset.Now;
      Version = new Version(1, 0);
      Name = "Document";
      Description = string.Empty;
    }

    public void AddBuilder<T>() where T : Builders.IBuilder, new()
    {
      T builder = new T();
      Builders.Add(builder);
      Builders.Sort( (x,y) => x.Order.CompareTo(y.Order));
    }

    public bool HasBuilder<T>() where T : Builders.IBuilder
    {
      foreach(var builder in Builders)
      {
        if (builder is T)
          return true;
      }
      return false;
    }

    public void Save(Stream stream)
    {

      foreach(var builder in Builders)
      {
        builder.Build(this);
      }

      using (var binaryWriter = new BinaryWriter(stream))
      {
        Writer writer = new Writer(binaryWriter);

        if (string.IsNullOrWhiteSpace(Name) == false &&
            Version != null
          )
        {
          writer.Version(Name, Version, Date);
        }

        if (string.IsNullOrWhiteSpace(Author) == false)
        {
          writer.Author(Author);
        }

        foreach(var node in Nodes)
        {
          node.Save(writer);
        }
      }
    }
  }
}
