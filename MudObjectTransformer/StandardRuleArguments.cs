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

                #region Generated rules
                AddRule("at startup", RuleBookType.Perform);
                AddRule("before command", RuleBookType.Perform, "PossibleMatch match", "Actor actor");
                AddRule("after command", RuleBookType.Perform, "PossibleMatch match", "Actor actor");
                AddRule("after every command", RuleBookType.Perform, "Actor actor");
                AddRule("player joined", RuleBookType.Perform, "Actor actor");
                AddRule("player left", RuleBookType.Perform, "Actor actor");
                AddRule("silly?", RuleBookType.Value, "MudObject item", "Boolean");
                AddRule("can silly?", RuleBookType.Check, "MudObject actor", "MudObject item");
                AddRule("silly", RuleBookType.Perform, "MudObject actor", "MudObject item");
                AddRule("can dance?", RuleBookType.Check, "MudObject actor");
                AddRule("dance", RuleBookType.Perform, "MudObject actor");
                AddRule("can introduce?", RuleBookType.Check, "MudObject actor", "MudObject itroductee");
                AddRule("introduce", RuleBookType.Perform, "MudObject actor", "MudObject introductee");
                AddRule("can remove?", RuleBookType.Check, "MudObject actor", "MudObject item");
                AddRule("removed", RuleBookType.Perform, "MudObject actor", "MudObject item");
                AddRule("wearable?", RuleBookType.Value, "MudObject item", "Boolean");
                AddRule("can wear?", RuleBookType.Check, "MudObject actor", "MudObject item");
                AddRule("worn", RuleBookType.Perform, "MudObject actor", "MudObject item");
                AddRule("can converse?", RuleBookType.Check, "MudObject actor", "MudObject item");
                AddRule("greet", RuleBookType.Perform, "MudObject actor", "MudObject npc");
                AddRule("list topics", RuleBookType.Perform, "MudObject actor");
                AddRule("unlocked", RuleBookType.Perform, "MudObject actor", "MudObject item", "MudObject key");
                AddRule("can look relloc?", RuleBookType.Check, "MudObject actor", "MudObject item", "RelativeLocations relloc");
                AddRule("look relloc", RuleBookType.Perform, "MudObject actor", "MudObject item", "RelativeLocations relloc");
                AddRule("can close?", RuleBookType.Check, "MudObject actor", "MudObject item");
                AddRule("closed", RuleBookType.Perform, "MudObject actor", "MudObject item");
                AddRule("quest reset", RuleBookType.Perform, "MudObject quest", "MudObject item");
                AddRule("quest accepted", RuleBookType.Perform, "MudObject actor", "MudObject quest");
                AddRule("quest completed", RuleBookType.Perform, "MudObject actor", "MudObject quest");
                AddRule("quest failed", RuleBookType.Perform, "MudObject actor", "MudObject quest");
                AddRule("quest abandoned", RuleBookType.Perform, "MudObject actor", "MudObject quest");
                AddRule("quest available?", RuleBookType.Value, "MudObject actor", "MudObject quest", "Boolean");
                AddRule("quest complete?", RuleBookType.Value, "MudObject actor", "MudObject quest", "Boolean");
                AddRule("quest failed?", RuleBookType.Value, "MudObject actor", "MudObject quest", "Boolean");
                AddRule("heartbeat", RuleBookType.Perform);
                AddRule("update", RuleBookType.Perform, "MudObject item");
                AddRule("printed name", RuleBookType.Value, "MudObject actor", "MudObject item", "String article", "String value");
                AddRule("topic available?", RuleBookType.Value, "MudObject actor", "MudObject npc", "MudObject topic", "Boolean value");
                AddRule("topic response", RuleBookType.Perform, "MudObject actor", "MudObject npc", "MudObject topic");
                AddRule("can access channel?", RuleBookType.Check, "MudObject actor", "MudObject channel");
                AddRule("can put?", RuleBookType.Check, "MudObject actor", "MudObject item", "MudObject container", "RelativeLocations relloc");
                AddRule("put", RuleBookType.Perform, "MudObject actor", "MudObject item", "MudObject container", "RelativeLocations relloc");
                AddRule("lockable?", RuleBookType.Value, "MudObject item", "Boolean");
                AddRule("can lock?", RuleBookType.Check, "MudObject actor", "MudObject item", "MudObject key");
                AddRule("locked", RuleBookType.Perform, "MudObject actor", "MudObject item", "MudObject key");
                AddRule("can open?", RuleBookType.Check, "MudObject actor", "MudObject item");
                AddRule("openable?", RuleBookType.Value, "MudObject item", "Boolean");
                AddRule("opened", RuleBookType.Perform, "MudObject actor", "MudObject item");
                AddRule("can drop?", RuleBookType.Check, "MudObject actor", "MudObject item");
                AddRule("drop", RuleBookType.Perform, "MudObject actor", "MudObject item");
                AddRule("inventory", RuleBookType.Perform, "MudObject actor");
                AddRule("speak", RuleBookType.Perform, "MudObject actor", "String text");
                AddRule("emote", RuleBookType.Perform, "MudObject actor", "String text");
                AddRule("can take?", RuleBookType.Check, "MudObject actor", "MudObject item");
                AddRule("take", RuleBookType.Perform, "MudObject actor", "MudObject item");
                AddRule("can go?", RuleBookType.Check, "MudObject actor", "Link link");
                AddRule("go", RuleBookType.Perform, "MudObject actor", "Link link");
                AddRule("can examine?", RuleBookType.Check, "MudObject actor", "MudObject item");
                AddRule("describe", RuleBookType.Perform, "MudObject actor", "MudObject item");
                AddRule("describe in locale", RuleBookType.Perform, "MudObject actor", "MudObject item");
                AddRule("describe locale", RuleBookType.Perform, "MudObject actor", "MudObject room");
                AddRule("emits-light", RuleBookType.Value, "MudObject item", "LightingLevel");
#endregion
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
                        + "RuleBookType."
                        + entry.Value.Type.ToString()
                        + (entry.Value.Arguments.Count != 0 ? ", " : "")
                        + String.Join(", ", entry.Value.Arguments.Select(ra => "\"" + ra.DeclarationType + " " + ra.Name + "\""))
                        + (entry.Value.Type == RuleBookType.Value ? ", \"" + entry.Value.ResultType + "\"" : "")
                        + ");");
            }
        }
    }
}
