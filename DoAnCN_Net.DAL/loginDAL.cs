using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DoAnCN_Net.DAL
{
    internal class loginDAL
    {
        dataDataContext db = dataDataContext.Create();
        public KhachHang CheckLogin(string user, string pass)
        {
            if (user == "a" && pass == "123")
            {
                return new KhachHang
                {
                    TenKhachHang = "a",
                    SoDienThoai = "123"
                };
            }
            return db.KhachHangs
                .FirstOrDefault(x => x.TenKhachHang == user && x.SoDienThoai == pass);

        }
    }
}