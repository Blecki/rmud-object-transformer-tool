using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransformationDebug
{
    class Program
    {
        private static void RunTest(String Code)
        {
            Console.WriteLine("BEFORE:");
            Console.WriteLine(Code);
            Console.WriteLine("AFTER:");
            Console.WriteLine(MudObjectTransformer.Pattern.ProcessFile(Code));
            Console.WriteLine();
        }
        static void Main(string[] args)
        {
            MudObjectTransformer.StandardRules.AddRule("test", MudObjectTransformer.RuleBookType.Value, "int foo", "float");
            RunTest(@"
Consider multi token name with arg1, arg2;
Consider another rule;

value test when foo == 3 do 6.0f;

consider test with 4;

");
            
            Console.ReadKey();
        }
    }
}
