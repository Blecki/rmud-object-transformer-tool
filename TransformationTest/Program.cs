using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransformationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MudObjectTransformTool.Pattern.DiscoverPatterns();
            Console.WriteLine(MudObjectTransformTool.Pattern.ProcessFile("int test;"));
            Console.ReadKey();
        }
    }
}
