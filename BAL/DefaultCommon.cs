using AppFramework;
using System.Configuration;

namespace BAL
{
    public class DefaultCommon : ICommon
    {
        public virtual string GetBoardID()
        {
            return DAL.boardId;
        }

        public virtual string GetConnectionString()
        {
            return DAL.connString;
        }

        
    }
}
