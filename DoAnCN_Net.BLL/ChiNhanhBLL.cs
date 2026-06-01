//using DoAnCN_Net.DAL;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace DoAnCN_Net.BLL
//{
//    public class ChiNhanhBLL
//    {
//        ChiNhanhDAL dal = new ChiNhanhDAL();

//        public List<ChiNhanh> GetAll()
//        {
//            try
//            {
//                return dal.GetAll();
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"Lỗi: {ex.Message}");
//            }
//        }

//        public ChiNhanh GetById(int maChiNhanh)
//        {
//            if (maChiNhanh <= 0)
//                throw new Exception("Mã chi nhánh không hợp lệ!");

//            try
//            {
//                return dal.GetById(maChiNhanh);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"Lỗi: {ex.Message}");
//            }
//        }

//        public bool Add(ChiNhanh chiNhanh)
//        {
//            // Validation dữ liệu
//            if (chiNhanh == null)
//                throw new Exception("Dữ liệu chi nhánh không hợp lệ!");

//            if (string.IsNullOrWhiteSpace(chiNhanh.TenChiNhanh))
//                throw new Exception("Tên chi nhánh không được để trống!");

//            if (string.IsNullOrWhiteSpace(chiNhanh.DiaChi))
//                throw new Exception("Địa chỉ không được để trống!");

//            if (string.IsNullOrWhiteSpace(chiNhanh.SoDienThoai))
//                throw new Exception("Số điện thoại không được để trống!");

//            // Validate số điện thoại
//            if (!IsValidPhoneNumber(chiNhanh.SoDienThoai))
//                throw new Exception("Số điện thoại không hợp lệ! (Phải có 10-11 số)");

//            // Validate tên chi nhánh (không chứa ký tự đặc biệt)
//            if (HasSpecialCharacters(chiNhanh.TenChiNhanh))
//                throw new Exception("Tên chi nhánh không được chứa ký tự đặc biệt!");

//            try
//            {
//                return dal.Insert(chiNhanh);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"Lỗi khi thêm: {ex.Message}");
//            }
//        }

//        public bool Update(ChiNhanh chiNhanh)
//        {
//            // Validation dữ liệu
//            if (chiNhanh == null)
//                throw new Exception("Dữ liệu chi nhánh không hợp lệ!");

//            if (chiNhanh.MaChiNhanh <= 0)
//                throw new Exception("Mã chi nhánh không hợp lệ!");

//            if (string.IsNullOrWhiteSpace(chiNhanh.TenChiNhanh))
//                throw new Exception("Tên chi nhánh không được để trống!");

//            if (string.IsNullOrWhiteSpace(chiNhanh.DiaChi))
//                throw new Exception("Địa chỉ không được để trống!");

//            if (string.IsNullOrWhiteSpace(chiNhanh.SoDienThoai))
//                throw new Exception("Số điện thoại không được để trống!");

//            // Validate số điện thoại
//            if (!IsValidPhoneNumber(chiNhanh.SoDienThoai))
//                throw new Exception("Số điện thoại không hợp lệ! (Phải có 10-11 số)");

//            try
//            {
//                return dal.Update(chiNhanh);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"Lỗi khi cập nhật: {ex.Message}");
//            }
//        }

//        public bool Delete(int id)
//        {
//            if (id <= 0)
//                throw new Exception("Mã chi nhánh không hợp lệ!");

//            try
//            {
//                return dal.Delete(id);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"Lỗi khi xóa: {ex.Message}");
//            }
//        }

//        public List<ChiNhanh> Search(string keyword)
//        {
//            try
//            {
//                return dal.Search(keyword);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"Lỗi khi tìm kiếm: {ex.Message}");
//            }
//        }

//        public int GetSoLuongSanh(int maChiNhanh)
//        {
//            if (maChiNhanh <= 0)
//                return 0;

//            try
//            {
//                return dal.GetSoLuongSanh(maChiNhanh);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"Lỗi khi lấy số lượng sảnh: {ex.Message}");
//            }
//        }

//        // Private helper methods for validation
//        private bool IsValidPhoneNumber(string phoneNumber)
//        {
//            if (string.IsNullOrWhiteSpace(phoneNumber))
//                return false;

//            // Loại bỏ khoảng trắng và dấu gạch ngang
//            phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");

//            // Kiểm tra độ dài 10-11 số và bắt đầu bằng 0
//            if (phoneNumber.Length < 10 || phoneNumber.Length > 11)
//                return false;

//            if (!phoneNumber.StartsWith("0"))
//                return false;

//            // Kiểm tra tất cả là số
//            foreach (char c in phoneNumber)
//            {
//                if (!char.IsDigit(c))
//                    return false;
//            }

//            return true;
//        }

//        private bool HasSpecialCharacters(string text)
//        {
//            if (string.IsNullOrWhiteSpace(text))
//                return false;

//            // Các ký tự đặc biệt không cho phép
//            char[] specialChars = { '@', '#', '$', '%', '^', '&', '*', '!', '=', '+', '\\', '/', '|', '<', '>', '~', '`' };

//            foreach (char c in specialChars)
//            {
//                if (text.Contains(c))
//                    return true;
//            }

//            return false;
//        }
//    }
//}
using DoAnCN_Net.DAL;
using System;
using System.Collections.Generic;

namespace DoAnCN_Net.BLL
{
    
    public class ChiNhanhBLL
    {
        ChiNhanhDAL dal = new ChiNhanhDAL();

        public List<ChiNhanh> GetAll()
        {
            return dal.GetAll();
        }

        public ChiNhanh GetById(int maChiNhanh)
        {
            if (maChiNhanh <= 0)
                throw new Exception("Mã chi nhánh không hợp lệ!");
            return dal.GetById(maChiNhanh);
        }

        public bool Add(ChiNhanh chiNhanh)
        {
            if (chiNhanh == null)
                throw new Exception("Dữ liệu không hợp lệ!");

            if (string.IsNullOrWhiteSpace(chiNhanh.TenChiNhanh))
                throw new Exception("Tên chi nhánh không được để trống!");

            if (string.IsNullOrWhiteSpace(chiNhanh.DiaChi))
                throw new Exception("Địa chỉ không được để trống!");

            if (string.IsNullOrWhiteSpace(chiNhanh.SoDienThoai))
                throw new Exception("Số điện thoại không được để trống!");

            return dal.Add(chiNhanh);
        }

        public bool Update(ChiNhanh chiNhanh)
        {
            if (chiNhanh == null)
                throw new Exception("Dữ liệu không hợp lệ!");

            if (chiNhanh.MaChiNhanh <= 0)
                throw new Exception("Mã chi nhánh không hợp lệ!");

            if (string.IsNullOrWhiteSpace(chiNhanh.TenChiNhanh))
                throw new Exception("Tên chi nhánh không được để trống!");

            if (string.IsNullOrWhiteSpace(chiNhanh.DiaChi))
                throw new Exception("Địa chỉ không được để trống!");

            if (string.IsNullOrWhiteSpace(chiNhanh.SoDienThoai))
                throw new Exception("Số điện thoại không được để trống!");

            return dal.Update(chiNhanh);
        }

        public bool Delete(int id)
        {
            if (id <= 0)
                throw new Exception("Mã chi nhánh không hợp lệ!");
            return dal.Delete(id);
        }
    }
}