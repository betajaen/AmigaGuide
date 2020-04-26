using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmigaGuide.Builders
{
  public class TableOfContents : IBuilder
  {
    public int Order => 100000;

    public void Build(Database database)
    {
      Node toc = database.Nodes["TOC"];
      toc.Clear();

      toc.Title = "Table of Contents";
      var p = toc.Paragraph();
      p.Span("Table of Contents", null, null, true, false, true);
      p.LineBreak = true;

      char lastLetter = "A"[0];

      foreach (var node in database.Nodes.OrderBy(x => x.Title))
      {
        if (node.Name != "TOC")
        {
          string name = node.Title ?? node.Name;

          char firstLetter = char.ToUpper(name[0]);

          if (firstLetter != lastLetter)
          {
            lastLetter = firstLetter;

            var letterPara = toc.Paragraph();
            letterPara.LineBreak = true;

            var letterSpan = letterPara.Span();
            letterSpan.Content = firstLetter.ToString();
            letterSpan.Bold = true;
          }

          var itemPara = toc.Paragraph();

          itemPara.Span("  ");
          itemPara.Link(node, node.Title, LinkFormat.ButtonBig);
        }
      }
    }
  }
}
