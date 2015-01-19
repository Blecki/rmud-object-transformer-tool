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
            Console.WriteLine(MudObjectTransformTool.Pattern.ProcessFile(Code));
            Console.WriteLine();
        }
        static void Main(string[] args)
        {
            MudObjectTransformTool.Pattern.DiscoverPatterns();
            RunTest(/*@"
Global first perform dance (Actor actor) 
    when actor is Player 
    do { 
        SendMessage(actor, ""Dance dance revolution""); 
        stop; 
    };

Check can put? (Actor actor) (MudObject item) (MudObject container) (RelativeLocation relloc)
    when item is CheeseSandwich && container is Mouth
    do {
        SendMessage(actor, ""Wait stop, you're lactose intolerant!"");
        Disallow;
    };
*/@"
Perform test rule 
    do continue;");
            
            Console.ReadKey();
        }
    }
}
