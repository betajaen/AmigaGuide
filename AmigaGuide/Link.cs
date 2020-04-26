using System;
using System.Collections.Generic;
using System.Text;

namespace AmigaGuide
{

  public enum LinkFormat
  {
    Button,
    ButtonBig,
    RightOf,
  }

  public class Link : Run
  {
    public LinkFormat Format {get; set; }

    public Node Node { get; set; }
    public string Title { get; set; }

    void Run.Save(Writer writer)
    {
      writer.Link(Node.Name, Title ?? Node.Name, Format, null);
    }
  }
}
