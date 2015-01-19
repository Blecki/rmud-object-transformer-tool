using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudObjectTransformTool
{
    public class RuleBodyClauseTag { }
    public class RuleBodyClauseHeadTag { }

    public class RulePattern : Pattern
    {
        private static Dictionary<String, List<Tuple<String, String>>> StandardRuleArguments = null;

        public RulePattern()
        {
            if (StandardRuleArguments == null)
            {
                StandardRuleArguments = new Dictionary<string, List<Tuple<string, string>>>();
                AddStandardRuleArgumentTypes("test rule", "Actor actor", "MudObject item", "MudObject container");
                AddStandardRuleArgumentTypes("test value", "Actor actor", "bool value");
            }
        }

        public static void AddStandardRuleArgumentTypes(String RuleName, params String[] TypeNamePairs)
        {
            var list = new List<Tuple<String, String>>(TypeNamePairs.Select(p =>
                {
                    var parts = p.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 2) throw new InvalidOperationException();
                    return Tuple.Create(parts[0], parts[1]);
                }));
            if (StandardRuleArguments == null)
                StandardRuleArguments = new Dictionary<string, List<Tuple<string, string>>>();
            if (StandardRuleArguments.ContainsKey(RuleName)) StandardRuleArguments[RuleName] = list;
            else StandardRuleArguments.Add(RuleName, list);
        }

        public override MatchResult Match(Token Start)
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
                var arguments = new List<Tuple<String, String>>();

                while (Start.Type != TokenType.EndOfFile && Start.Type == TokenType.OpenParen)
                {
                    Start = AdvanceAndSkipWhitespace(Start, 1);
                    //An empty argument - () - is allowed. It's required for value rules that take 0 arguments.
                    if (Start.Type == TokenType.CloseParen)
                    {
                        Start = AdvanceAndSkipWhitespace(Start, 1);
                        continue;
                    }

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

                bool usingStandardArguments = false;
                if (StandardRuleArguments.ContainsKey(ruleName) && arguments.Count == 0)
                {
                    arguments = StandardRuleArguments[ruleName];
                    usingStandardArguments = true;
                }

                //For value rules only, a result type must appear immediately after the last argument.
                var resultType = "";
                if (ruleType == "VALUE")
                {
                    if (usingStandardArguments)
                    {
                        resultType = arguments[arguments.Count - 1].Item1;
                        arguments = new List<Tuple<string, string>>(arguments.Take(arguments.Count - 1));
                    }
                    else
                    {
                        if (Start.Type != TokenType.Token) return MatchResult.NoMatch;
                        resultType = Start.Value;
                        Start = AdvanceAndSkipWhitespace(Start, 1);
                    }
                }

                //Detect when and do clauses. They can appear in any order.
                while (Start.Type != TokenType.EndOfFile && Start.Type != TokenType.SemiColon)
                {
                    if (Start.Type != TokenType.Token) return MatchResult.NoMatch;
                    if (Start.Value.ToUpper() == "WHEN" || Start.Value.ToUpper() == "DO")
                    {
                        var bodyClauseEnd = FindEndOfBodyClause(AdvanceAndSkipWhitespace(Start, 1));
                        clauseList.Add(Tuple.Create(Start, bodyClauseEnd));
                        Start = bodyClauseEnd;
                    }
                    else
                        return MatchResult.NoMatch;
                }

                Start = AdvanceAndSkipWhitespace(Start, 0);
                //The match must end with a semicolon.
                if (Start.Type != TokenType.SemiColon) return MatchResult.NoMatch;
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
                    before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "<" + String.Join(", ", arguments.Select(t => t.Item1)) + ", " + resultType + ">(\"" + ruleName + "\")"));
                else
                    before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "<" + String.Join(", ", arguments.Select(t => t.Item1)) + ">(\"" + ruleName + "\")"));

                //Emit clauses.
                foreach (var clause in clauseList)
                {
                    if (clause.Item1.Value.ToUpper() == "WHEN" || clause.Item1.Value.ToUpper() == "DO")
                    {
                        if (clause.Item1.Value.ToUpper() == "WHEN")
                            before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "\n.When((" + String.Join(", ", arguments.Select(t => t.Item2)) + ") => "));
                        else
                            before = InsertAfter(before, Token.Create(TokenType.GeneratedBlock, "\n.Do((" + String.Join(", ", arguments.Select(t => t.Item2)) + ") => "));


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
                return MatchResult.Create(originalStart.Previous.Next);
            }
            else
                return MatchResult.NoMatch;
        }

        /// <summary>
        /// Given a start token, which is the first token after the 'when' or 'do' keyword, find the 
        /// first token after the end of the clause.
        /// </summary>
        /// <param name="Start"></param>
        /// <returns></returns>
        private Token FindEndOfBodyClause(Token Start)
        {
            var end = Start;

            while (end.Type != TokenType.EndOfFile && end.Type != TokenType.SemiColon && end.Value.ToUpper() != "WHEN" && end.Value.ToUpper() != "DO")
            {
                if (end.Type == TokenType.OpenBrace || end.Type == TokenType.OpenBracket || end.Type == TokenType.OpenParen)
                    end = FindMatchingBracket(end);
                else
                    end = Advance(end, 1);
            }

            return end;
        }

        /// <summary>
        /// Given a token that is a brace, bracket, or paren, find the token after the token
        /// that closes the pair.
        /// </summary>
        /// <param name="Start"></param>
        /// <returns></returns>
        private Token FindMatchingBracket(Token Start)
        {
            var closeType = TokenType.CloseParen;
            if (Start.Type == TokenType.OpenBrace) closeType = TokenType.CloseBrace;
            else if (Start.Type == TokenType.OpenBracket) closeType = TokenType.CloseBracket;
            else if (Start.Type == TokenType.OpenParen) closeType = TokenType.CloseParen;

            Start = Advance(Start, 1);
            while (Start.Type != TokenType.EndOfFile && Start.Type != closeType)
            {
                if (Start.Type == TokenType.OpenBrace || Start.Type == TokenType.OpenBracket || Start.Type == TokenType.OpenParen)
                    Start = FindMatchingBracket(Start);
                else
                    Start = Advance(Start, 1);
            }
            return Advance(Start, 1);
        }
    }
}
