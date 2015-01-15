using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gacfucker
{
    class Program
    {
        static void Main(string[] args)
        {
            var publish = new System.EnterpriseServices.Internal.Publish();
            publish.GacInstall(args[0]);
        }
    }
}
