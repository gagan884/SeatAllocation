using AppFramework;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BAL
{
    public class DAL
    {
        public static string connString = null;
        public static string boardId = null;
        public static int roundNo = 0;

        SqlCommand cmd = null;
        DataSet ds = null;
        SqlConnection conn = null;
        SqlDataAdapter da = null;
        int DBSqlTimeOut = 300;
        public void ExecuteCommand(string command)
        {
            conn = new SqlConnection(connString);
            conn.Open();
            cmd = new SqlCommand(command, conn) { CommandType = CommandType.Text, CommandTimeout = DBSqlTimeOut };
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public DataTable GetDataTableUsingCommand(string command)
        {
            ds = new DataSet("Result");
            da = new SqlDataAdapter(new SqlCommand(command, new SqlConnection(connString)) { CommandTimeout = DBSqlTimeOut });
            da.Fill(ds);
            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public void ExecuteProcedure(string proc)
        {
            conn = new SqlConnection(connString);
            conn.Open();
            cmd = new SqlCommand(proc, conn) { CommandType = CommandType.StoredProcedure, CommandTimeout = DBSqlTimeOut };
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public CustomeResponse SaveTableUsingBulkCopy(ref DataTable table, string destinationTable, int timeout = 10)
        {
            try
            {
                conn = new SqlConnection(connString);
                conn.Open();
                SqlBulkCopy bulkCopy = new SqlBulkCopy(conn) { DestinationTableName = destinationTable, BulkCopyTimeout = timeout };
                bulkCopy.WriteToServer(table);
                conn.Close();
                return new CustomeResponse("Y", destinationTable + " Saved successfully", null);
            }
            catch (Exception ex)
            {
                conn.Close();
                return new CustomeResponse("N", ex.Message, null);
            }

        }


    }
}
