using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudObjectTransformer
{
    public class PatternMatch
    {
        public Token NextToken;
        public bool Matched;

        public static PatternMatch NoMatch
        {
            get
            {
                return new PatternMatch { Matched = false };
            }
        }

        public static PatternMatch Create(Token NextToken)
        {
            return new PatternMatch { NextToken = NextToken, Matched = true };
        }
    }
}
