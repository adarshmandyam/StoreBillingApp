using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AC.Billing.DataAccess;
using System.Data;
namespace AC.Billing.Business
{
    public class UserBLL : BaseClassBLL
    {
        UserDAL userDAL;
        public UserBLL(DBHelper _dbHelper)
            : base(_dbHelper)
        {
            userDAL = new UserDAL(_dbHelper);
        }
        public DataTable GetUserDetails(string username, string password)
        {
            return userDAL.GetUserDetails(username, password);
        }

        public override void Dispose()
        {
            userDAL.Dispose();
        }
    }
}
