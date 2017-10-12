using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC.Billing.DataAccess
{
    public class UserDAL : BaseClassDAL
    {
        public UserDAL(DBHelper _dbHelper)
            : base(_dbHelper)
        { }

        public DataTable GetUserDetails(string username , string password)
        {
           
            dbHelper.SelectParameterClear();
            dbHelper.SelectParameter("@UserName", DbType.String, username);
            dbHelper.SelectParameter("@Password", DbType.String, password);
            DataTable dtUser = dbHelper.GetDataInDataTable(CommandType.StoredProcedure, "Get_UserDetails");
            return dtUser;
        }
    }
}
