using AppFramework;
using System;
using System.Data;

namespace BAL
{
    public class ComCouns21UI : AbstractUI
    {
        public override string GetHeader()
        {
            return "Common Counselling Application";
        }

        public override string GetSubheader()
        {
            return "Seat Allocation";
        }

        public override ActionOutput Validate(string boardId, string userId, string password, int roundno)
        {
            //select 1 from Administrator where Role in ('BOARDADMIN','NICADMIN') and isActive='Y' and  UserId='LOVEE.ARORA' and HashPass=CONVERT(NVARCHAR(64),HashBytes('sha2_256', 'Ccmt@nic2019'),2)
            
            string cmd = @"select name userName,defaultRoleId + ',' + ISNULL(AdditionalRoleIds, '') userRole from App_Administrator
  where(defaultRoleId + ',' + ISNULL(AdditionalRoleIds, '') like '%BOARDADMIN%'
     or defaultRoleId + ',' + ISNULL(AdditionalRoleIds, '') like '%NICADMIN%')
   and isActive = 'Y' and UserId = '"+userId+"' and password = CONVERT(NVARCHAR(64), HashBytes('sha2_256', '"+password+"'), 2)";
            //string cmd = "select role from XT_Administrator where Role in ('admin','verify') and isActive='Y' and  UserId='" + userId + "' and HashPass=CONVERT(NVARCHAR(64),HashBytes('sha2_256', '" + password + "'),2)";
            DataTable dt = new DAL().GetDataTableUsingCommand(cmd);
            if (dt == null || dt.Rows.Count == 0)
            {
                return new ActionOutput(ActionStatus.Failed, "Kindly enter login credentials");
            }
            else
            {
                //return new CustomeResponse("PS", "Valid", dt.Rows[0]["Role"].ToString().Equals("NICADMIN") ? "admin" : "verify");
                return new ActionOutput(ActionStatus.Success, "Valid",  "admin",DataType.StringValue );
            }
        }
    }
}
