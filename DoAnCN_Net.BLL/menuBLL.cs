using DoAnCN_Net.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnCN_Net.BLL
{
    public class menuBLL
    {
        private readonly menuDAL _dal = new menuDAL();

        // ════════════════════════════════════════════════════════════════════
        //  READ
        // ════════════════════════════════════════════════════════════════════

        public List<Menu> GetAllMenus() => _dal.GetAllMenus();

        public Menu GetById(int maMenu) => _dal.GetById(maMenu);

        public Dictionary<int, int> GetSoMonByMenu() => _dal.GetSoMonByMenu();

        public List<MonAn> GetMonAnByMenu(int maMenu) => _dal.GetMonAnByMenu(maMenu);

        /// Alias dùng cho user_XemChiTietMenuTrongDatSanh
        public List<MonAn> GetChiTietMenu(int maMenu) => _dal.GetMonAnByMenu(maMenu);

        /// Lấy danh sách MaMon đã có trong menu (dùng để tick sẵn khi edit)
        public List<int> GetMaMonByMenu(int maMenu) => _dal.GetMaMonByMenu(maMenu);

        /// Lấy toàn bộ món ăn (dùng cho CheckBox list trong dialog)
        public List<MonAn> GetAllMonAn()
        {
            using (var db = dataDataContext.Create())
            {
                return db.MonAns.ToList();
            }
        }

        /// Lọc + tìm kiếm phía BLL (không cần round-trip DB)
        public List<Menu> Filter(List<Menu> source, string keyword, string loai)
        {
            IEnumerable<Menu> query = source;

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(m =>
                    m.TenMenu.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);

            if (loai != "Tất cả")
                query = query.Where(m => m.LoaiMenu == loai);

            return query.ToList();
        }

        // ════════════════════════════════════════════════════════════════════
        //  WRITE — Menu (giữ overload cũ, thêm overload kèm món)
        // ════════════════════════════════════════════════════════════════════

        /// Thêm menu không kèm món (dùng ở các WPF khác)
        public void AddMenu(string tenMenu, string loaiMenu)
        {
            Validate(tenMenu, loaiMenu, editingMa: null);
            _dal.Insert(new Menu { TenMenu = tenMenu.Trim(), LoaiMenu = loaiMenu });
        }

        /// Thêm menu kèm danh sách món (dùng cho MenuDialog mới)
        public void AddMenu(string tenMenu, string loaiMenu, List<int> dsMaMon)
        {
            Validate(tenMenu, loaiMenu, editingMa: null);
            ValidateDsMon(dsMaMon);
            _dal.InsertWithMonAn(tenMenu.Trim(), loaiMenu, dsMaMon);
        }

        /// Cập nhật menu không kèm món (dùng ở các WPF khác)
        public void UpdateMenu(int maMenu, string tenMenu, string loaiMenu)
        {
            Validate(tenMenu, loaiMenu, editingMa: maMenu);
            _dal.Update(maMenu, tenMenu.Trim(), loaiMenu);
        }

        /// Cập nhật menu kèm danh sách món (dùng cho MenuDialog mới)
        public void UpdateMenu(int maMenu, string tenMenu, string loaiMenu, List<int> dsMaMon)
        {
            Validate(tenMenu, loaiMenu, editingMa: maMenu);
            ValidateDsMon(dsMaMon);
            _dal.UpdateWithMonAn(maMenu, tenMenu.Trim(), loaiMenu, dsMaMon);
        }

        public void DeleteMenu(int maMenu) => _dal.Delete(maMenu);

        // ════════════════════════════════════════════════════════════════════
        //  WRITE — ChiTietMenu
        // ════════════════════════════════════════════════════════════════════

        public void AddMonVaoMenu(int maMenu, int maMon)
        {
            var monHienCo = _dal.GetMonAnByMenu(maMenu);
            if (monHienCo.Any(m => m.MaMon == maMon))
                throw new Exception("Món ăn này đã có trong menu rồi.");

            _dal.InsertChiTietMenu(maMenu, maMon);
        }

        public void RemoveMonKhoiMenu(int maMenu, int maMon)
            => _dal.DeleteChiTietMenu(maMenu, maMon);

        // ════════════════════════════════════════════════════════════════════
        //  STATISTICS
        // ════════════════════════════════════════════════════════════════════

        public int TongSoMenu(List<Menu> list) => list.Count;
        public int SoMenuChay(List<Menu> list) => list.Count(m => m.LoaiMenu == "Chay");
        public int SoMenuMan(List<Menu> list) => list.Count(m => m.LoaiMenu == "Mặn");
        public decimal TongGiaMenu(List<MonAn> mons) => mons.Sum(m => m.Gia ?? 0m);

        // ════════════════════════════════════════════════════════════════════
        //  VALIDATE
        // ════════════════════════════════════════════════════════════════════

        private void Validate(string tenMenu, string loaiMenu, int? editingMa)
        {
            if (string.IsNullOrWhiteSpace(tenMenu))
                throw new Exception("Tên menu không được để trống.");

            if (tenMenu.Trim().Length < 3)
                throw new Exception("Tên menu phải có ít nhất 3 ký tự.");

            if (string.IsNullOrWhiteSpace(loaiMenu))
                throw new Exception("Vui lòng chọn loại menu.");

            if (_dal.TenMenuDaTonTai(tenMenu.Trim(), editingMa))
                throw new Exception($"Tên menu \"{tenMenu.Trim()}\" đã tồn tại.");
        }

        private static void ValidateDsMon(List<int> dsMaMon)
        {
            if (dsMaMon == null || dsMaMon.Count == 0)
                throw new Exception("Vui lòng chọn ít nhất một món ăn.");
        }
    }
}