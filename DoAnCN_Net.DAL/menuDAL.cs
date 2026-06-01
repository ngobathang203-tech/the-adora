using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnCN_Net.DAL
{
    public class menuDAL
    {
        // ════════════════════════════════════════════════════════════════════
        //  MENU — READ
        // ════════════════════════════════════════════════════════════════════

        public List<Menu> GetAllMenus()
        {
            using (var db = dataDataContext.Create())
            {
                return db.Menus.ToList();
            }
        }

        public Menu GetById(int maMenu)
        {
            using (var db = dataDataContext.Create())
            {
                return db.Menus.FirstOrDefault(m => m.MaMenu == maMenu);
            }
        }

        public bool TenMenuDaTonTai(string tenMenu, int? editingMa)
        {
            using (var db = dataDataContext.Create())
            {
                return db.Menus.Any(m =>
                    m.TenMenu == tenMenu &&
                    (editingMa == null || m.MaMenu != editingMa.Value));
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  MENU — WRITE
        // ════════════════════════════════════════════════════════════════════

        public void Insert(Menu menu)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    db.Menus.InsertOnSubmit(menu);
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi thêm menu: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Thêm menu + danh sách món trong 1 transaction.
        /// Trả về MaMenu vừa tạo.
        /// </summary>
        public int InsertWithMonAn(string tenMenu, string loaiMenu, List<int> dsMaMon)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    var menu = new Menu { TenMenu = tenMenu, LoaiMenu = loaiMenu };
                    db.Menus.InsertOnSubmit(menu);
                    db.SubmitChanges(); // flush để lấy MaMenu

                    foreach (var maMon in dsMaMon)
                        db.ChiTietMenus.InsertOnSubmit(new ChiTietMenu
                        {
                            MaMenu = menu.MaMenu,
                            MaMon = maMon
                        });

                    db.SubmitChanges();
                    return menu.MaMenu;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi thêm menu: {ex.Message}");
                }
            }
        }

        public void Update(int maMenu, string tenMenu, string loaiMenu)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    var existing = db.Menus.FirstOrDefault(m => m.MaMenu == maMenu);
                    if (existing == null)
                        throw new Exception("Không tìm thấy menu.");

                    existing.TenMenu = tenMenu;
                    existing.LoaiMenu = loaiMenu;
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi cập nhật menu: {ex.Message}");
                }
            }
        }


        /// Cập nhật menu + thay toàn bộ danh sách món trong 1 transaction.
        public void UpdateWithMonAn(int maMenu, string tenMenu, string loaiMenu, List<int> dsMaMon)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    // Cập nhật tên/loại
                    var existing = db.Menus.FirstOrDefault(m => m.MaMenu == maMenu);
                    if (existing == null)
                        throw new Exception("Không tìm thấy menu.");

                    existing.TenMenu = tenMenu;
                    existing.LoaiMenu = loaiMenu;

                    // Xóa chi tiết cũ
                    var oldItems = db.ChiTietMenus
                                     .Where(c => c.MaMenu == maMenu)
                                     .ToList();
                    db.ChiTietMenus.DeleteAllOnSubmit(oldItems);

                    // Thêm chi tiết mới
                    foreach (var maMon in dsMaMon)
                        db.ChiTietMenus.InsertOnSubmit(new ChiTietMenu
                        {
                            MaMenu = maMenu,
                            MaMon = maMon
                        });

                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi cập nhật menu: {ex.Message}");
                }
            }
        }

        public void Delete(int maMenu)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    // Xóa chi tiết trước
                    var chiTiet = db.ChiTietMenus
                                    .Where(c => c.MaMenu == maMenu)
                                    .ToList();
                    db.ChiTietMenus.DeleteAllOnSubmit(chiTiet);

                    var menu = db.Menus.FirstOrDefault(m => m.MaMenu == maMenu);
                    if (menu == null)
                        throw new Exception("Không tìm thấy menu.");

                    db.Menus.DeleteOnSubmit(menu);
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi xóa menu: {ex.Message}");
                }
            }
        }

        //  CHI TIẾT MENU
        public List<MonAn> GetMonAnByMenu(int maMenu)
        {
            using (var db = dataDataContext.Create())
            {
                return (from ctm in db.ChiTietMenus
                        join mon in db.MonAns on ctm.MaMon equals mon.MaMon
                        where ctm.MaMenu == maMenu
                        select mon).ToList();
            }
        }

        public List<int> GetMaMonByMenu(int maMenu)
        {
            using (var db = dataDataContext.Create())
            {
                return db.ChiTietMenus
                         .Where(c => c.MaMenu == maMenu)
                         .Select(c => c.MaMon)
                         .ToList();
            }
        }

        public void InsertChiTietMenu(int maMenu, int maMon)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    db.ChiTietMenus.InsertOnSubmit(new ChiTietMenu
                    {
                        MaMenu = maMenu,
                        MaMon = maMon
                    });
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi thêm món vào menu: {ex.Message}");
                }
            }
        }

        public void DeleteChiTietMenu(int maMenu, int maMon)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    var item = db.ChiTietMenus
                                 .FirstOrDefault(c => c.MaMenu == maMenu && c.MaMon == maMon);
                    if (item == null)
                        throw new Exception("Không tìm thấy món trong menu.");

                    db.ChiTietMenus.DeleteOnSubmit(item);
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi xóa món khỏi menu: {ex.Message}");
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  THỐNG KÊ
        // ════════════════════════════════════════════════════════════════════

        public Dictionary<int, int> GetSoMonByMenu()
        {
            using (var db = dataDataContext.Create())
            {
                return db.ChiTietMenus
                         .GroupBy(c => c.MaMenu)
                         .ToDictionary(g => g.Key, g => g.Count());
            }
        }
    }
}