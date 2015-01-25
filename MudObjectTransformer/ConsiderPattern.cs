using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudObjectTransformer
{
    public class ConsiderPattern : Pattern
    {
        public override PatternMatch Match(Token Start)
        {
            var pattern = Sequence(
                "consider",
                Capture(Star(Pred(t => t.Value != "with" && t.Value != ";")), "rule-name"),
                Optional(
                    Sequence(
                    MToken("with"),
                    CaptureMany(Sequence(Expression), "argument"),
                    Star(CaptureMany(Sequence(Comma, Expression), "argument")))),
                Semicolon);
            var context = MatchContext.Create(Start);
            var matchResult = pattern.Matches(context);
            if (!matchResult.Matched) return PatternMatch.NoMatch;
            var ruleName = Pattern.SpanAsString(Span.Trim(context["rule-name"] as Span));

            var ruleType = RuleBookType.Perform;
            if (StandardRules.RuleDefined(ruleName))
                ruleType = StandardRules.RuleType(ruleName);
            
            return PatternMatch.Create(Replace(context.Span, 
                MakeSpan(
                    "GlobalRules.Consider",
                    ruleType.ToString(),
                    "Rule",
                    (ruleType == RuleBookType.Value ? "<" + StandardRules.RuleResultType(ruleName) + ">" : ""),
                    "(\"" + ruleName + "\"",
                    (context.TypedValue<List<Span>>("argument") == null ? 
                        (Object)(new Token { Type = TokenType.GeneratedBlock, Value = ""}) :
                        (Object)(AggregateSpan(context["argument"] as List<Span>, a => MakeSpan(", ", a)))),
                    ");"
                )).Start);
        }
    }
}
