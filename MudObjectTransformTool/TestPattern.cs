using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudObjectTransformTool
{
    public class TestPattern : Pattern
    {
        public override MatchResult Match(Token Start)
        {
            if (Matches(MSequence(MToken("@@int"), MWhitespace(), MToken("test")), Start))
            {
                Start.Value = "float";
                return MatchResult.Create(Advance(Start, 3));
            }

            return MatchResult.NoMatch;
        }
    }
}
