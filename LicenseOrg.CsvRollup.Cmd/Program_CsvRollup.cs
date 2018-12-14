using LicenseOrg.CsvRollup.ProxyClasses;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace LicenseOrg.CsvRollup.Cmd
{
    public class Program_ScheduleNextRun
    {
        static void Main(string[] args)
        {
            var online = false;
            //var online = true;

            var isDelete = false;
            //var isDelete = true;

            var connectionString = online ? getOnlineConnectionString() : getOnPremConnectionString();

            var svc = createService(connectionString);

            var licenseOrgId = online ? getOnlineId() : getOnPremId();

            var licenseOrg = getLicenseOrg(svc, licenseOrgId);

            var app = new LicenseOrgCsvRollupApp(svc, licenseOrg, isDelete );

            app.Run();

            Console.WriteLine("Press <Enter> to exit...");
            //Console.ReadLine();
        }

        private static Guid getOnPremId()
        {
            return new Guid("{8CBE6C7E-BF0D-E711-8104-00155D6FD705}");
        }

        private static Guid getOnlineId()
        {
            return new Guid("{e3e04d1d-7902-e711-81dd-fc15b4287754}");
        }

        private static string getOnPremConnectionString()
        {
            return "Url=http://xrmedgecrm03:5555/DEV07; Username=xrmedge\\dev01; Password=;";
        }

        private static string getOnlineConnectionString()
        {
            return "Url=https://xedev24.crm.dynamics.com; Username=afischman@xedev24.onmicrosoft.com; Password=; AuthType=Office365";
        }

        private static IOrganizationService createService(string connectionString)
        {
            return new CrmServiceClient(connectionString);
        }

        private static xe0_LicenseOrg getLicenseOrg(IOrganizationService svc, Guid id)
        {
            return svc.Retrieve(xe0_LicenseOrg.LogicalName, id, new ColumnSet(true)).ToEntity<xe0_LicenseOrg>();
        }
    }
}