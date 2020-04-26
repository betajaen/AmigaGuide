using System;
using System.Collections.Generic;
using System.Text;

namespace AmigaGuide.Builders
{
  public interface IBuilder
  {
    int Order { get; }
    void Build(Database database);
  }
}
