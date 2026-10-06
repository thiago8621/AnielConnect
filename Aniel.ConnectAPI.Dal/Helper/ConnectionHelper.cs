using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.ConnectAPI.Dal.Helper
{
    public class ConnectionHelper
    {
        public static FbConnection CreateConnection()
        {
            return CommandHelper.CreateConnection();
        }

        public static FbTransaction BeginTransaction(FbConnection conn)
        {
            return CommandHelper.BeginTransaction(conn);
        }

    }
}
