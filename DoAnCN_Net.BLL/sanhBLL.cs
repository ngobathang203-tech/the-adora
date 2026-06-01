using DoAnCN_Net.DAL;
using System;
using System.Collections.Generic;

namespace DoAnCN_Net.BLL
{
    public class sanhBLL
    {
        private readonly sanhDAL dal = new sanhDAL();

        public List<Sanh> GetAll()
        {
            return dal.GetAll();
        }

        public bool Add(Sanh sanh)
        {
            if (string.IsNullOrWhiteSpace(sanh.TenSanh))
                throw new Exception("Tên sảnh không được để trống!");

            if (sanh.SucChua <= 0)
                throw new Exception("Sức chứa phải lớn hơn 0!");

            if (sanh.GiaThue < 0)
                throw new Exception("Giá thuê không được âm!");

            if (sanh.MaChiNhanh <= 0)
                throw new Exception("Vui lòng chọn chi nhánh!");

            return dal.Add(sanh);
        }

        public bool Update(Sanh sanh)
        {
            if (sanh.MaSanh <= 0)
                throw new Exception("Mã sảnh không hợp lệ!");

            if (string.IsNullOrWhiteSpace(sanh.TenSanh))
                throw new Exception("Tên sảnh không được để trống!");

            if (sanh.SucChua <= 0)
                throw new Exception("Sức chứa phải lớn hơn 0!");

            if (sanh.GiaThue < 0)
                throw new Exception("Giá thuê không được âm!");

            if (sanh.MaChiNhanh <= 0)
                throw new Exception("Vui lòng chọn chi nhánh!");

            return dal.Update(sanh);
        }

        public bool Delete(int maSanh)
        {
            if (maSanh <= 0)
                throw new Exception("Mã sảnh không hợp lệ!");

            return dal.Delete(maSanh);
        }

        public Sanh GetById(int maSanh)
        {
            return dal.GetById(maSanh);
        }

        public bool IsBeingUsed(int maSanh)
        {
            return dal.IsBeingUsed(maSanh);
        }
    }
}