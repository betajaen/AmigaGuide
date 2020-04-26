using System;
using System.Collections.Generic;
using System.Text;

namespace AmigaGuide
{
  public class Node
  {
    public string Name { get; set; }
    public string Title { get; set; }
    public Node Next { get; set; }

    public Node Prev { get; set; }

    List<Paragraph> Paragraphs;


    internal Node(string name)
    {
      Name = name;
      Paragraphs = new List<Paragraph>();
    }

    internal void Save(Writer writer)
    {
      writer.NewLine(true);

      writer.Node(Name, Title, Next?.Name ?? string.Empty, Prev?.Name ?? string.Empty);

      foreach(var para in Paragraphs)
      {
        para.Save(writer);
      }

      writer.EndNode();
    }

    public bool IsClear()
    {
      return Paragraphs.Count == 0;
    }

    public void Clear()
    {
      Paragraphs.Clear();
    }

    public Paragraph Paragraph()
    {
      var paragraph = new Paragraph();
      Paragraphs.Add(paragraph);
      return paragraph;
    }

    public Node Paragraph(string spanText, bool lineBreak = false)
    {
      var paragraph = new Paragraph();
      paragraph.Span(spanText);
      paragraph.LineBreak = lineBreak;
      Paragraphs.Add(paragraph);
      return this;
    }

  }
}
