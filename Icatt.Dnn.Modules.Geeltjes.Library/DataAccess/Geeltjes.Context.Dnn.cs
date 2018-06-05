using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetNuke.Data;

namespace Icatt.DotNetNuke.Modules.Geeltjes.DataAccess
{
    public partial class GeeltjesEntities
    {
        #region Connection Stuff

        public static GeeltjesEntities GetContext()
        {
            string connectionString = GetConnectionString();

            var retval = new GeeltjesEntities(connectionString);

            retval.CommandTimeout = 180;

            return retval;
        }

        private static string GetConnectionString()
        {
            var dnnConnectStringBuilder = DataProvider.Instance().GetConnectionStringBuilder();
            dnnConnectStringBuilder.ConnectionString = DataProvider.Instance().ConnectionString;
            // we need to check for MultipleActiveResultSets, which we need to have set to true
            if (!dnnConnectStringBuilder.ContainsKey("MultipleActiveResultSets"))
            {
                dnnConnectStringBuilder.Add("MultipleActiveResultSets", true);
            }
            else
            {
                dnnConnectStringBuilder["MultipleActiveResultSets"] = true;
            }

            return String.Format(
                "metadata=res://*/DataAccess.Geeltjes.csdl|res://*/DataAccess.Geeltjes.ssdl|res://*/DataAccess.Geeltjes.msl;provider=System.Data.SqlClient;provider connection string=\"{0}\"",
                dnnConnectStringBuilder.ConnectionString);
        }

        #endregion
    }
}
