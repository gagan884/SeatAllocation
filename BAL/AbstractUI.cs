using AppFramework;
using System.Data;

namespace BAL
{
    public abstract class AbstractUI : IUserInterface
    
    {
        public abstract string GetHeader();
        public abstract string GetSubheader();

        public virtual ActionOutput Validate(string boardId, string userId, string password, int roundno)
        {
            DataTable dt = new DAL().GetDataTableUsingCommand("Select boardId,userId,pwd,maxRoundNo,userRole from dbo.XT_Administrator where boardId='" + boardId + "' and userId='" + userId + "' and pwd='" + password + "'");
            if (dt == null || dt.Rows.Count == 0)
            {
                return new ActionOutput(ActionStatus.Failed, "Kindly enter login credentials");
            }
            else
            {
                return new ActionOutput(ActionStatus.Success, "Valid", dt.Rows[0]["userRole"].ToString(),DataType.StringValue);
            }
        }
    }
}
