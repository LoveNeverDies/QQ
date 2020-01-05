using Newbe.Mahua.Plugins.Parrot.Helper;
using System.Data;
using System.Data.SqlClient;

namespace Newbe.Mahua.Plugins.Parrot.HelperService
{
    public interface ISQLServerTableHelper
    {
        /// <summary>
        /// 增删改
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string sql, params SqlParameter[] pms);

        /// <summary>
        /// 获取第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        object ExecuteScalar(string sql, params SqlParameter[] pms);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        IDataReader ExecuteReader(string sql, params SqlParameter[] pms);

        /// <summary>
        /// 返回datatable格式的list数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        DataTable ExecuteDataTable(string sql, params SqlParameter[] pms);

        /// <summary>
        /// 返回dataset格式的list数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pms"></param>
        /// <returns></returns>
        DataSet ExecuteDataSet(string sql, params SqlParameter[] pms);
    }
    class SQLServerTableHelper : ISQLServerTableHelper
    {
        IJsonHelper jsonHelper = null;
        SqlConnection conn = null;
        SqlCommand cmd = null;
        public SQLServerTableHelper()
        {
            jsonHelper = new JsonHelper();
        }
        string ConnectionString { get { return jsonHelper.ReadJsonByString("SQLServerConnectionString"); } }

        DataSet ISQLServerTableHelper.ExecuteDataSet(string sql, params SqlParameter[] pms)
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

        DataTable ISQLServerTableHelper.ExecuteDataTable(string sql, params SqlParameter[] pms)
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

        int ISQLServerTableHelper.ExecuteNonQuery(string sql, params SqlParameter[] pms)
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

        IDataReader ISQLServerTableHelper.ExecuteReader(string sql, params SqlParameter[] pms)
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

        object ISQLServerTableHelper.ExecuteScalar(string sql, params SqlParameter[] pms)
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
