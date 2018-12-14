using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvRollupNN.Cmd
{
    public class Program
    {
        static void Main(string[] args)
        {
            var svc = new CrmServiceClient(args[0]);
            if (!svc.IsReady) { throw new Exception("service not ready"); }
            var app = new App(svc);
            //app.Run(new Guid("DBAF5212-D089-4FCB-B09D-6DB00E2D90A8"));
            app.Run(new EntityReference("cobalt_meeting",new Guid("DBAF5212-D089-4FCB-B09D-6DB00E2D90A8")));
            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press <Enter> to exit...");
                Console.ReadLine();
            }
        }
    }
}
