using System.Collections.Generic;
using DoAnCN_Net;

public class ThongKeBLL
{
    ThongKeDAL dal = new ThongKeDAL();

    public List<dynamic> GetDoanhThu()
    {
        return dal.GetDoanhThu();
    }

    public List<dynamic> GetTrangThai()
    {
        return dal.GetTrangThai();
    }
}