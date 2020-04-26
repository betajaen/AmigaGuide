using System;
using System.Collections.Generic;
using System.Text;

namespace AmigaGuide
{
  public class Span : Run
  {
    public Colour? Foreground { get; set; }

    public Colour? Background { get; set; }

    public bool? Bold { get; set; }

    public bool? Italic { get; set; }

    public bool? Underline { get; set; }

    public string Content { get; set; }

    void Run.Save(Writer writer)
    {
      if (string.IsNullOrEmpty(Content))
        return;

      if (Foreground.HasValue)
        writer.PushForeground(Foreground.Value);

      if (Background.HasValue)
        writer.PushBackground(Background.Value);

      if (Bold.HasValue)
        writer.PushBold(Bold.Value);

      if (Italic.HasValue)
        writer.PushItalic(Italic.Value);

      if (Underline.HasValue)
        writer.PushUnderline(Underline.Value);

      writer.Text(Content, false);

      if (Underline.HasValue)
        writer.PopUnderline();

      if (Italic.HasValue)
        writer.PopItalic();

      if (Bold.HasValue)
        writer.PopBold();

      if (Background.HasValue)
        writer.PopBackground();

      if (Foreground.HasValue)
        writer.PopForeground();
    }
  }
}
