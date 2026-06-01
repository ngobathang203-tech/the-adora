using DoAnCN_Net.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnCN_Net.BLL
{
    public class monAnBLL
    {
        private readonly monAnDAL _dal = new monAnDAL();

        public List<MonAn> GetAll() => _dal.GetAll();

        public MonAn GetById(int maMon) => _dal.GetById(maMon);

        public void AddMonAn(string tenMon, decimal gia)
        {
            Validate(tenMon, gia);
            _dal.Add(new MonAn { TenMon = tenMon, Gia = gia });
        }

        public void UpdateMonAn(int maMon, string tenMon, decimal gia)
        {
            Validate(tenMon, gia);
            _dal.Update(new MonAn { MaMon = maMon, TenMon = tenMon, Gia = gia });
        }

        public void DeleteMonAn(int maMon)
        {
            _dal.Delete(maMon);
        }

        private static void Validate(string tenMon, decimal gia)
        {
            if (string.IsNullOrWhiteSpace(tenMon))
                throw new Exception("Tên món không được để trống.");
            if (gia < 0)
                throw new Exception("Giá món không được âm.");
        }
        /// Đếm số menu đang dùng mỗi món (dùng để hiển thị cột "Dùng trong" và chặn xóa)
        public Dictionary<int, int> GetSoMenuChuaMon()
        {
            using (var db = dataDataContext.Create())
            {
                return db.ChiTietMenus
                         .GroupBy(c => c.MaMon)
                         .ToDictionary(g => g.Key, g => g.Count());
            }
        }
    }
}