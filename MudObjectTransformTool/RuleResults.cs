using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudObjectTransformTool
{
    public class AllowPattern : Pattern
    {
        public override MatchResult Match(Token Start)
        {
            if (Matches(MSequence(MToken("allow"), MSemicolon()), Start))
                return MatchResult.Create(Replace(Start, Advance(Start, 2), Token.Create(TokenType.GeneratedBlock, "return CheckResult.Allow;")));

            return MatchResult.NoMatch;
        }
    }

    public class DisallowPattern : Pattern
    {
        public override MatchResult Match(Token Start)
        {
            if (Matches(MSequence(MToken("disallow"), MSemicolon()), Start))
                return MatchResult.Create(Replace(Start, Advance(Start, 2), Token.Create(TokenType.GeneratedBlock, "return PerformResult.Disallow;")));

            return MatchResult.NoMatch;
        }
    }

    public class StopPattern : Pattern
    {
        public override MatchResult Match(Token Start)
        {
            if (Matches(MSequence(MToken("stop"), MSemicolon()), Start))
                return MatchResult.Create(Replace(Start, Advance(Start, 2), Token.Create(TokenType.GeneratedBlock, "return PerformResult.Stop;")));

            return MatchResult.NoMatch;
        }
    }

    public class ContinuePattern : Pattern
    {
        public override MatchResult Match(Token Start)
        {
            if (Matches(MSequence(MToken("continue"), MSemicolon()), Start))
                return MatchResult.Create(Replace(Start, Advance(Start, 2), Token.Create(TokenType.GeneratedBlock, "return PerformResult.Continue;")));

            return MatchResult.NoMatch;
        }
    }
}
