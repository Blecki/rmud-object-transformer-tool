using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudObjectTransformer
{
    public class RuleBodyClauseTag { }
    public class RuleBodyClauseHeadTag { }

    public class RulePattern : Pattern
    {
        public RulePattern()
        {
            StandardRules.InitializeStandardRules();
        }

        public override PatternMatch Match(Token Start)
        {
            //Reject obvious mismatches early.
            if (Start.Value.ToUpper() == "PERFORM" || Start.Value.ToUpper() == "CHECK" || Start.Value.ToUpper() == "VALUE" || Start.Value.ToUpper() == "FIRST" || Start.Value.ToUpper() == "LAST" || Start.Value.ToUpper() == "GLOBAL")
            {
                var originalStart = Start;
                var clauseList = new List<Tuple<Token, Token>>();
                bool globalRule = false;

                //Detect first/last/global modifiers
                while (Start.Value.ToUpper() == "FIRST" || Start.Value.ToUpper() == "LAST" || Start.Value.ToUpper() == "GLOBAL")
                {
                    if (Start.Value.ToUpper() == "FIRST" || Start.Value.ToUpper() == "LAST")
                        clauseList.Add(Tuple.Create<Token, Token>(Start, null));
                    else if (Start.Value.ToUpper() == "GLOBAL")
                        globalRule = true;

                    Start = AdvanceAndSkipWhitespace(Start, 1);
                }

                var ruleType = Start.Value.ToUpper();
                Start = AdvanceAndSkipWhitespace(Start, 1);

                //Extract the name of the rulebook - whitespace between words matters!
                var ruleName = "";
                while (Start.Type != TokenType.EndOfFile && Start.Type != TokenType.OpenParen && Start.Value.ToUpper() != "WHEN" && Start.Value.ToUpper() != "DO" && Start.Type != TokenType.SemiColon)
                {
                    ruleName += Start.Value;
                    Start = Advance(Start, 1);
                }
                ruleName = ruleName.Trim();

                //Detect a list of (paranthetical) arguments.
                var arguments = new List<RuleArgument>();

                while (Start.Type != TokenType.EndOfFile && Start.Type == TokenType.OpenParen)
                {
                    Start = AdvanceAndSkipWhitespace(Start, 1);
                    //An empty argument - () - is allowed. It's required for value rules that take 0 arguments.
                    if (Start.Type == TokenType.CloseParen)
                    {
                        Start = AdvanceAndSkipWhitespace(Start, 1);
                        continue;
                    }

                    if (Start.Type != TokenType.Token) return PatternMatch.NoMatch;
                    var argumentType = Start.Value;
                    Start = AdvanceAndSkipWhitespace(Start, 1);
                    if (Start.Type != TokenType.Token) return PatternMatch.NoMatch;
                    var argumentName = Start.Value;
                    Start = AdvanceAndSkipWhitespace(Start, 1);
                    if (Start.Type != TokenType.CloseParen) return PatternMatch.NoMatch;

                    arguments.Add(new RuleArgument { DeclarationType = argumentType, Name = argumentName });
                    Start = AdvanceAndSkipWhitespace(Start, 1);
                }

                bool usingStandardArguments = false;
                if (StandardRules.RuleDefined(ruleName) && arguments.Count == 0)
                {
                    arguments = StandardRules.RuleArguments(ruleName);
                    usingStandardArguments = true;
                }

                //For value rules only, a result type must appear immediately after the last argument.
                var resultType = "";
                if (ruleType == "VALUE")
                {
                    if (usingStandardArguments)
                    {
                        resultType = StandardRules.RuleResultType(ruleName);
                    }
                    else
                    {
                        if (Start.Type != TokenType.Token) return PatternMatch.NoMatch;
                        resultType = Start.Value;
                        Start = AdvanceAndSkipWhitespace(Start, 1);
                    }
                }

                //Detect when and do clauses. They can appear in any order.
                while (Start.Type != TokenType.EndOfFile && Start.Type != TokenType.SemiColon)
                {
                    if (Start.Type != TokenType.Token) return PatternMatch.NoMatch;
                    if (Start.Value.ToUpper() == "WHEN" || Start.Value.ToUpper() == "DO")
                    {
                        var bodyClauseEnd = FindEndOfBodyClause(AdvanceAndSkipWhitespace(Start, 1));
                        clauseList.Add(Tuple.Create(Start, bodyClauseEnd));
                        Start = bodyClauseEnd;
                    }
                    else
                        return PatternMatch.NoMatch;
                }

                Start = AdvanceAndSkipWhitespace(Start, 0);
                //The match must end with a semicolon.
                if (Start.Type != TokenType.SemiColon) return PatternMatch.NoMatch;
                Start = Advance(Start, 1);

                var end = Start;
                var before = originalStart.Previous;

                //Splice out the middle bit
                if (before != null) before.Next = end;
                if (end != null) end.Previous = before;

                //Build the header block.
                if (globalRule) before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "GlobalRules."));

                if (ruleType == "PERFORM") before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "Perform"));
                if (ruleType == "CHECK") before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "Check"));
                if (ruleType == "VALUE")
                    before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "Value"));

                if (ruleType == "VALUE")
                    before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "<" + String.Join(", ", arguments.Select(t => t.DeclarationType)) + ", " + resultType + ">(\"" + ruleName + "\")"));
                else
                    before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "<" + String.Join(", ", arguments.Select(t => t.DeclarationType)) + ">(\"" + ruleName + "\")"));

                //Emit clauses.
                foreach (var clause in clauseList)
                {
                    if (clause.Item1.Value.ToUpper() == "WHEN" || clause.Item1.Value.ToUpper() == "DO")
                    {
                        if (clause.Item1.Value.ToUpper() == "WHEN")
                            before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "\n.When((" + String.Join(", ", arguments.Select(t => t.Name)) + ") => "));
                        else
                            before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "\n.Do((" + String.Join(", ", arguments.Select(t => t.Name)) + ") => "));


                        var rest = before.Next;

                        before.Next = AdvanceAndSkipWhitespace(clause.Item1, 1);
                        before.Next.Previous = before;
                        var lastTokenInBody = clause.Item2.Previous;
                        lastTokenInBody.Next = rest;
                        rest.Previous = lastTokenInBody;

                        foreach (var token in EnumerateTokens(before, rest))
                            token.Tag = new RuleBodyClauseTag();

                        before.Next.Tag = new RuleBodyClauseHeadTag();
                        before = InsertAfter(lastTokenInBody, Token.Create(TokenType.GeneratedBlock, ")"));
                    }
                    else if (clause.Item1.Value.ToUpper() == "FIRST")
                    {
                        before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, ".First"));
                    }
                    else if (clause.Item1.Value.ToUpper() == "LAST")
                    {
                        before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, ".Last"));
                    }
                }

                before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, ";"));
                return PatternMatch.Create(originalStart.Previous.Next);
            }
            else
                return PatternMatch.NoMatch;
        }
    }
}
