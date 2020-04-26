using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AmigaGuide
{

  internal class Writer
  {
    internal static class Ascii
    {
      internal const byte NewLine = 0xA;
      internal const byte At = 0x40;
      internal const byte Space = 0x20;
      internal const byte Backslash = 0x5C;
      internal const byte DoubleQuote = 0x22;
      internal const byte LeftCurlyBracket = 0x7B;
      internal const byte RightCurlyBracket = 0x7D;
    }

    BinaryWriter BinaryWriter;

    List<Colour>    ForegroundStack;
    List<Colour>    BackgroundStack;
    List<bool>      BoldStack;
    List<bool>      ItalicStack;
    List<bool>      UnderlineStack;
    List<Alignment> AlignmentStack;
    string          CurrentNode;
    byte            LastChar;


    private Alignment Alignment
    {
      get
      {
        int count = AlignmentStack.Count;

        if (count == 0)
        {
          return Alignment.Left;
        }
        else
        {
          return AlignmentStack[count - 1];
        }
      }
    }

    private Colour Foreground
    {
      get
      {
        int count = ForegroundStack.Count;

        if (count == 0)
        {
          return Colour.Text;
        }
        else
        {
          return ForegroundStack[count - 1];
        }
      }
    }

    private Colour Background
    {
      get
      {
        int count = BackgroundStack.Count;

        if (count == 0)
        {
          return Colour.Background;
        }
        else
        {
          return BackgroundStack[count - 1];
        }
      }
    }

    private bool Bold
    {
      get
      {
        int count = BoldStack.Count;

        if (count == 0)
        {
          return false;
        }
        else
        {
          return BoldStack[count - 1];
        }
      }
    }

    private bool Italic
    {
      get
      {
        int count = ItalicStack.Count;

        if (count == 0)
        {
          return false;
        }
        else
        {
          return ItalicStack[count - 1];
        }
      }
    }

    private bool Underline
    {
      get
      {
        int count = UnderlineStack.Count;

        if (count == 0)
        {
          return false;
        }
        else
        {
          return UnderlineStack[count - 1];
        }
      }
    }

    internal Writer(BinaryWriter writer)
    {
      BinaryWriter = writer;
      ForegroundStack = new List<Colour>();
      BackgroundStack = new List<Colour>();
      BoldStack = new List<bool>();
      ItalicStack = new List<bool>();
      UnderlineStack = new List<bool>();
      AlignmentStack = new List<Alignment>();
      CurrentNode = string.Empty;

      Emit(Ascii.At);
      Emit("DATABASE");

      LastChar = 0;
    }

    void Emit(byte ch)
    {
      LastChar = ch;
      BinaryWriter.Write(ch);
    }

    internal void NewLine(bool force = false)
    {
      if (LastChar != Ascii.NewLine || force == true)
      {
        Emit(Ascii.NewLine);
      }
    }

    void Emit(Alignment m)
    {
      switch(m)
      {
        case Alignment.Left:
          Emit("@{JLEFT}");
        break;
        case Alignment.Center:
          Emit("@{JCENTER}");
          break;
        case Alignment.Right:
          Emit("@{JRIGHT}");
          break;
      }
    }

    void EmitBold(bool hasBold)
    {
      if (hasBold)
        Emit("@{B}");
      else
        Emit("@{UB}");
    }

    void EmitItalic(bool hasItalic)
    {
      if (hasItalic)
        Emit("@{I}");
      else
        Emit("@{UI}");
    }

    void EmitUnderline(bool hasUnderline)
    {
      if (hasUnderline)
        Emit("@{U}");
      else
        Emit("@{UU}");
    }

    void Emit(Colour colour, bool isForeground)
    {
      if (isForeground)
      {
        Emit($"@{{FG {colour}}}");
      }
      else
      {
        Emit($"@{{BG {colour}}}");
      }
    }

    void Emit(string syntax)
    {
      foreach(var ch in syntax)
      {
        byte b = (byte)ch;
        Emit(b);
      }
    }


    void Identifier(string name)
    {
      foreach (var ch in name)
      {
        if (char.IsLetterOrDigit(ch) || ch == '_' || ch == '.')
        {
          byte b = (byte) ch;
          Emit(b);
        }
      }
    }

    void NodeName(string name)
    {
      foreach(var ch in name)
      {
        if (char.IsLetterOrDigit(ch) || ch == '_')
        {
          byte b = (byte) char.ToUpper(ch);
          Emit(b);
        }
      }
    }

    internal void CharEscaped(char ch, bool newLinesAllowed)
    {
      if (char.IsControl(ch))
      {
        if (ch == '\n' && newLinesAllowed)
        {
          Emit(Ascii.NewLine);
        }
      }
      else if (ch == '@')
      {
        Emit(Ascii.Backslash);
        Emit(Ascii.At);
      }
      else if (ch < 127)
      {
        byte b = (byte) ch;
        Emit(b);
      }
    }


    internal void Text(string text, bool newLinesAllowed)
    {
      foreach(var ch in text)
      {
        CharEscaped(ch, newLinesAllowed);
      }
    }

    internal void Author(string author)
    {
      if (string.IsNullOrWhiteSpace(CurrentNode) == false)
      {
        throw new Exception("Node is already open with " + CurrentNode);
      }

      NewLine();
      Emit(Ascii.At);
      Emit("AUTHOR");
      Emit(Ascii.Space);
      Emit(Ascii.DoubleQuote);
      Text(author, false);
      Emit(Ascii.DoubleQuote);
      NewLine();

    }

    internal void Version(string name, Version version, DateTimeOffset date)
    {
      if (string.IsNullOrWhiteSpace(CurrentNode) == false)
      {
        throw new Exception("Node is already open with " + CurrentNode);
      }

      NewLine();
      Emit(Ascii.At);
      Emit("$VER:");
      Emit(Ascii.Space);
      Identifier(name);
      Emit(Ascii.Space);
      Identifier(version.ToString());
      Emit(Ascii.Space);
      Emit($"({date.Year}-{date.Month}-{date.Day})");
      NewLine();

    }

    internal void Node(string node, string title, string next, string prev)
    {
      if (string.IsNullOrWhiteSpace(CurrentNode) == false)
      {
        throw new Exception("Node is already open with " + CurrentNode);
      }

      NewLine();
      Emit(Ascii.At);
      Emit("NODE");
      Emit(Ascii.Space);
      NodeName(node);
      Emit(Ascii.Space);
      Emit(Ascii.DoubleQuote);
      if (string.IsNullOrWhiteSpace(title))
      {
        Text(node, false);
      }
      else
      {
        Text(title, false);
      }
      Emit(Ascii.DoubleQuote);
      NewLine();

      if (string.IsNullOrWhiteSpace(next) == false)
      {
        Emit(Ascii.At);
        Emit("PREV");
        Emit(Ascii.Space);
        NodeName(prev);
        NewLine();
      }

      if (string.IsNullOrWhiteSpace(next) == false)
      {
        Emit(Ascii.At);
        Emit("NEXT");
        Emit(Ascii.Space);
        NodeName(next);
        NewLine();
      }

      CurrentNode = node;
    }

    internal void EndNode()
    {
      if (string.IsNullOrWhiteSpace(CurrentNode))
      {
        throw new Exception("Node is not open!");
      }

      NewLine();
      Emit(Ascii.At);
      Emit("ENDNODE");
      NewLine();

      CurrentNode = string.Empty;
    }


    internal void Link(string node, string text, LinkFormat format, int? lineNo)
    {
      if (string.IsNullOrWhiteSpace(CurrentNode))
      {
        throw new Exception("Node is not open!");
      }

      Emit(Ascii.At);
      Emit(Ascii.LeftCurlyBracket);

      Emit(Ascii.DoubleQuote);

      switch(format)
      {
        case LinkFormat.Button:
        {
            Text(text, false);
        }
        break;

        case LinkFormat.ButtonBig:
        {
            Text(text.PadRight(20, ' '), false);
        }
        break;

        case LinkFormat.RightOf:
          {
            Text("  ", false);
          }
        break;
      }

      Emit(Ascii.DoubleQuote);

      Emit(Ascii.Space);
      Emit("LINK");
      Emit(Ascii.Space);

      Emit(Ascii.DoubleQuote);
      NodeName(node);
      Emit(Ascii.DoubleQuote);

      if (lineNo.HasValue)
      {
        Emit(Ascii.Space); // space
        Identifier(lineNo.Value.ToString());
      }

      Emit(Ascii.RightCurlyBracket);

      if (format == LinkFormat.RightOf)
      {
        Emit(Ascii.Space);
        Emit(Ascii.Space);
        Text(text, false);
      }

    }

    internal void PushAlignment(Alignment alignment)
    {
      if (string.IsNullOrWhiteSpace(CurrentNode))
      {
        throw new Exception("Node is not open!");
      }

      Alignment current = this.Alignment;
      AlignmentStack.Add(alignment);

      if (current != Alignment)
      {
        Emit(alignment);
      }

    }


    internal void PushForeground(Colour colour)
    {
      if (string.IsNullOrWhiteSpace(CurrentNode))
      {
        throw new Exception("Node is not open!");
      }

      Colour current = this.Foreground;
      ForegroundStack.Add(colour);

      if (current != colour)
      {
        Emit(colour, true);
      }
    }

    internal void PushBackground(Colour colour)
    {
      if (string.IsNullOrWhiteSpace(CurrentNode))
      {
        throw new Exception("Node is not open!");
      }

      Colour current = this.Background;
      BackgroundStack.Add(colour);

      if (current != colour)
      {
        Emit(colour, false);
      }
    }

    internal void PushBold(bool enabled)
    {
      if (string.IsNullOrWhiteSpace(CurrentNode))
      {
        throw new Exception("Node is not open!");
      }

      bool current = this.Bold;
      BoldStack.Add(enabled);

      if (current != enabled)
      {
        EmitBold(enabled);
      }

    }

    internal void PushItalic(bool enabled)
    {
      if (string.IsNullOrWhiteSpace(CurrentNode))
      {
        throw new Exception("Node is not open!");
      }

      bool current = this.Italic;
      ItalicStack.Add(enabled);

      if (current != enabled)
      {
        EmitItalic(enabled);
      }
    }

    internal void PushUnderline(bool enabled)
    {
      if (string.IsNullOrWhiteSpace(CurrentNode))
      {
        throw new Exception("Node is not open!");
      }

      bool current = this.Underline;
      UnderlineStack.Add(enabled);

      if (current != enabled)
      {
        EmitUnderline(enabled);
      }
    }

    internal void PopAlignnent()
    {
      if (string.IsNullOrWhiteSpace(CurrentNode))
      {
        throw new Exception("Node is not open!");
      }

      Alignment was = this.Alignment;
      
      if (AlignmentStack.Count > 0)
      {
        AlignmentStack.RemoveAt(AlignmentStack.Count - 1);
      }

      Alignment now = this.Alignment;

      if (was != now)
      {
        Emit(now);
      }


    }

    internal void PopForeground()
    {
      if (string.IsNullOrWhiteSpace(CurrentNode))
      {
        throw new Exception("Node is not open!");
      }

      Colour was = this.Foreground;

      if (ForegroundStack.Count > 0)
      {
        ForegroundStack.RemoveAt(ForegroundStack.Count - 1);
      }

      Colour now = this.Foreground;

      if (was != now)
      {
        Emit(now, true);
      }
    }

    internal void PopBackground()
    {
      if (string.IsNullOrWhiteSpace(CurrentNode))
      {
        throw new Exception("Node is not open!");
      }

      Colour was = this.Foreground;

      if (BackgroundStack.Count > 0)
      {
        BackgroundStack.RemoveAt(BackgroundStack.Count - 1);
      }

      Colour now = this.Background;

      if (was != now)
      {
        Emit(now, false);
      }
    }

    internal void PopBold()
    {
      if (string.IsNullOrWhiteSpace(CurrentNode))
      {
        throw new Exception("Node is not open!");
      }

      bool was = this.Bold;

      if (BoldStack.Count > 0)
      {
        BoldStack.RemoveAt(BoldStack.Count - 1);
      }

      bool now = this.Bold;

      if (was != now)
      {
        EmitBold(now);
      }
    }

    internal void PopItalic()
    {
      if (string.IsNullOrWhiteSpace(CurrentNode))
      {
        throw new Exception("Node is not open!");
      }

      bool was = this.Italic;

      if (ItalicStack.Count > 0)
      {
        ItalicStack.RemoveAt(ItalicStack.Count - 1);
      }

      bool now = this.Italic;

      if (was != now)
      {
        EmitItalic(now);
      }
    }

    internal void PopUnderline()
    {
      if (string.IsNullOrWhiteSpace(CurrentNode))
      {
        throw new Exception("Node is not open!");
      }

      bool was = this.Underline;

      if (UnderlineStack.Count > 0)
      {
        UnderlineStack.RemoveAt(UnderlineStack.Count - 1);
      }

      bool now = this.Underline;

      if (was != now)
      {
        EmitUnderline(now);
      }
    }

  }
}
