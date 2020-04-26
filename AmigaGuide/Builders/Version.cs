using System;
using System.Collections.Generic;
using System.Text;

namespace AmigaGuide.Builders
{
  public class Version : IBuilder
  {
    public int Order => 0;

    public void Build(Database database)
    {
      var version = database.Nodes["VERSION"];

      version.Title = "Version";

      var header = version.Paragraph().Span();
      header.Content = "Version";
      header.Underline = true;

      version.Paragraph().LineBreak = true;

      version.Paragraph().Span($"{database.Name} {database.Version} {database.Date.Year}-{database.Date.Month}-{database.Date.Year}");
    }
  }
}
