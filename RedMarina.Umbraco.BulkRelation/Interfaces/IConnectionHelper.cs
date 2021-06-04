using System.Data.SqlClient;

namespace RedMarina.Umbraco.BulkRelation.Interfaces
{
    public interface IConnectionHelper
    {
        SqlConnection GetConnection();
    }
}