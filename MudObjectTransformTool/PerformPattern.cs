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
            if (Start.Value == "perform" || Start.Value == "check" || Start.Value == "value")
            {
                var originalStart = Start;

                var ruleType = Start.Value;
                Start = AdvanceAndSkipWhitespace(Start, 1);

                var ruleName = "";
                while (Start.Type != TokenType.EndOfFile && Start.Type != TokenType.OpenParen)
                {
                    ruleName += Start.Value;
                    Start = Advance(Start, 1);
                }

                ruleName = ruleName.Trim();

                var arguments = new List<Tuple<String, String>>();

                while (Start.Type != TokenType.EndOfFile && Start.Type == TokenType.OpenParen)
                {
                    Start = AdvanceAndSkipWhitespace(Start, 1);
                    if (Start.Type != TokenType.Token) return MatchResult.NoMatch;
                    var argumentName = Start.Value;
                    Start = AdvanceAndSkipWhitespace(Start, 1);
                    if (Start.Type != TokenType.Token) return MatchResult.NoMatch;
                    var argumentType = Start.Value;
                    Start = AdvanceAndSkipWhitespace(Start, 1);
                    if (Start.Type != TokenType.CloseParen) return MatchResult.NoMatch;

                    arguments.Add(Tuple.Create(argumentName, argumentType));
                    Start = AdvanceAndSkipWhitespace(Start, 1);
                }

                var resultType = "";
                if (ruleType == "value")
                {
                    if (Start.Type != TokenType.Token) return MatchResult.NoMatch;
                    resultType = Start.Value;
                    Start = AdvanceAndSkipWhitespace(Start, 1);
                }

                var clauseList = new List<Tuple<Token, Token>>();
                while (Start.Type != TokenType.EndOfFile && Start.Type != TokenType.SemiColon)
                {
                    if (Start.Type != TokenType.Token) return MatchResult.NoMatch;
                    if (Start.Value == "when" || Start.Value == "do")
                    {
                        var bodyClauseEnd = ExtractBodyClause(AdvanceAndSkipWhitespace(Start, 1));
                        clauseList.Add(Tuple.Create(Start, bodyClauseEnd));
                        Start = bodyClauseEnd;
                    }
                    else
                        return MatchResult.NoMatch;
                }

                Start = AdvanceAndSkipWhitespace(Start, 0);
                if (Start.Type != TokenType.SemiColon) return MatchResult.NoMatch;
                Start = Advance(Start, 1);

                var end = Start;
                var before = originalStart.Previous;

                //Splice out the middle bit
                if (before != null) before.Next = end;
                if (end != null) end.Previous = before;

                if (ruleType == "perform") before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "Perform"));
                if (ruleType == "check") before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "Check"));
                if (ruleType == "value") before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "Value"));

                if (ruleType == "value")
                    before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "<" + String.Join(", ", arguments.Select(t => t.Item1)) + ", " + resultType + ">(\"" + ruleName + "\")"));
                else
                    before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "<" + String.Join(", ", arguments.Select(t => t.Item1)) + ">(\"" + ruleName + "\")"));

                var build = originalStart.Next;

                foreach (var clause in clauseList)
                {
                    if (clause.Item1.Value == "when")
                    {
                        before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "\n.When((" + String.Join(", ", arguments.Select(t => t.Item2)) + ") => "));

                        var rest = before.Next;
                        before.Next = Advance(clause.Item1, 1);
                        var lastTokenInBody = (clause.Item2 == null ? null : clause.Item2.Previous);
                        if (lastTokenInBody != null) lastTokenInBody.Next = rest;
                        if (rest != null) rest.Previous = lastTokenInBody;

                        before = InsertAfter(lastTokenInBody, Token.Create(TokenType.GeneratedBlock, ")"));
                    }
                    else if (clause.Item1.Value == "do")
                    {
                        before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "\n.Do((" + String.Join(", ", arguments.Select(t => t.Item2)) + ") => "));

                        var rest = before.Next;
                        before.Next = Advance(clause.Item1, 1);
                        var lastTokenInBody = (clause.Item2 == null ? null : clause.Item2.Previous);
                        if (lastTokenInBody != null) lastTokenInBody.Next = rest;
                        if (rest != null) rest.Previous = lastTokenInBody;

                        before = InsertAfter(lastTokenInBody, Token.Create(TokenType.GeneratedBlock, ")"));
                    }
                }

                before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, ";"));
                return MatchResult.Create(originalStart);
            }
            else
                return MatchResult.NoMatch;
        }

        private Token ExtractBodyClause(Token Start)
        {
            var end = Start;

            while (end.Type != TokenType.EndOfFile && end.Type != TokenType.SemiColon && end.Value != "when" && end.Value != "do")
            {
                if (end.Type == TokenType.OpenBrace || end.Type == TokenType.OpenBracket || end.Type == TokenType.OpenParen)
                    end = MatchBracket(end);
                else
                    end = Advance(end, 1);
            }

            return end;
        }

        private Token MatchBracket(Token Start)
        {
            var closeType = TokenType.CloseParen;
            if (Start.Type == TokenType.OpenBrace) closeType = TokenType.CloseBrace;
            else if (Start.Type == TokenType.OpenBracket) closeType = TokenType.CloseBracket;
            else if (Start.Type == TokenType.OpenParen) closeType = TokenType.CloseParen;

            Start = Advance(Start, 1);
            while (Start.Type != TokenType.EndOfFile && Start.Type != closeType)
            {
                if (Start.Type == TokenType.OpenBrace || Start.Type == TokenType.OpenBracket || Start.Type == TokenType.OpenParen)
                    Start = MatchBracket(Start);
                else
                    Start = Advance(Start, 1);
            }
            return Advance(Start, 1);
        }
    }
}
