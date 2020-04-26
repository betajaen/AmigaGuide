using System.Collections;
using System.Collections.Generic;

namespace AmigaGuide
{
  public class NodeCollection : IEnumerable<Node>
  {
    private List<Node> Nodes;

    internal NodeCollection()
    {
      Nodes = new List<Node>();
    }

    public bool Has(string name)
    {
      foreach (var node in Nodes)
      {
        if (node.Name.Equals(name, System.StringComparison.InvariantCultureIgnoreCase))
          return true;
      }

      return false;
    }

    public Node this[string name]
    {
      get
      {
        foreach(var node in Nodes)
        {
          if (node.Name.Equals(name, System.StringComparison.InvariantCultureIgnoreCase))
            return node;
        }
        Node newNode = new Node(name);
        Nodes.Add(newNode);
        return newNode;
      }
    }

    public IEnumerator<Node> GetEnumerator()
    {
      yield return this["MAIN"];
      
      foreach(var node in Nodes)
      {
        if (node.Name.Equals("MAIN", System.StringComparison.InvariantCulture))
          continue;
        yield return node;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      yield return this["MAIN"];

      foreach (var node in Nodes)
      {
        if (node.Name.Equals("MAIN", System.StringComparison.InvariantCulture))
          continue;
        yield return node;
      }
    }


  }
}
