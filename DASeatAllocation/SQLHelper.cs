using AppFramework;
using BAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DASeatAllocation
{
    class SQLHelper
    {
        SqlCommand cmd = null;
        DataSet ds = null;
        SqlConnection conn = null;
        SqlDataAdapter da = null;

        public void StartConnection()
        {
            conn= new SqlConnection(ObjectFactory.GetCommonObject().GetConnectionString());
            conn.Open();
        }

        public void CloseConnection()
        {           
            conn.Close();
        }

        public void ExecuteCommand(string command)
        {          
            cmd = new SqlCommand(command, conn) { CommandType = CommandType.Text, CommandTimeout = AppConfig.SqlTimeOut };
            cmd.ExecuteNonQuery();
        }

        public DataTable GetDataTableUsingCommand(string command)
        {
            ds = new DataSet("Result");
            da = new SqlDataAdapter(new SqlCommand(command,conn));
            da.Fill(ds);
            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public void ExecuteProcedure(string proc)
        {            
            cmd = new SqlCommand(proc, conn) { CommandType = CommandType.StoredProcedure, CommandTimeout = AppConfig.SqlTimeOut };
            cmd.ExecuteNonQuery();           
        }

        public CustomeResponse SaveTableUsingBulkCopy(ref DataTable table, string destinationTable, int timeout = 10)
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
