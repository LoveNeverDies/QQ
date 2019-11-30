using Newbe.Mahua.Plugins.Parrot.Helper;
using System.Data;
using System.Data.SqlClient;

namespace Newbe.Mahua.Plugins.Parrot.HelperService
{
    public interface ISqlServerTableHelper
    {
        int ExecuteNonQuery(string sql, params SqlParameter[] pms);
        object ExecuteScalar(string sql, params SqlParameter[] pms);
        IDataReader ExecuteReader(string sql, params SqlParameter[] pms);
        DataTable ExecuteDataTable(string sql, params SqlParameter[] pms);
        DataSet ExecuteDataSet(string sql, params SqlParameter[] pms);
    }
    class SqlServerTableHelper : ISqlServerTableHelper
    {
        IJsonHelper jsonHelper = null;
        SqlConnection conn = null;
        SqlCommand cmd = null;
        public SqlServerTableHelper()
        {
            jsonHelper = new JsonHelper();
        }
        string ConnectionString { get { return jsonHelper.ReadJsonByString("SqlServerConnectionString"); } }
        DataSet ISqlServerTableHelper.ExecuteDataSet(string sql, params SqlParameter[] pms)
        {
            DataSet dataSet = new DataSet();
            using (conn = new SqlConnection(ConnectionString))
            {
                using (cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        if (pms != null && pms.Length > 0)
                        {
                            da.SelectCommand.Parameters.AddRange(pms);
                        }
                        da.Fill(dataSet);
                    }
                }
            }
            return dataSet;
        }

        DataTable ISqlServerTableHelper.ExecuteDataTable(string sql, params SqlParameter[] pms)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, ConnectionString))
            {
                if (pms != null && pms.Length > 0)
                {
                    adapter.SelectCommand.Parameters.AddRange(pms);
                }
                adapter.Fill(dt);
            }
            return dt;
        }

        int ISqlServerTableHelper.ExecuteNonQuery(string sql, params SqlParameter[] pms)
        {
            using (conn = new SqlConnection(ConnectionString))
            {
                using (cmd = new SqlCommand(sql, conn))
                {
                    if (pms != null && pms.Length > 0)
                    {
                        cmd.Parameters.AddRange(pms);
                    }
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        IDataReader ISqlServerTableHelper.ExecuteReader(string sql, params SqlParameter[] pms)
        {
            using (conn = new SqlConnection(ConnectionString))
            {
                using (cmd = new SqlCommand(sql, conn))
                {
                    if (pms != null && pms.Length > 0)
                    {
                        cmd.Parameters.AddRange(pms);
                    }
                    conn.Open();
                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
        }

        object ISqlServerTableHelper.ExecuteScalar(string sql, params SqlParameter[] pms)
        {
            using (conn = new SqlConnection(ConnectionString))
            {
                using (cmd = new SqlCommand(sql, conn))
                {
                    if (pms != null && pms.Length > 0)
                    {
                        cmd.Parameters.AddRange(pms);
                    }
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}
