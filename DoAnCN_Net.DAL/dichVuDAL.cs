    using System;
    using System.Collections.Generic;
    using System.Linq;
    namespace DoAnCN_Net.DAL
    {
        public class dichVuDAL
        {
            public List<DichVu> GetAll()
            {
                using (var db = dataDataContext.Create())
                {
                    return db.DichVus.ToList();
                }
            }
            public DichVu GetById(int maDichVu)
            {
                using (var db = dataDataContext.Create())
                {
                    return db.DichVus.FirstOrDefault(d => d.MaDichVu == maDichVu);
                }
            }
            public bool TenDaTonTai(string tenDichVu, int? editingMa)
            {
                using (var db = dataDataContext.Create())
                {
                    return db.DichVus.Any(d =>
                        d.TenDichVu == tenDichVu &&
                        (editingMa == null || d.MaDichVu != editingMa.Value));
                }
            }
            public bool Add(DichVu dv)
            {
                using (var db = dataDataContext.Create())
                {
                    try
                    {
                        db.DichVus.InsertOnSubmit(dv);
                        db.SubmitChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Lỗi khi thêm: {ex.Message}");
                    }
                }
            }
            public bool Update(DichVu dv)
            {
                using (var db = dataDataContext.Create())
                {
                    try
                    {
                        var existing = db.DichVus.FirstOrDefault(d => d.MaDichVu == dv.MaDichVu);
                        if (existing == null) return false;
                        existing.TenDichVu = dv.TenDichVu;
                        existing.Gia = dv.Gia;
                        existing.MoTa = dv.MoTa;
                        db.SubmitChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Lỗi khi cập nhật: {ex.Message}");
                    }
                }
            }
            public bool Delete(int maDichVu)
            {
                using (var db = dataDataContext.Create())
                {
                    try
                    {
                        var dv = db.DichVus.FirstOrDefault(d => d.MaDichVu == maDichVu);
                        if (dv == null) return false;
                        db.DichVus.DeleteOnSubmit(dv);
                        db.SubmitChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Lỗi khi xóa: {ex.Message}");
                    }
                }
            }
            public Dictionary<int, int> GetSoSuKienDung()
            {
                using (var db = dataDataContext.Create())
                {
                    return db.DichVuSuKiens
                             .GroupBy(d => d.MaDichVu)
                             .ToDictionary(g => g.Key, g => g.Count());
                }
            }
        }
    }