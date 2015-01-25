using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudObjectTransformer
{
    public class Span
    {
        private static Token Terminal = new Token { Type = TokenType.EndOfFile };
        public Token Start = Terminal;
        public Token End = Terminal;

        public void Append(Token T)
        {
            if (Start == End)
            {
                Start = T;
                Start.Next = End;
                End.Previous = Start;
            }
            else
            {
                End.Previous.Next = T;
                T.Previous = End.Previous;
                T.Next = End;
                End.Previous = T;
            }
        }

        public void Append(Span S)
        {
            if (Start == End)
            {
                Start = S.Start;
                End = S.End;
            }
            else
            {
                End.Previous.Next = S.Start;
                S.Start.Previous = End.Previous;
                End = S.End;
            }
        }

        public static Span Trim(Span S)
        {
            var start = S.Start;
            var end = S.End;
            while (start.Type == TokenType.Whitespace && start != end)
                start = start.Next;
            while (end.Previous != start && end.Previous.Type == TokenType.Whitespace)
                end = end.Previous;
            return new Span { Start = start, End = end };
        }
    }

    public partial class Pattern
    {
        public static Span MakeSpan(params Object[] args)
        {
            var result = new Span();
            foreach (var arg in args)
            {
                if (arg is Token) result.Append(arg as Token);
                else if (arg is Span) result.Append(arg as Span);
                else if (arg is String) result.Append(new Token { Type = TokenType.GeneratedBlock, Value = arg as String });
                else throw new InvalidOperationException();
            }
            return result;
        }

        public static Span AggregateSpan(List<Span> List, Func<Span, Span> Function)
        {
            var result = new Span();
            foreach (var t in List) result.Append(Function(t));
            return result;
        }

        public static Span Replace(Span What, Span With)
        {
            var before = What.Start.Previous;
            var after = What.End;

            before.Next = With.Start;
            With.Start.Previous = before;

            after.Previous = With.End.Previous;
            after.Previous.Next = after;

            return With;
        }

        public static String SpanAsString(Span Span)
        {
            var start = Span.Start;
            var r = "";
            while (start != Span.End)
            {
                r += start.Value;
                start = start.Next;
            }
            return r;
        }
    }
}
