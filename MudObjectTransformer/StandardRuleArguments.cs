using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudObjectTransformer
{
    public class RuleArgument
    {
        public String DeclarationType;
        public String Name;
    }

    public enum RuleBookType
    {
        Perform,
        Check,
        Value
    }

    public class RuleBookDefinition
    {
        public RuleBookType Type;
        public List<RuleArgument> Arguments;
        public String ResultType;
    }

    public static class StandardRules
    {
        private static Dictionary<String, RuleBookDefinition> Rules = null;

        public static void InitializeStandardRules()
        {
            if (Rules == null)
            {
                Rules = new Dictionary<String, RuleBookDefinition>();
                AddRule("test rule", RuleBookType.Perform, "Actor actor", "MudObject item", "MudObject container");
                AddRule("test value", RuleBookType.Perform, "Actor actor", "bool value");
            }
        }

        public static bool RuleDefined(String Name)
        {
            return Rules.ContainsKey(Name);
        }

        public static List<RuleArgument> RuleArguments(String Name)
        {
            return Rules[Name].Arguments;
        }

        public static String RuleResultType(String Name)
        {
            return Rules[Name].ResultType;
        }

        public static RuleBookType RuleType(String Name)
        {
            return Rules[Name].Type;
        }

        public static void AddRule(String RuleName, RuleBookType Type, params String[] TypeNamePairs)
        {
            var resultType = "CheckResult";
            if (Type == RuleBookType.Check) { }
            else if (Type == RuleBookType.Perform) resultType = "PerformResult";
            else if (Type == RuleBookType.Value)
            {
                if (TypeNamePairs.Length == 0) throw new InvalidOperationException();
                resultType = TypeNamePairs.Last();
            }

            var list = new List<RuleArgument>(TypeNamePairs.Take(Type == RuleBookType.Value ? TypeNamePairs.Length - 1 : TypeNamePairs.Length).Select(p =>
                {
                    var parts = p.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 2) throw new InvalidOperationException();
                    return new RuleArgument { DeclarationType = parts[0], Name = parts[1] };
                }));

            if (Rules == null)
                Rules = new Dictionary<String, RuleBookDefinition>();
            
            if (Rules.ContainsKey(RuleName))
                Rules.Remove(RuleName);
            Rules.Add(RuleName, new RuleBookDefinition
            {
                Arguments = list,
                Type = Type,
                ResultType = resultType
            });
        }

        public static void DumpStandardRuleArgumentTypes(String Filename)
        {
            using (var writer = new System.IO.StreamWriter(Filename))
            {
                foreach (var entry in Rules)
                    writer.WriteLine("AddRule(\"" + entry.Key + "\", "
                        + entry.Value.Type.ToString()
                        + (entry.Value.Arguments.Count != 0 ? ", " : "")
                        + String.Join(", ", entry.Value.Arguments.Select(ra => "\"" + ra.DeclarationType + " " + ra.Name + "\""))
                        + (entry.Value.Type == RuleBookType.Value ? ", " + entry.Value.ResultType : "")
                        + ");");
            }
        }
    }
}
