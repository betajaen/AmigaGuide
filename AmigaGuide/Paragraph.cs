using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AmigaGuide
{
  public class Paragraph : IEnumerable<Run>
  {
    List<Run> Runs;

    public Alignment? Alignment { get; set; }

    public bool LineBreak { get; set; }

    internal Paragraph()
    {
      Runs = new List<Run>();
    }

    public Span Span()
    {
      Span span = new Span();
      Runs.Add(span);

      return span;
    }

    public Paragraph Span(string content)
    {
      Span span = new Span();
      span.Content = content;
      Runs.Add(span);

      return this;
    }

    public Paragraph Span(string content,  Colour? foreground, Colour? background, bool? bold, bool? italic, bool? underline)
    {
      Span span = new Span();
      span.Content = content;
      if (foreground.HasValue)
        span.Foreground = foreground;
      if (background.HasValue)
        span.Background = background;
      if (bold.HasValue)
        span.Bold = bold;
      if (italic.HasValue)
        span.Italic = italic;
      if (underline.HasValue)
        span.Underline = underline;

      Runs.Add(span);

      return this;
    }

    public Paragraph Link(Node node, string text, LinkFormat format = LinkFormat.Button)
    {
      Link link = new Link();
      link.Node = node;
      link.Title = text;
      link.Format = format;
      Runs.Add(link);

      return this;
    }

    internal void Save(Writer writer)
    {

      if (Alignment.HasValue)
      {
        writer.PushAlignment(Alignment.Value);
      }

      foreach(var run in Runs)
      {
        run.Save(writer);
      }

      if (Alignment.HasValue)
      {
        writer.PopAlignnent();
      }

      writer.NewLine();

      if (LineBreak)
      {
        writer.NewLine(true);
      }
    }

    public IEnumerator<Run> GetEnumerator()
    {
      return ((IEnumerable<Run>)Runs).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<Run>)Runs).GetEnumerator();
    }
  }
}
