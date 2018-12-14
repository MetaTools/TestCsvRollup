using LicenseOrg.CsvRollup.ProxyClasses;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;

namespace LicenseOrg.CsvRollup
{
    public class LicenseOrgCsvRollupApp
    {
        private IOrganizationService svc;
        private xe0_LicenseOrg licenseOrg;
        private bool isDelete;

        public LicenseOrgCsvRollupApp(IOrganizationService svc, xe0_LicenseOrg licenseOrg, bool isDelete)
        {
            this.svc = svc;
            this.licenseOrg = licenseOrg;
            this.isDelete = isDelete;
        }

        public void Run()
        {           
            if (licenseOrg.Contains(xe0_LicenseOrg.Properties.xe0_LicenseId))
            {
                var license = svc.Retrieve(xe0_License.LogicalName, licenseOrg.xe0_LicenseId.Id, new ColumnSet(true)).ToEntity<xe0_License>();

                var licenseOrgs = license.xe0_xe0_license_xe0_licenseorg(svc);
   
                var names = isDelete ? orgUniqueNamesMinusDeleted(licenseOrgs,licenseOrg) : allOrgUniqueNames(licenseOrgs);

                names.Sort();

                license.xe0_Orgs = string.Join(", ", names.ToArray());

                license.Update(svc);
            }            
        }

        private List<string> allOrgUniqueNames(List<xe0_LicenseOrg> licenseOrgs)
        {
            var list = new List<string>();

            foreach (var licenseOrg in licenseOrgs)
            {
                var crmOrg = svc.Retrieve(xe0_DynamicsOrganization.LogicalName, licenseOrg.xe0_OrgId.Id, new ColumnSet(true)).ToEntity<xe0_DynamicsOrganization>();
                list.Add(crmOrg.xe0_UniqueName);
            }

            return list;
        }

        private List<string> orgUniqueNamesMinusDeleted(List<xe0_LicenseOrg> licenseOrgs, xe0_LicenseOrg deletedLicenseOrg)
        {
            var deletedName = deletedOrgUniqueName(deletedLicenseOrg);

            var allNames = allOrgUniqueNames(licenseOrgs);

            allNames.Remove(deletedName);

            return allNames;
        }

        private string deletedOrgUniqueName(xe0_LicenseOrg deletedLicenseOrg)
        {
            var crmOrg = svc.Retrieve(xe0_DynamicsOrganization.LogicalName, deletedLicenseOrg.xe0_OrgId.Id, new ColumnSet(true)).ToEntity<xe0_DynamicsOrganization>();
            return crmOrg.xe0_UniqueName ?? string.Empty;
        }
    }
}