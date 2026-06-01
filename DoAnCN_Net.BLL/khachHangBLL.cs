using DoAnCN_Net.DAL;
using System;
using System.Collections.Generic;

namespace DoAnCN_Net.BLL
{
    public class khachHangBLL
    {
        KhachHangDAL dal = new KhachHangDAL();

        public List<KhachHang> GetAll()
        {
            return dal.GetAll();
        }

        public KhachHang GetById(int maKhachHang)
        {
            if (maKhachHang <= 0)
                throw new Exception("Mã khách hàng không hợp lệ!");

            return dal.GetById(maKhachHang);
        }

        public List<KhachHang> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetAll();

            return dal.Search(keyword);
        }

        public bool Add(KhachHang khachHang)
        {
            // Validate dữ liệu
            if (string.IsNullOrWhiteSpace(khachHang.TenKhachHang))
                throw new Exception("Tên khách hàng không được để trống!");

            if (string.IsNullOrWhiteSpace(khachHang.SoDienThoai))
                throw new Exception("Số điện thoại không được để trống!");

            // Kiểm tra số điện thoại đã tồn tại
            if (dal.IsPhoneNumberExists(khachHang.SoDienThoai))
                throw new Exception("Số điện thoại đã tồn tại trong hệ thống!");

            // Kiểm tra email nếu có
            if (!string.IsNullOrEmpty(khachHang.Email) && dal.IsEmailExists(khachHang.Email))
                throw new Exception("Email đã tồn tại trong hệ thống!");

            return dal.Add(khachHang);
        }

        public bool Update(KhachHang khachHang)
        {
            if (khachHang.MaKhachHang <= 0)
                throw new Exception("Mã khách hàng không hợp lệ!");

            if (string.IsNullOrWhiteSpace(khachHang.TenKhachHang))
                throw new Exception("Tên khách hàng không được để trống!");

            if (string.IsNullOrWhiteSpace(khachHang.SoDienThoai))
                throw new Exception("Số điện thoại không được để trống!");

            // Kiểm tra số điện thoại đã tồn tại (trừ chính nó)
            if (dal.IsPhoneNumberExists(khachHang.SoDienThoai, khachHang.MaKhachHang))
                throw new Exception("Số điện thoại đã tồn tại trong hệ thống!");

            // Kiểm tra email nếu có
            if (!string.IsNullOrEmpty(khachHang.Email) && dal.IsEmailExists(khachHang.Email, khachHang.MaKhachHang))
                throw new Exception("Email đã tồn tại trong hệ thống!");

            return dal.Update(khachHang);
        }

        public bool Delete(int maKhachHang)
        {
            if (maKhachHang <= 0)
                throw new Exception("Mã khách hàng không hợp lệ!");

            return dal.Delete(maKhachHang);
        }

        public List<DatSanh> GetBookingHistory(int maKhachHang)
        {
            if (maKhachHang <= 0)
                throw new Exception("Mã khách hàng không hợp lệ!");

            return dal.GetBookingHistory(maKhachHang);
        }
    }
}