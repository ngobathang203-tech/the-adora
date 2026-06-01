using System.Collections.Generic;
using System.Linq;
using System;

namespace DoAnCN_Net.DAL
{
    public class chiTietMenuDAL
    {
        public List<MonAn> GetAllMonAn()
        {
            using (var db = dataDataContext.Create())
            {
                return db.MonAns.ToList();
            }
        }

        public List<MonAn> GetMonAnByMenu(int maMenu)
        {
            using (var db = dataDataContext.Create())
            {
                return (from c in db.ChiTietMenus
                        join m in db.MonAns on c.MaMon equals m.MaMon
                        where c.MaMenu == maMenu
                        orderby m.TenMon
                        select m).ToList();
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
                    throw new Exception($"Lỗi khi thêm chi tiết menu: {ex.Message}");
                }
            }
        }

        public void DeleteChiTietMenu(int maMenu, int maMon)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    var entity = db.ChiTietMenus
                                   .FirstOrDefault(c => c.MaMenu == maMenu && c.MaMon == maMon);
                    if (entity == null) return;
                    db.ChiTietMenus.DeleteOnSubmit(entity);
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi xóa chi tiết menu: {ex.Message}");
                }
            }
        }
    }
}