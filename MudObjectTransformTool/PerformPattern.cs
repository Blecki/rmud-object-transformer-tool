using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudObjectTransformTool
{
    public class PerformPattern : Pattern
    {
        public override MatchResult Match(Token Start)
        {
            var originalStart = Start;
            if (!Matches(MToken("perform"), Start)) return MatchResult.NoMatch;

            Start = AdvanceAndSkipWhitespace(Start, 1);

            var ruleName = "";
            while (Start != null && Start.Type != TokenType.OpenParen)
            {
                ruleName += Start.Value;
                Start = Advance(Start, 1);
            }

            ruleName = ruleName.Trim();

            var arguments = new List<Tuple<String, String>>();

            while (Start != null && Start.Type == TokenType.OpenParen)
            {
                Start = AdvanceAndSkipWhitespace(Start, 1);
                if (Start == null || Start.Type != TokenType.Token) return MatchResult.NoMatch;
                var argumentName = Start.Value;
                Start = AdvanceAndSkipWhitespace(Start, 1);
                if (Start == null || Start.Type != TokenType.Token) return MatchResult.NoMatch;
                var argumentType = Start.Value;
                Start = AdvanceAndSkipWhitespace(Start, 1);
                if (Start == null || Start.Type != TokenType.CloseParen) return MatchResult.NoMatch;

                arguments.Add(Tuple.Create(argumentName, argumentType));
                Start = AdvanceAndSkipWhitespace(Start, 1);
            }

            originalStart.Next = Start;
            originalStart.Value = "Perform<" + String.Join(", ", arguments.Select(t => t.Item2)) + ">(\"" + ruleName + "\")";

            return MatchResult.Create(originalStart.Next);
        }
    }
}
