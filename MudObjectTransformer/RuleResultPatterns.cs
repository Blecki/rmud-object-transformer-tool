using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudObjectTransformer
{
    public class AllowPattern : Pattern
    {
        public override PatternMatch Match(Token Start)
        {
            if (Start.Tag is RuleBodyClauseTag && Matches(Sequence("allow", Semicolon), Start))
                return PatternMatch.Create(Replace(Start, Advance(Start, 2), Token.Create(TokenType.GeneratedBlock, "return CheckResult.Allow;")));
            else if (Start.Tag is RuleBodyClauseHeadTag && Matches(MToken("allow"), Start))
                return PatternMatch.Create(Replace(Start, Advance(Start, 1), Token.Create(TokenType.GeneratedBlock, "PerformResult.Allow")));

            return PatternMatch.NoMatch;
        }
    }

    public class DisallowPattern : Pattern
    {
        public override PatternMatch Match(Token Start)
        {
            if (Start.Tag is RuleBodyClauseTag && Matches(Sequence(MToken("disallow"), Semicolon), Start))
                return PatternMatch.Create(Replace(Start, Advance(Start, 2), Token.Create(TokenType.GeneratedBlock, "return CheckResult.Disallow;")));
            else if (Start.Tag is RuleBodyClauseHeadTag && Matches(MToken("disallow"), Start))
                return PatternMatch.Create(Replace(Start, Advance(Start, 1), Token.Create(TokenType.GeneratedBlock, "PerformResult.Disallow")));

            return PatternMatch.NoMatch;
        }
    }

    public class StopPattern : Pattern
    {
        public override PatternMatch Match(Token Start)
        {
            if (Start.Tag is RuleBodyClauseTag && Matches(Sequence(MToken("stop"), Semicolon), Start))
                return PatternMatch.Create(Replace(Start, Advance(Start, 2), Token.Create(TokenType.GeneratedBlock, "return PerformResult.Stop;")));
            else if (Start.Tag is RuleBodyClauseHeadTag && Matches(MToken("stop"), Start))
                return PatternMatch.Create(Replace(Start, Advance(Start, 1), Token.Create(TokenType.GeneratedBlock, "PerformResult.Stop")));

            return PatternMatch.NoMatch;
        }
    }

    public class ContinuePattern : Pattern
    {
        public override PatternMatch Match(Token Start)
        {
            if (Start.Tag is RuleBodyClauseTag && Matches(Sequence(MToken("continue"), Semicolon), Start))
                return PatternMatch.Create(Replace(Start, Advance(Start, 2), Token.Create(TokenType.GeneratedBlock, "return PerformResult.Continue;")));
            else if (Start.Tag is RuleBodyClauseHeadTag && Matches(MToken("continue"), Start))
                return PatternMatch.Create(Replace(Start, Advance(Start, 1), Token.Create(TokenType.GeneratedBlock, "PerformResult.Continue")));
                

            return PatternMatch.NoMatch;
        }
    }
}
