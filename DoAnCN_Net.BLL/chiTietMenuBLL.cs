using DoAnCN_Net.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnCN_Net.BLL
{
    public class chiTietMenuBLL
    {
        private readonly chiTietMenuDAL _dal = new chiTietMenuDAL();

        // ════════════════════════════════════════════════════════════════════
        //  READ
        // ════════════════════════════════════════════════════════════════════

        /// Trả về tất cả món ăn (dùng để hiển thị danh sách thêm)
        public List<MonAn> GetAllMonAn()
        {
            return _dal.GetAllMonAn();
        }

        /// Trả về các món đã có trong menu
        public List<MonAn> GetMonAnByMenu(int maMenu)
        {
            return _dal.GetMonAnByMenu(maMenu);
        }

        // ════════════════════════════════════════════════════════════════════
        //  WRITE
        // ════════════════════════════════════════════════════════════════════

        /// Thêm một món vào menu (có kiểm tra trùng)
        public void AddMonVaoMenu(int maMenu, int maMon)
        {
            var hiện = _dal.GetMonAnByMenu(maMenu);
            if (hiện.Any(m => m.MaMon == maMon))
                throw new Exception("Món ăn này đã có trong menu rồi.");

            _dal.InsertChiTietMenu(maMenu, maMon);
        }

        /// Xóa một món khỏi menu
        public void RemoveMonKhoiMenu(int maMenu, int maMon)
        {
            _dal.DeleteChiTietMenu(maMenu, maMon);
        }
    }
}