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
            RunTest("@@int test; int test; int foo;");
            RunTest("perform dance (Actor actor) when actor is Player do { SendMessage(actor, \"Dance dance revolution\"); stop; };");
            RunTest("check can open? (Actor actor) (MudObject item) when actor.Gender == Gender.Woman && item == pickleJar do { SendMessage(actor, \"You expected a sexist joke. You pig.\"); allow; };");

            Console.ReadKey();
        }
    }
}
