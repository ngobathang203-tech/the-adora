using DoAnCN_Net.DAL;

namespace DoAnCN_Net.BLL
{
    public class user_KhachHangBLL
    {
        private user_KhachHangDAL khachHangDAL = new user_KhachHangDAL();

        // Lấy thông tin khách hàng
        public KhachHang GetThongTinKhachHang(int maKhachHang)
        {
            if (maKhachHang <= 0)
                return null;

            return khachHangDAL.GetKhachHangById(maKhachHang);
        }

        // Lấy tên khách hàng
        public string GetTenKhachHang(int maKhachHang)
        {
            var kh = GetThongTinKhachHang(maKhachHang);
            return kh?.TenKhachHang ?? "Khách hàng";
        }
    }
}