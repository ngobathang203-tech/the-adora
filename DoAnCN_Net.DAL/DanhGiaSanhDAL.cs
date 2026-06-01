using System;
using System.Collections.Generic;
using System.Linq;

namespace DoAnCN_Net.DAL
{
    public class DanhGiaSanhDAL
    {
        public List<DanhGiaSanh> GetBySanh(int maSanh)
        {
            using (var db = dataDataContext.Create())
                return db.DanhGiaSanhs
                         .Where(d => d.MaSanh == maSanh)
                         .OrderByDescending(d => d.NgayDanhGia)
                         .ToList();
        }

        public List<DanhGiaSanh> GetAll()
        {
            using (var db = dataDataContext.Create())
                return db.DanhGiaSanhs
                         .OrderByDescending(d => d.NgayDanhGia)
                         .ToList();
        }

        public DanhGiaSanh GetById(int maDanhGia)
        {
            using (var db = dataDataContext.Create())
                return db.DanhGiaSanhs
                         .FirstOrDefault(d => d.MaDanhGia == maDanhGia);
        }

        public double GetRatingTrungBinh(int maSanh)
        {
            using (var db = dataDataContext.Create())
            {
                var list = db.DanhGiaSanhs
                             .Where(d => d.MaSanh == maSanh)
                             .Select(d => (int)d.SoSao)
                             .ToList();
                return list.Count == 0 ? 0 : list.Average();
            }
        }

        public bool Add(DanhGiaSanh dg)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    db.DanhGiaSanhs.InsertOnSubmit(dg);
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi thêm đánh giá: {ex.Message}");
                }
            }
        }

        public bool Delete(int maDanhGia)
        {
            using (var db = dataDataContext.Create())
            {
                try
                {
                    var dg = db.DanhGiaSanhs
                               .FirstOrDefault(d => d.MaDanhGia == maDanhGia);
                    if (dg == null) return false;
                    db.DanhGiaSanhs.DeleteOnSubmit(dg);
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi xóa đánh giá: {ex.Message}");
                }
            }
        }

        public List<SoSaoThongKe> GetThongKeSao(int maSanh)
        {
            using (var db = dataDataContext.Create())
                return db.DanhGiaSanhs
                         .Where(d => d.MaSanh == maSanh)
                         .GroupBy(d => d.SoSao)
                         .Select(g => new SoSaoThongKe
                         {
                             SoSao = (int)g.Key,
                             SoLuot = g.Count()
                         })
                         .OrderByDescending(x => x.SoSao)
                         .ToList();
        }
    }

    public class SoSaoThongKe
    {
        public int SoSao { get; set; }
        public int SoLuot { get; set; }
        public string NhanSao => new string('★', SoSao) + new string('☆', 5 - SoSao);
    }
}