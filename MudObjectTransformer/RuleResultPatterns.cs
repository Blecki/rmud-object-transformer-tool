using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudObjectTransformer
{
    public class AllowPattern : Pattern
    {
        public override MatchResult Match(Token Start)
        {
            if (Start.Tag is RuleBodyClauseTag && Matches(MSequence(MToken("allow"), MSemicolon()), Start))
                return MatchResult.Create(Replace(Start, Advance(Start, 2), Token.Create(TokenType.GeneratedBlock, "return CheckResult.Allow;")));
            else if (Start.Tag is RuleBodyClauseHeadTag && Matches(MToken("allow"), Start))
                return MatchResult.Create(Replace(Start, Advance(Start, 1), Token.Create(TokenType.GeneratedBlock, "PerformResult.Allow")));

            return MatchResult.NoMatch;
        }
    }

    public class DisallowPattern : Pattern
    {
        public override MatchResult Match(Token Start)
        {
            if (Start.Tag is RuleBodyClauseTag && Matches(MSequence(MToken("disallow"), MSemicolon()), Start))
                return MatchResult.Create(Replace(Start, Advance(Start, 2), Token.Create(TokenType.GeneratedBlock, "return CheckResult.Disallow;")));
            else if (Start.Tag is RuleBodyClauseHeadTag && Matches(MToken("disallow"), Start))
                return MatchResult.Create(Replace(Start, Advance(Start, 1), Token.Create(TokenType.GeneratedBlock, "PerformResult.Disallow")));

            return MatchResult.NoMatch;
        }
    }

    public class StopPattern : Pattern
    {
        public override MatchResult Match(Token Start)
        {
            if (Start.Tag is RuleBodyClauseTag && Matches(MSequence(MToken("stop"), MSemicolon()), Start))
                return MatchResult.Create(Replace(Start, Advance(Start, 2), Token.Create(TokenType.GeneratedBlock, "return PerformResult.Stop;")));
            else if (Start.Tag is RuleBodyClauseHeadTag && Matches(MToken("stop"), Start))
                return MatchResult.Create(Replace(Start, Advance(Start, 1), Token.Create(TokenType.GeneratedBlock, "PerformResult.Stop")));

            return MatchResult.NoMatch;
        }
    }

    public class ContinuePattern : Pattern
    {
        public override MatchResult Match(Token Start)
        {
            if (Start.Tag is RuleBodyClauseTag && Matches(MSequence(MToken("continue"), MSemicolon()), Start))
                return MatchResult.Create(Replace(Start, Advance(Start, 2), Token.Create(TokenType.GeneratedBlock, "return PerformResult.Continue;")));
            else if (Start.Tag is RuleBodyClauseHeadTag && Matches(MToken("continue"), Start))
                return MatchResult.Create(Replace(Start, Advance(Start, 1), Token.Create(TokenType.GeneratedBlock, "PerformResult.Continue")));
                

            return MatchResult.NoMatch;
        }
    }
}
