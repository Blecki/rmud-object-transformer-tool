using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudObjectTransformer
{
    public class MatchContext : Dictionary<String, Object>
    {
        public Span Span { get { return new Span { Start = this.Start, End = this.CurrentToken }; } }
        public Token Start;
        public Token CurrentToken;

        public static MatchContext Create(Token Start)
        {
            return new MatchContext
            {
                Start = Start,
                CurrentToken = Start
            };
        }
    }

    public class Matcher
    {
        public virtual PatternMatch Matches(MatchContext Context)
        {
            throw new NotImplementedException();
        }

        public PatternMatch TryMatch(MatchContext Context)
        {
            var rememberedPlace = Context.CurrentToken;
            var r = Matches(Context);
            Context.CurrentToken = rememberedPlace;
            return r;
        }
    }

    public class TokenMatcher : Matcher
    {
        String ExactString;

        public TokenMatcher(String ExactString)
        {
            this.ExactString = ExactString.ToUpper();
        }

        public override PatternMatch Matches(MatchContext Context)
        {
            if (Context.CurrentToken != null && Context.CurrentToken.Value.ToUpper() == ExactString) return PatternMatch.Create(Context.CurrentToken.Next);
            else return PatternMatch.NoMatch;
        }
    }

    public class CaptureMatcher : Matcher
    {
        String ArgumentName;
        Matcher SubMatcher;

        public CaptureMatcher(Matcher SubMatcher, String ArgumentName)
        {
            this.ArgumentName = ArgumentName;
            this.SubMatcher = SubMatcher;
        }

        public override PatternMatch Matches(MatchContext Context)
        {
            var start = Context.CurrentToken;
            var subResult = SubMatcher.Matches(Context);
            if (subResult.Matched)
            {
                Context.Upsert(ArgumentName, new Span { Start = start, End = subResult.NextToken });
                Context.CurrentToken = subResult.NextToken;
            }
            return subResult;
        }
    }

    public class CaptureManyMatcher : Matcher
    {
        String ArgumentName;
        Matcher SubMatcher;

        public CaptureManyMatcher(Matcher SubMatcher, String ArgumentName)
        {
            this.ArgumentName = ArgumentName;
            this.SubMatcher = SubMatcher;
        }

        public override PatternMatch Matches(MatchContext Context)
        {
            var start = Context.CurrentToken;
            var subResult = SubMatcher.Matches(Context);
            if (subResult.Matched)
            {
                if (Context.TypedValue<List<Span>>(ArgumentName) == null) Context.Upsert(ArgumentName, new List<Span>());
                Context.TypedValue<List<Span>>(ArgumentName).Add(new Span { Start = start, End = subResult.NextToken });
                Context.CurrentToken = subResult.NextToken;
            }
            return subResult;
        }
    }

    public class BasicTokenMatcher : Matcher
    {
        public TokenType ExpectedType;

        public BasicTokenMatcher(TokenType ExpectedType)
        {
            this.ExpectedType = ExpectedType;
        }

        public override PatternMatch Matches(MatchContext Context)
        {
            if (Context.CurrentToken != null && Context.CurrentToken.Type == ExpectedType) return PatternMatch.Create(Context.CurrentToken.Next);
            else return PatternMatch.NoMatch;
        }
    }

    public class SequenceMatcher : Matcher
    {
        List<Matcher> SubMatchers;

        public SequenceMatcher(params Object[] Subs)
        {
            SubMatchers = new List<Matcher>();
            foreach (var sub in Subs)
            {
                if (sub is Matcher) SubMatchers.Add(sub as Matcher);
                else if (sub is String) SubMatchers.Add(new TokenMatcher(sub as String));
                else throw new InvalidOperationException();
            }
        }

        public override PatternMatch Matches(MatchContext Context)
        {
            foreach (var sub in SubMatchers)
            {
                var subResult = sub.Matches(Context);
                if (subResult.Matched == false) return PatternMatch.NoMatch;
                Context.CurrentToken = subResult.NextToken;
            }
            return PatternMatch.Create(Context.CurrentToken);
        }
    }

    public class GreedyOrMatcher : Matcher
    {
        List<Matcher> SubMatchers;

        public GreedyOrMatcher(params Object[] Subs)
        {
            SubMatchers = new List<Matcher>();
            foreach (var sub in Subs)
            {
                if (sub is Matcher) SubMatchers.Add(sub as Matcher);
                else if (sub is String) SubMatchers.Add(new TokenMatcher(sub as String));
                else throw new InvalidOperationException();
            }
        }

        public override PatternMatch Matches(MatchContext Context)
        {
            foreach (var sub in SubMatchers)
            {
                var subResult = sub.Matches(Context);
                if (subResult.Matched)
                {
                    Context.CurrentToken = subResult.NextToken;
                    return subResult;
                }
            }
            return PatternMatch.NoMatch;
        }
    }

    public class KleeneMatcher : Matcher
    {
        Matcher SubMatcher;

        public KleeneMatcher(Matcher SubMatcher)
        {
            this.SubMatcher = SubMatcher;
        }

        public override PatternMatch Matches(MatchContext Context)
        {
            PatternMatch subResult;
            while ((subResult = SubMatcher.Matches(Context)).Matched)
                Context.CurrentToken = subResult.NextToken;
            return PatternMatch.Create(Context.CurrentToken);
        }
    }

    public class ExpressionMatcher : Matcher
    {
        Matcher Terminal;

        public ExpressionMatcher(Matcher Terminal)
        {
            this.Terminal = Terminal;
        }

        public override PatternMatch Matches(MatchContext Context)
        {
            PatternMatch terminalMatch = PatternMatch.NoMatch;

            do
            {
                if (Context.CurrentToken.Type == TokenType.OpenBrace ||
                    Context.CurrentToken.Type == TokenType.OpenBracket ||
                    Context.CurrentToken.Type == TokenType.OpenParen)
                {
                    Context.CurrentToken = Pattern.FindMatchingBracket(Context.CurrentToken);
                    continue;
                }
                else
                {
                    terminalMatch = Terminal.TryMatch(Context);
                    if (!terminalMatch.Matched) Context.CurrentToken = Context.CurrentToken.Next;
                }
            } while (Context.CurrentToken.Type != TokenType.EndOfFile && terminalMatch.Matched == false);
            return PatternMatch.Create(Context.CurrentToken);
        }
    }

    public class NegativeMatcher : Matcher
    {
        Matcher SubMatcher;

        public NegativeMatcher(Matcher SubMatcher)
        {
            this.SubMatcher = SubMatcher;
        }

        public override PatternMatch Matches(MatchContext Context)
        {
            var subResult = SubMatcher.Matches(Context);
            if (subResult.Matched) return PatternMatch.NoMatch;
            else return PatternMatch.Create(Context.CurrentToken);
        }
    }

    public class PredMatcher : Matcher
    {
        Func<Token, bool> Predicate;

        public PredMatcher(Func<Token, bool> Predicate)
        {
            this.Predicate = Predicate;
        }

        public override PatternMatch Matches(MatchContext Context)
        {
            if (Predicate(Context.CurrentToken))
                return PatternMatch.Create(Context.CurrentToken.Next);
            else
                return PatternMatch.NoMatch;
        }
    }

    public class OptionalMatcher : Matcher
    {
        Matcher SubMatcher;

        public OptionalMatcher(Matcher SubMatcher)
        {
            this.SubMatcher = SubMatcher;
        }

        public override PatternMatch Matches(MatchContext Context)
        {
            var subResult = SubMatcher.Matches(Context);
            if (subResult.Matched) Context.CurrentToken = subResult.NextToken;
            return PatternMatch.Create(Context.CurrentToken);
        }
    }

    public partial class Pattern
    {
        public static Matcher Sequence(params Object[] SubMatchers) { return new SequenceMatcher(SubMatchers); }
        public static Matcher Whitespace { get { return new BasicTokenMatcher(TokenType.Whitespace); } }
        public static Matcher Semicolon { get { return new BasicTokenMatcher(TokenType.SemiColon); } }
        public static Matcher Comma { get { return new BasicTokenMatcher(TokenType.Comma); } }
        public static Matcher Star(Matcher Sub) { return new KleeneMatcher(Sub); }
        public static Matcher Pred(Func<Token, bool> Predicate) { return new PredMatcher(Predicate); }
        public static Matcher MToken(String ExactString) { return new TokenMatcher(ExactString); }
        public static Matcher Capture(Matcher Sub, String Name) { return new CaptureMatcher(Sub, Name); }
        public static Matcher CaptureMany(Matcher Sub, String Name) { return new CaptureManyMatcher(Sub, Name); }
        public static Matcher Not(Matcher Sub) { return new NegativeMatcher(Sub); }
        public static Matcher GreedyOr(params Object[] SubMatchers) { return new GreedyOrMatcher(SubMatchers); }
        public static Matcher Expression { get { return new ExpressionMatcher(GreedyOr(Comma, Semicolon)); } }
        public static Matcher Optional(Matcher Sub) { return new OptionalMatcher(Sub); }
        

        public static bool Matches(Matcher M, Token Start)
        {
            var context = MatchContext.Create(Start);
            var result = M.Matches(context);
            return result.Matched;
        }
    }
}
