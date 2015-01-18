using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransformationDebug
{
    class Program
    {
        static void Main(string[] args)
        {
            MudObjectTransformTool.Pattern.DiscoverPatterns();
            Console.WriteLine(MudObjectTransformTool.Pattern.ProcessFile("@@int test; int test; int foo;"));
            Console.WriteLine(MudObjectTransformTool.Pattern.ProcessFile("perform dance (actor Actor) when actor is Player do SendMessage(actor, \"Dance dance revolution\");"));
            Console.WriteLine(MudObjectTransformTool.Pattern.ProcessFile("check can open? (actor Actor) (item MudObject) when actor.Gender == Gender.Woman && item == pickleJar do { SendMessage(actor, \"You expected a sexist joke. You pig.\"); return CheckResult.Allow; }"));

            Console.ReadKey();
        }
    }
}
