using AppFramework;
using System;
using System.Data;
using System.Data.SqlClient;

namespace BAL
{
    public class AllocationSqlHelper
    {
        SqlCommand cmd = null;
        DataSet ds = null;
        SqlConnection conn = null;
        SqlDataAdapter da = null;
        const int SqlCommandTimeOut=300;

        public void StartConnection()
        {
            conn= new SqlConnection(DAL.connString);
            conn.Open();
        }

        public void CloseConnection()
        {           
            conn.Close();
        }

        public void ExecuteCommand(string command)
        {          
            cmd = new SqlCommand(command, conn) { CommandType = CommandType.Text, CommandTimeout = SqlCommandTimeOut };
            cmd.ExecuteNonQuery();
        }

        public DataTable GetDataTableUsingCommand(string command, CommandType sqlCommandType=CommandType.Text)
        {
            ds = new DataSet("Result");
            da = new SqlDataAdapter(new SqlCommand(command, conn) {  CommandType=sqlCommandType});
            da.Fill(ds);
            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public void ExecuteProcedure(string proc)
        {            
            cmd = new SqlCommand(proc, conn) { CommandType = CommandType.StoredProcedure, CommandTimeout = SqlCommandTimeOut };
            cmd.ExecuteNonQuery();           
        }

        public CustomeResponse SaveTableUsingBulkCopy(ref DataTable table, string destinationTable, int timeout = SqlCommandTimeOut)
        {
            try
            {
                SqlBulkCopy bulkCopy = new SqlBulkCopy(conn) { DestinationTableName = destinationTable, BulkCopyTimeout = timeout };
                bulkCopy.WriteToServer(table);
                return new CustomeResponse("Y", destinationTable + " Saved successfully", null);
            }
            catch (Exception ex)
            {
                return new CustomeResponse("N", ex.Message, null);
            }

        }
    }
}
