using System;
using System.Collections.Generic;
using System.Text;

namespace AmigaGuide.Builders
{
  public class Main : IBuilder
  {
    public int Order => 0;

    public void Build(Database database)
    {
      var main = database.Nodes["MAIN"];

      if (main.IsClear() == false)
        return;

      main.Title = database.Name;
      
      main.Paragraph().Span($"{database.Name}", null, null, true, false, true);
      
      if (string.IsNullOrWhiteSpace(database.Description) == false)
      {
        main.Paragraph().Span($"{database.Description}", null, null, null, true, null);
      }

      if (string.IsNullOrWhiteSpace(database.Author) == false)
      {
        main.Paragraph($"Written by {database.Author} ({database.Date.Year}-{database.Date.Month}-{database.Date.Day})");
      }

      bool hasToc = database.HasBuilder<TableOfContents>();
      bool hasVersion = database.HasBuilder<Version>();
      
      main.Paragraph().LineBreak = true;
      main.Paragraph("Use the Next and Previous buttons to browse this guide.", true);

      if (hasToc || hasVersion)
      {
        main.Paragraph("Interesting Pages:", true);
      
        if (hasToc)
        {
          var p = main.Paragraph();
          p.Span("  ");
          p.Link(database.Nodes["TOC"], "Table of Contents", LinkFormat.RightOf);
        }

        if (hasVersion)
        {
          var p = main.Paragraph();
          p.Span("  ");
          p.Link(database.Nodes["VERSION"], "Version", LinkFormat.RightOf);
        }
      }

    }

  }
}
