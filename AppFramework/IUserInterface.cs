using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework
{
    public interface IUserInterface
    {
        ActionOutput Validate(string boardId,string userId, string password, int roundno);
        string GetHeader();
        string GetSubheader();
    }
}
