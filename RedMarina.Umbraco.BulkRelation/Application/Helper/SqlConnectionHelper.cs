using RedMarina.Umbraco.BulkRelation.Interfaces;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace RedMarina.Umbraco.BulkRelation.Application.Helper
{
    public class SqlConnectionHelper : IConnectionHelper
    {
        public SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ConnectionString);

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            return connection;
        }
    }
}