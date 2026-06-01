using DoAnCN_Net.DAL;
using System.Collections.Generic;

namespace DoAnCN_Net.BLL
{
    public class LoginBLL
    {
        KhachHangDAL dal = new KhachHangDAL();

        public KhachHang CheckLogin(string user, string pass)
        {
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                return null;

            return dal.CheckLogin(user, pass);
        }

        //public List<DatSanh> GetDashboardData()
        //{
        //    return dal.GetTopDatSanh();
        //}
    }
}