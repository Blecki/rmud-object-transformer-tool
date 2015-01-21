using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudObjectTransformer
{
    public class MatchResult
    {
        public Token NextToken;
        public bool Matched;

        public static MatchResult NoMatch
        {
            get
            {
                return new MatchResult { Matched = false };
            }
        }

        public static MatchResult Create(Token NextToken)
        {
            return new MatchResult { NextToken = NextToken, Matched = true };
        }
    }
}
