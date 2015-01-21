﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudObjectTransformer
{
    public class Matcher
    {
        public virtual MatchResult Matches(Token Token)
        {
            throw new NotImplementedException();
        }
    }

    public class TokenMatcher : Matcher
    {
        String ExactString;

        public TokenMatcher(String ExactString)
        {
            this.ExactString = ExactString.ToUpper();
        }

        public override MatchResult Matches(Token Token)
        {
            if (Token != null && Token.Value.ToUpper() == ExactString) return MatchResult.Create(Token.Next);
            else return MatchResult.NoMatch;
        }
    }

    public class WhitespaceMatcher : Matcher
    {
        public override MatchResult Matches(Token Token)
        {
            if (Token != null && Token.Type == TokenType.Whitespace) return MatchResult.Create(Token.Next);
            else return MatchResult.NoMatch;
        }
    }

    public class SequenceMatcher : Matcher
    {
        List<Matcher> SubMatchers;

        public SequenceMatcher(params Matcher[] SubMatchers)
        {
            this.SubMatchers = new List<Matcher>(SubMatchers);
        }

        public override MatchResult Matches(Token Token)
        {
            var current = Token;
            foreach (var sub in SubMatchers)
            {
                var subResult = sub.Matches(current);
                if (subResult.Matched == false) return MatchResult.NoMatch;
                current = subResult.NextToken;
            }
            return MatchResult.Create(current);
        }
    }
}