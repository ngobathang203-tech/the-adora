using DoAnCN_Net.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// lấy tên khách hàng để vào góc trái khi user đăng nhập vào sảnh
namespace DoAnCN_Net
{
    public static class SessionManager
    {
        public static KhachHang CurrentUser { get; set; }
    }
}